using DentalClinicProject.Core.DTOs;
using FluentValidation;

namespace DentalClinicProject.Core.Validators
{
    public class VerifyEmailDTOValidator : AbstractValidator<VerifyEmailDTO>
    {
        public VerifyEmailDTOValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("Verification code is required")
                .Length(6)
                .WithMessage("Verification code must be 6 characters");
        }
    }
}
