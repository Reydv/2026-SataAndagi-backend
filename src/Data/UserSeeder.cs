using _2026_SataAndagi_backend.Models;
using BCrypt.Net;

namespace _2026_SataAndagi_backend.Data;

public static class UserSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        // 1. Check if database is empty
        if (context.Users.Any()) return;

        string passwordHash = BCrypt.Net.BCrypt.HashPassword("123");

        // 2. Create Users
        var adminUser = new User { Name = "Admin Chiyo", IdentityNumber = "ADM001", PasswordHash = passwordHash, Role = "Admin" };
        var studentUser = new User { Name = "Budi", IdentityNumber = "3124600061", PasswordHash = passwordHash, Role = "Student" };
        var professorUser = new User { Name = "Dr. Agus", IdentityNumber = "19800001", PasswordHash = passwordHash, Role = "Professor" };

        context.Users.AddRange(adminUser, studentUser, professorUser);
        context.SaveChanges(); // Save to generate Ids

        // 3. Create Profiles linked to Users
        var studentProfile = new Student { UserId = studentUser.Id, Department = "IT", Degree = "D4" };
        var professorProfile = new Professor { UserId = professorUser.Id, Department = "IT", ManagementPosition = "Ketua Lab" };

        context.Students.Add(studentProfile);
        context.Professors.Add(professorProfile);
        
        // 4. Commit transaction
        context.SaveChanges();
    }
}