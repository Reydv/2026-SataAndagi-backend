using _2026_SataAndagi_backend.Models;

namespace _2026_SataAndagi_backend.Data;

public static class RoomSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Rooms.Any()) return;

        var rooms = new List<Room>
        {
            new Room { Name = "Lab A", Sector = "Gedung D4", Capacity = 30 },
            new Room { Name = "Lab B", Sector = "Gedung D4", Capacity = 30 },
            new Room { Name = "Auditorium", Sector = "Gedung Pascasarjana", Capacity = 200 },
            new Room { Name = "Meeting Room 1", Sector = "Gedung Pascasarjana", Capacity = 10 },
            new Room { Name = "Kelas 301", Sector = "Gedung Pascasarjana", Capacity = 50 }
        };

        context.Rooms.AddRange(rooms);
        context.SaveChanges();
    }
}