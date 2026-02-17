using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2026_SataAndagi_backend.Data;
using _2026_SataAndagi_backend.DTOs;
using _2026_SataAndagi_backend.Models;
using Microsoft.AspNetCore.Authorization; // Uncomment if Auth required

namespace _2026_SataAndagi_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RoomsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/rooms
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
    {
        return await _context.Rooms.ToListAsync();
    }

    // GET: api/rooms/5
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> GetRoom(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return NotFound();
        return room;
    }

    // POST: api/rooms
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Room>> CreateRoom(CreateRoomDto request)
    {
        var room = new Room
        {
            Name = request.Name,
            Sector = request.Sector,
            Capacity = request.Capacity,
            IsAvailable = true
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
    }

    // PUT: api/rooms/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRoom(int id, UpdateRoomDto request)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return NotFound();

        room.Name = request.Name;
        room.Sector = request.Sector;
        room.Capacity = request.Capacity;
        room.IsAvailable = request.IsAvailable;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/rooms/5 (Soft Delete)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return NotFound();

        // Soft Delete Logic
        room.IsDeleted = true;
        
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // GET: api/rooms/availability
    [HttpGet("availability")]
    public async Task<ActionResult<IEnumerable<RoomAvailabilityDto>>> SearchAvailableRooms(
        [FromQuery] DateTime? startDate, 
        [FromQuery] DateTime? endDate, 
        [FromQuery] string? sector, 
        [FromQuery] int minCapacity = 0,
        [FromQuery] string? search = null) // Added search parameter 
    {
        // 1. Set Defaults: Current Time to +3 Hours 
        DateTime start = startDate ?? DateTime.UtcNow;
        DateTime end = endDate ?? DateTime.UtcNow.AddHours(3);

        if (end <= start) return BadRequest("End time must be after start time.");

        // 2. Base Query: Only Active Rooms
        var query = _context.Rooms.AsQueryable()
            .Where(r => r.IsAvailable && !r.IsDeleted);

        // 3. Apply Filters
        if (!string.IsNullOrEmpty(sector))
        {
            query = query.Where(r => r.Sector == sector); // 
        }

        if (minCapacity > 0)
        {
            query = query.Where(r => r.Capacity >= minCapacity); // 
        }

        // Name Search (Case-Insensitive Partial Match) 
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(r => r.Name.Contains(search));
        }

        // 4. Exclusion Logic: Overlap Check
        // Exclude ONLY if there is an overlapping APPROVED reservation .
        // Pending reservations are ignored (users can compete) .
        var availableRooms = await query
            .Where(r => !_context.Reservations.Any(res => 
                res.RoomId == r.Id &&
                res.Status == ReservationStatus.Approved && 
                res.StartTime < end && 
                res.EndTime > start))
            .Select(r => new RoomAvailabilityDto 
            {
                Id = r.Id, // Included Id in response 
                Name = r.Name,
                Sector = r.Sector,
                Capacity = r.Capacity
            })
            .ToListAsync();

        return Ok(availableRooms);
    }
}