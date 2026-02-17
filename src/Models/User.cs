using System.ComponentModel.DataAnnotations;

namespace _2026_SataAndagi_backend.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required] // Maps to NRP (Student) or NIP (Professor) or EmployeeID (Admin)
    public string IdentityNumber { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty;
    
    public string Role { get; set; } = "Student"; // Admin, Student, Professor
    
    // Navigation Properties (Optional, helpful for Include queries)
    public Student? StudentProfile { get; set; }
    public Professor? ProfessorProfile { get; set; }
}