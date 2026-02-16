namespace _2026_SataAndagi_backend.DTOs;

public class UserProfileDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IdentityNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    
    // Profile Specifics (Nullable because they depend on role)
    public string? Department { get; set; }
    public string? Degree { get; set; }            // Only for Students
    public string? ManagementPosition { get; set; } // Only for Professors
}