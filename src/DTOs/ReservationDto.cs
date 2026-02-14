using System.ComponentModel.DataAnnotations;

namespace _2026_SataAndagi_backend.DTOs;

public class CreateReservationDto
{
    [Required]
    public int RoomId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }
    
    public string Purpose { get; set; } = string.Empty;
}