namespace _2026_SataAndagi_backend.Models;

public class Student
{
    public int Id { get; set; }
    
    public int UserId { get; set; } // Foreign Key
    public User? User { get; set; }
    
    public string Department { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty; // e.g., D3, D4
}