namespace Moshi.BookingSystem.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public DateTime BookingDate { get; set; }
    public string Resource { get; set; }
    public string Status { get; set; }
}