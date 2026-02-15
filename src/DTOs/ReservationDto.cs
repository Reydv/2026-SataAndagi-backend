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

public class ReservationDetailDto
{
    public int Id { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty; // Included as requested
    public DateTime CreatedAt { get; set; }
}

public class UpdateReservationStatusDto
{
    public string Status { get; set; } = string.Empty; // "Approved" or "Rejected"
}

public class UpdateReservationDto
{
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Admin can fix status here
}