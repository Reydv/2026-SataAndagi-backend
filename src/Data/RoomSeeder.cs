using _2026_SataAndagi_backend.Models;

namespace _2026_SataAndagi_backend.Data;

public static class RoomSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Rooms.Any()) return;

        var rooms = new List<Room>
        {
            new Room { Name = "Lab A", Sector = "Computing Block", Capacity = 30 },
            new Room { Name = "Lab B", Sector = "Computing Block", Capacity = 30 },
            new Room { Name = "Auditorium", Sector = "Main Building", Capacity = 200 },
            new Room { Name = "Meeting Room 1", Sector = "Admin Block", Capacity = 10 },
            new Room { Name = "Classroom 101", Sector = "Main Building", Capacity = 50 }
        };

        context.Rooms.AddRange(rooms);
        context.SaveChanges();
    }
}