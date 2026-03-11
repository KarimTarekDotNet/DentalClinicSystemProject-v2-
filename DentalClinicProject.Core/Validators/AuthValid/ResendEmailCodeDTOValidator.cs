using DentalClinicProject.Core.DTOs.Auth;
using FluentValidation;

namespace DentalClinicProject.Core.Validators.AuthValid
{
    public class ResendEmailCodeDTOValidator : AbstractValidator<ResendEmailCodeDTO>
    {
        public ResendEmailCodeDTOValidator()
        {
            RuleFor(x => x.SessionToken)
                .NotEmpty().WithMessage("Session token is required.");
        }
    }
}
