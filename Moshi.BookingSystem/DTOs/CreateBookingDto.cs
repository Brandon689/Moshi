namespace Moshi.BookingSystem.DTOs;

public class CreateBookingDto
{
    public string CustomerName { get; set; }
    public DateTime BookingDate { get; set; }
    public string Resource { get; set; }
}