using DentalClinicProject.Core.DTOs;
using FluentValidation;

namespace DentalClinicProject.Core.Validators
{
    public class VerifyLoginCodeDTOValidator : AbstractValidator<VerifyLoginCodeDTO>
    {
        public VerifyLoginCodeDTOValidator()
        {
            RuleFor(x => x.Identifier)
                .NotEmpty()
                .WithMessage("Identifier is required")
                .MinimumLength(3)
                .WithMessage("Identifier must be at least 3 characters");

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("Verification code is required")
                .Length(6)
                .WithMessage("Verification code must be 6 characters");
        }
    }
}
