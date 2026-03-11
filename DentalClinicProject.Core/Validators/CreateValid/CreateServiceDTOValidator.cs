using DentalClinicProject.Core.DTOs.Core.Create;
using FluentValidation;

namespace DentalClinicProject.Core.Validators.CreateValid
{
    public class CreateServiceDTOValidator : AbstractValidator<CreateServiceDTO>
    {
        public CreateServiceDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Service name is required.")
                .MinimumLength(3).WithMessage("Service name must be at least 3 characters.")
                .MaximumLength(200).WithMessage("Service name cannot exceed 200 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Service price must be greater than 0.");

            RuleFor(x => x.DurationInMinutes)
                .GreaterThan(0).WithMessage("Duration must be greater than 0 minutes.")
                .LessThanOrEqualTo(480).WithMessage("Duration cannot exceed 480 minutes (8 hours).");
        }
    }
}
