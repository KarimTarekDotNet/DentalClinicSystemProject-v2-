using DentalClinicProject.Core.DTOs;
using FluentValidation;

namespace DentalClinicProject.Core.Validators
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
