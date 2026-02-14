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
        bool isBlocked = await _context.Reservations.AnyAsync(r => 
            r.RoomId == request.RoomId &&
            r.Status == ReservationStatus.Approved && // Only Approved blocks it
            r.StartTime < request.EndTime && 
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
}