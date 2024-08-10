namespace Moshi.BookingSystem.Models;

public class Booking
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public DateTime BookingDate { get; set; }
    public string Resource { get; set; }
    public string Status { get; set; }
}