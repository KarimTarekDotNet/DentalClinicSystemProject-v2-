using DentalClinicProject.Core.DTOs.Core.Update;
using FluentValidation;

namespace DentalClinicProject.Core.Validators.UpdateValid
{
    public class UpdateServiceDTOValidator : AbstractValidator<UpdateServiceDTO>
    {
        public UpdateServiceDTOValidator()
        {
            RuleFor(x => x.Name)
                .MinimumLength(3).WithMessage("Service name must be at least 3 characters.")
                .MaximumLength(200).WithMessage("Service name cannot exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Service price must be greater than 0.")
                .When(x => x.Price.HasValue);

            RuleFor(x => x.DurationInMinutes)
                .GreaterThan(0).WithMessage("Duration must be greater than 0 minutes.")
                .LessThanOrEqualTo(480).WithMessage("Duration cannot exceed 480 minutes (8 hours).")
                .When(x => x.DurationInMinutes.HasValue);
        }
    }
}
