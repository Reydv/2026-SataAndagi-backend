using System.ComponentModel.DataAnnotations;

namespace _2026_SataAndagi_backend.Models;

public class Room
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Sector { get; set; } = string.Empty; // e.g., "Main Building", "Annex"

    [Range(1, 1000)]
    public int Capacity { get; set; }

    public bool IsAvailable { get; set; } = true;
    
    public bool IsDeleted { get; set; } = false; // Soft Delete Flag
}