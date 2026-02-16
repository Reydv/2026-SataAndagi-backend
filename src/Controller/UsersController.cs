using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using _2026_SataAndagi_backend.Data;
using _2026_SataAndagi_backend.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace _2026_SataAndagi_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Critical: Must be logged in
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/users/{id}
    // Use Case: Admin viewing a specific user's details
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")] // Critical: Restrict to Admin
    public async Task<ActionResult<UserProfileDto>> GetUserInfo(int id)
    {
        // 1. Fetch User with specific Profile Data
        var user = await _context.Users
            .Include(u => u.StudentProfile)
            .Include(u => u.ProfessorProfile)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return NotFound("User ID not found.");

        // 2. Map to DTO (Same DTO as before, but for the target ID)
        var profile = new UserProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            IdentityNumber = user.IdentityNumber,
            Role = user.Role,
            // Smart Mapping: Pull department from whichever profile exists
            Department = user.StudentProfile?.Department ?? user.ProfessorProfile?.Department,
            Degree = user.StudentProfile?.Degree,
            ManagementPosition = user.ProfessorProfile?.ManagementPosition
        };

        return Ok(profile);
    }
}