using FluentValidation;

namespace Moshi.BookingSystem.Validators;

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BookingDate).NotEmpty().GreaterThan(DateTime.Now);
        RuleFor(x => x.Resource).NotEmpty().MaximumLength(50);
    }
}