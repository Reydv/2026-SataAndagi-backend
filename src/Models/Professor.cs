namespace _2026_SataAndagi_backend.Models;

public class Professor
{
    public int Id { get; set; }
    
    public int UserId { get; set; } // Foreign Key
    public User? User { get; set; }
    
    public string Department { get; set; } = string.Empty;
    public string ManagementPosition { get; set; } = "Lecturer"; // e.g., Head of Dept
}