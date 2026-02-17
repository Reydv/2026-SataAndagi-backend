using System.ComponentModel.DataAnnotations;

namespace _2026_SataAndagi_backend.DTOs;

public class CreateRoomDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Sector { get; set; } = string.Empty;

    [Range(1, 1000)]
    public int Capacity { get; set; }
}

public class UpdateRoomDto : CreateRoomDto
{
    public bool IsAvailable { get; set; }
}