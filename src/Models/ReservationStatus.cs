namespace _2026_SataAndagi_backend.Models;

public enum ReservationStatus
{
    Pending,  // Default: Waiting for Admin (Allows overlap)
    Approved, // Final: Blocks other bookings
    Rejected, 
    Cancelled // Soft Delete equivalent
}