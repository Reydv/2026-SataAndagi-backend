namespace _2026_SataAndagi_backend.DTOs;

public class RoomAvailabilityDto
{
    public int Id { get; set; } // Added per revised requirement 
    public string Name { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public int Capacity { get; set; }
}