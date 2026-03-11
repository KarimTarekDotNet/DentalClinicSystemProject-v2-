using DentalClinicProject.Core.DTOs.Core.Update;
using FluentValidation;

namespace DentalClinicProject.Core.Validators.UpdateValid
{
    public class UpdateRateDTOValidator : AbstractValidator<UpdateRateDTO>
    {
        public UpdateRateDTOValidator()
        {
            RuleFor(x => x.Value)
                .IsInEnum().WithMessage("Rating value must be a valid rating category.")
                .When(x => x.Value.HasValue);

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Comment));
        }
    }
}
