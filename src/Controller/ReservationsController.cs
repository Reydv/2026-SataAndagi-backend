using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using _2026_SataAndagi_backend.Data;
using _2026_SataAndagi_backend.DTOs;
using _2026_SataAndagi_backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace _2026_SataAndagi_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Only authenticated users can book
public class ReservationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReservationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/reservations
    [HttpPost]
    public async Task<IActionResult> CreateReservation(CreateReservationDto request)
    {
        // 1. Basic Validation
        if (request.EndTime <= request.StartTime)
            return BadRequest("End time must be after start time.");

        // 2. Identify the User (from JWT)
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        int userId = int.Parse(userIdClaim.Value);

        // 3. CRITICAL: Availability Check 
        // Rule: Block ONLY if there is an overlapping APPROVED reservation.
        // We DO NOT block if there are only PENDING reservations (Queue system).
        // Rule: User cannot request the SAME room if they already have a Pending/Approved request overlapping this time.
        bool userHasConflict = await _context.Reservations.AnyAsync(r =>
            r.UserId == userId &&                 // Check current user's history
            r.RoomId == request.RoomId &&         // Check specific room
            r.Status != ReservationStatus.Rejected &&  // Ignore Rejected
            r.Status != ReservationStatus.Cancelled && // Ignore Cancelled
            r.StartTime < request.EndTime &&      // Overlap Check
            r.EndTime > request.StartTime);

        if (isBlocked)
        {
            return Conflict("This room is already fully booked (Approved) for this time slot.");
        }

        // 4. Create the Booking (Queue Entry)
        var reservation = new Reservation
        {
            RoomId = request.RoomId,
            UserId = userId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Purpose = request.Purpose,
            Status = ReservationStatus.Pending 
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(CreateReservation), new { id = reservation.Id }, reservation);
    }

    // DELETE: api/reservations/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelReservation(int id)
    {
        // Get user ID to ensure they only delete their own booking
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        var reservation = await _context.Reservations.FindAsync(id);

        if (reservation == null) return NotFound();
        
        // Optional: Allow Admin to delete anyone's, but User can only delete theirs
        if (reservation.UserId != userId && !User.IsInRole("Admin")) 
            return Forbid();

        // Soft Delete 
        reservation.DeletedAt = DateTime.UtcNow;
        reservation.Status = ReservationStatus.Cancelled;
        
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // GET: api/reservations?page=1&pageSize=10&status=Pending
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationDetailDto>>> GetReservations(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null) // Filter Parameter
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

        // 1. Base Query with Relations
        var query = _context.Reservations
            .Include(r => r.Room)
            .Include(r => r.User)
            .AsQueryable();

        // 2. Role Filter: Users see ONLY their own; Admins see ALL
        if (userRole != "Admin")
        {
            query = query.Where(r => r.UserId == userId);
        }

        // 3. Status Filter (Optional)
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ReservationStatus>(status, true, out var statusEnum))
        {
            query = query.Where(r => r.Status == statusEnum);
        }

        // 4. Pagination & Mapping
        var reservations = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ReservationDetailDto
            {
                Id = r.Id,
                RoomName = r.Room!.Name,
                UserName = r.User!.Name,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                Status = r.Status.ToString(),
                Purpose = r.Purpose,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return Ok(reservations);
    }

    // PATCH: api/reservations/{id}/status (Admin Only)
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateReservationStatusDto request)
    {
        // 1. Validate Input Status
        if (!Enum.TryParse<ReservationStatus>(request.Status, true, out var newStatus))
        {
            return BadRequest("Invalid status. Use 'Approved' or 'Rejected'.");
        }

        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null) return NotFound();

        // 2. Apply Status Change
        reservation.Status = newStatus;

        // 3. Cascading Rejection Logic (Only if Approved)
        // If we Approve this, we must Reject all other Pending requests that overlap.
        if (newStatus == ReservationStatus.Approved)
        {
            var conflicts = await _context.Reservations
                .Where(r => r.RoomId == reservation.RoomId &&
                            r.Id != reservation.Id && // Exclude self
                            r.Status == ReservationStatus.Pending && // Only affect Pending ones
                            r.StartTime < reservation.EndTime && // Overlap Check
                            r.EndTime > reservation.StartTime)
                .ToListAsync();

            foreach (var conflict in conflicts)
            {
                conflict.Status = ReservationStatus.Rejected;
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { Message = $"Reservation updated to {newStatus}" });
    }
}