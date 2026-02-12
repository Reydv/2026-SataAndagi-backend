using _2026_SataAndagi_backend.Models;

namespace _2026_SataAndagi_backend.Data;

public static class UserSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        // 1. Check if database is empty
        if (context.Users.Any()) return;

        // 2. Create Users
        var adminUser = new User { Name = "Admin Chiyo", IdentityNumber = "ADM001", PasswordHash = "123", Role = "Admin" };
        var studentUser = new User { Name = "Budi", IdentityNumber = "3124600061", PasswordHash = "123", Role = "Student" };
        var professorUser = new User { Name = "Dr. Agus", IdentityNumber = "19800001", PasswordHash = "123", Role = "Professor" };

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