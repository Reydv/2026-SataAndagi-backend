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

    // GET: api/users/profile
    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileDto>> GetMyProfile()
    {
        // 1. Identify User from Token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        int userId = int.Parse(userIdClaim.Value);

        // 2. Fetch User WITH their specific profile
        // We use "Include" to join the Student/Professor tables
        var user = await _context.Users
            .Include(u => u.StudentProfile)
            .Include(u => u.ProfessorProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return NotFound("User account not found.");

        // 3. Map to DTO
        var profile = new UserProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            IdentityNumber = user.IdentityNumber,
            Role = user.Role,
            // Determine which profile data to fill
            Department = user.StudentProfile?.Department ?? user.ProfessorProfile?.Department,
            Degree = user.StudentProfile?.Degree,
            ManagementPosition = user.ProfessorProfile?.ManagementPosition
        };

        return Ok(profile);
    }
}