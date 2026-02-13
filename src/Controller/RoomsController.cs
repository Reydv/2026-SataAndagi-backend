using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2026_SataAndagi_backend.Data;
using _2026_SataAndagi_backend.DTOs;
using _2026_SataAndagi_backend.Models;
using Microsoft.AspNetCore.Authorization; // Uncomment if Auth required

namespace _2026_SataAndagi_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize(Roles = "Admin")] // Uncomment to restrict access
public class RoomsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RoomsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/rooms
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
    {
        return await _context.Rooms.ToListAsync();
    }

    // GET: api/rooms/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> GetRoom(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return NotFound();
        return room;
    }

    // POST: api/rooms
    [HttpPost]
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
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return NotFound();

        // Soft Delete Logic
        room.IsDeleted = true;
        
        await _context.SaveChangesAsync();
        return NoContent();
    }
}