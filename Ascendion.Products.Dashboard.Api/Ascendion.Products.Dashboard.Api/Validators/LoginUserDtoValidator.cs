using Ascendion.Products.Dashboard.DTO.Auth;
using FluentValidation;

namespace Ascendion.Products.Dashboard.Validators;

public class LoginUserDtoValidator:AbstractValidator<LoginRequestDto>
{
    public LoginUserDtoValidator()
    {
        RuleFor(user => user.Email).NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
        RuleFor(user => user.Password).NotEmpty().WithMessage("Password is Required")
            .MinimumLength(6).WithMessage("Password must be at least 8 characters long.").
            Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character and be at least 8 characters long.");

    }
}
