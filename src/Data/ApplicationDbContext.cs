using Microsoft.EntityFrameworkCore;
using _2026_SataAndagi_backend.Models;

namespace _2026_SataAndagi_backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Professor> Professors { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Reservation> Reservations { get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Enforce Unique Identity Number (NRP/NIP)
        modelBuilder.Entity<User>()
            .HasIndex(u => u.IdentityNumber)
            .IsUnique();

        // Configure User -> Student (One-to-One)
        modelBuilder.Entity<User>()
            .HasOne(u => u.StudentProfile)
            .WithOne(s => s.User)
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade); // If User deleted, delete Profile

        // Configure User -> Professor (One-to-One)
        modelBuilder.Entity<User>()
            .HasOne(u => u.ProfessorProfile)
            .WithOne(p => p.User)
            .HasForeignKey<Professor>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Room>().HasQueryFilter(r => !r.IsDeleted);
        modelBuilder.Entity<Reservation>().HasQueryFilter(r => r.DeletedAt == null);
    }
}