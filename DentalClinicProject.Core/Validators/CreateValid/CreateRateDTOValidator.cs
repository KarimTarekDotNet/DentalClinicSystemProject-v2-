using DentalClinicProject.Core.DTOs.Core.Create;
using FluentValidation;

namespace DentalClinicProject.Core.Validators.CreateValid
{
    public class CreateRateDTOValidator : AbstractValidator<CreateRateDTO>
    {
        public CreateRateDTOValidator()
        {
            RuleFor(x => x.Value)
                .IsInEnum().WithMessage("Rating value must be a valid rating category.");

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Comment));

            RuleFor(x => x.AppointmentId)
                .GreaterThan(0).WithMessage("Appointment ID must be greater than 0.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product ID must be greater than 0.");

            RuleFor(x => x.DoctorId)
                .GreaterThan(0).WithMessage("Doctor ID must be greater than 0.");
        }
    }
}
