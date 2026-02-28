using DentalClinicProject.Core.DTOs;
using FluentValidation;

namespace DentalClinicProject.Core.Validators
{
    public class VerifyPhoneDTOValidator : AbstractValidator<VerifyPhoneDTO>
    {
        public VerifyPhoneDTOValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("Verification code is required")
                .Length(6)
                .WithMessage("Verification code must be 6 characters")
                .Matches(@"^\d{6}$")
                .WithMessage("Verification code must contain only numbers");
        }
    }
}
