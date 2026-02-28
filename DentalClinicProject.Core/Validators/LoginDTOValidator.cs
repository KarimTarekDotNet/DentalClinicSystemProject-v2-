using DentalClinicProject.Core.DTOs;
using FluentValidation;

namespace DentalClinicProject.Core.Validators
{
    public class LoginDTOValidator : AbstractValidator<LoginDTO>
    {
        public LoginDTOValidator()
        {
            RuleFor(x => x.Identifier)
                .NotEmpty()
                .WithMessage("Email, Username, or Phone Number is required")
                .MinimumLength(3)
                .WithMessage("Identifier must be at least 3 characters")
                .MaximumLength(100)
                .WithMessage("Identifier cannot exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters");
        }
    }
}
