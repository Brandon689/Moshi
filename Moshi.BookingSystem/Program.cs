using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moshi.BookingSystem;
using Moshi.BookingSystem.Data;
using Moshi.BookingSystem.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database context
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

// Logging
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();

// Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();

// Error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// CRUD Endpoints

// Create
app.MapPost("/bookings", async (BookingDbContext db, IValidator<CreateBookingDto> validator, CreateBookingDto bookingDto) =>
{
    var validationResult = await validator.ValidateAsync(bookingDto);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }

    var booking = new Booking
    {
        CustomerName = bookingDto.CustomerName,
        BookingDate = bookingDto.BookingDate,
        Resource = bookingDto.Resource,
        Status = "Pending" // Default status
    };

    db.Bookings.Add(booking);
    await db.SaveChangesAsync();
    return Results.Created($"/bookings/{booking.Id}", booking);
})
.RequireAuthorization()
.WithName("CreateBooking")
.WithOpenApi();

// Read (all with filtering, sorting, and pagination)
app.MapGet("/bookings", async (
    BookingDbContext db,
    [FromQuery] string? customerName,
    [FromQuery] DateTime? fromDate,
    [FromQuery] DateTime? toDate,
    [FromQuery] string? sortBy,
    [FromQuery] bool descending = false,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10
    ) =>
{
    var query = db.Bookings.AsQueryable();

    if (!string.IsNullOrEmpty(customerName))
        query = query.Where(b => b.CustomerName.Contains(customerName));

    if (fromDate.HasValue)
        query = query.Where(b => b.BookingDate >= fromDate.Value);

    if (toDate.HasValue)
        query = query.Where(b => b.BookingDate <= toDate.Value);

    if (!string.IsNullOrEmpty(sortBy))
    {
        query = sortBy.ToLower() switch
        {
            "customername" => descending ? query.OrderByDescending(b => b.CustomerName) : query.OrderBy(b => b.CustomerName),
            "bookingdate" => descending ? query.OrderByDescending(b => b.BookingDate) : query.OrderBy(b => b.BookingDate),
            _ => query.OrderBy(b => b.Id)
        };
    }

    var totalItems = await query.CountAsync();
    var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

    var bookings = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return Results.Ok(new
    {
        TotalItems = totalItems,
        TotalPages = totalPages,
        Page = page,
        PageSize = pageSize,
        Items = bookings
    });
})
.WithName("GetBookings")
.WithOpenApi();

// Read (single)
app.MapGet("/bookings/{id}", async (int id, BookingDbContext db) =>
    await db.Bookings.FindAsync(id)
        is Booking booking
            ? Results.Ok(booking)
            : Results.NotFound())
.WithName("GetBooking")
.WithOpenApi();

// Update
app.MapPut("/bookings/{id}", async (int id, BookingDto inputBooking, BookingDbContext db) =>
{
    var booking = await db.Bookings.FindAsync(id);

    if (booking is null) return Results.NotFound();

    booking.CustomerName = inputBooking.CustomerName;
    booking.BookingDate = inputBooking.BookingDate;
    booking.Resource = inputBooking.Resource;
    booking.Status = inputBooking.Status;

    await db.SaveChangesAsync();

    return Results.NoContent();
})
.RequireAuthorization()
.WithName("UpdateBooking")
.WithOpenApi();

// Delete
app.MapDelete("/bookings/{id}", async (int id, BookingDbContext db) =>
{
    if (await db.Bookings.FindAsync(id) is Booking booking)
    {
        db.Bookings.Remove(booking);
        await db.SaveChangesAsync();
        return Results.Ok(booking);
    }

    return Results.NotFound();
})
.RequireAuthorization()
.WithName("DeleteBooking")
.WithOpenApi();

app.MapPost("/login", (UserCredentials credentials) =>
{
    // TODO: Validate credentials against your user store
    if (IsValidUser(credentials))
    {
        var token = GenerateJwtToken(credentials.Username);
        return Results.Ok(new { Token = token });
    }
    return Results.Unauthorized();
});

// Helper methods (implement these based on your needs)
bool IsValidUser(UserCredentials credentials)
{
    // TODO: Implement user validation logic
    return credentials.Username == "test" && credentials.Password == "moshi123";
}

string GenerateJwtToken(string username)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: builder.Configuration["Jwt:Issuer"],
        audience: builder.Configuration["Jwt:Audience"],
        claims: new[] { new Claim(ClaimTypes.Name, username) },
        expires: DateTime.Now.AddHours(1),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}




app.Run();
public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
// DTO and Validator definitions (typically these would be in separate files)
public class UserCredentials
{
    public string Username { get; set; }
    public string Password { get; set; }
}
public class BookingDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public DateTime BookingDate { get; set; }
    public string Resource { get; set; }
    public string Status { get; set; }
}

public class CreateBookingDto
{
    public string CustomerName { get; set; }
    public DateTime BookingDate { get; set; }
    public string Resource { get; set; }
}

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BookingDate).NotEmpty().GreaterThan(DateTime.Now);
        RuleFor(x => x.Resource).NotEmpty().MaximumLength(50);
    }
}
