using DentalClinicProject.Core.DTOs;
using FluentValidation;

namespace DentalClinicProject.Core.Validator
{
    public class ValidRegister : AbstractValidator<RegisterDTO>
    {
        public ValidRegister()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MinimumLength(4).WithMessage("First name must be at least 4 characters.")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MinimumLength(4).WithMessage("Last name must be at least 4 characters.")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(4).WithMessage("Username must be at least 4 characters.")
                .MaximumLength(100).WithMessage("Username cannot exceed 100 characters.")
                .Matches(@"^(?!.*[_\\s-]{2,})[a-zA-Z0-9][a-zA-Z0-9_\\s\\-]*[a-zA-Z0-9]$")
                .WithMessage("Username must start and end with a letter or number and cannot contain consecutive spaces, hyphens, or underscores.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Please enter a valid email address.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,15}$")
                .WithMessage("Password must be 8–15 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(x => x.Password)
                .WithMessage("Confirm password must match the password.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(\([0-9]{3}\)|[0-9]{3})[\s\-]?[0-9]{3}[\s\-]?[0-9]{4}$")
                .WithMessage("Phone number must be in a valid format (e.g., 123-456-7890 or (123) 456-7890).");
        }
    }
}
