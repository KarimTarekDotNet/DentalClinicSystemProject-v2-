using DentalClinicProject.Core.DTOs.Core.Create;
using FluentValidation;

namespace DentalClinicProject.Core.Validators.CreateValid
{
    public class CreateDoctorRateDTOValidator : AbstractValidator<CreateDoctorRateDTO>
    {
        public CreateDoctorRateDTOValidator()
        {
            RuleFor(x => x.Value)
                .IsInEnum().WithMessage("Rating value must be a valid rating category.");

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Comment));

            RuleFor(x => x.DoctorId)
                .GreaterThan(0).WithMessage("Doctor ID must be greater than 0.");
        }
    }
}
