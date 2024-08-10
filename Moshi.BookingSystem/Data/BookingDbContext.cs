using Microsoft.EntityFrameworkCore;
using Moshi.BookingSystem.Models;

namespace Moshi.BookingSystem.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

    public DbSet<Booking> Bookings { get; set; }
}