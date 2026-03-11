using DentalClinicProject.Core.DTOs.Core.Create;
using FluentValidation;

namespace DentalClinicProject.Core.Validators.CreateValid
{
    public class CreateAppointmentDTOValidator : AbstractValidator<CreateAppointmentDTO>
    {
        public CreateAppointmentDTOValidator()
        {
            RuleFor(x => x.DoctorId)
                .GreaterThan(0).WithMessage("Doctor ID must be greater than 0.");

            RuleFor(x => x.ServiceId)
                .GreaterThan(0).WithMessage("Service ID must be greater than 0.");

            RuleFor(x => x.ExaminationAppointment)
                .NotEmpty().WithMessage("Examination appointment date is required.")
                .Must(BeAFutureDate).WithMessage("Examination appointment must be in the future.")
                .Must(BeWithinWorkingHours).WithMessage("Examination appointment must be between 9 AM and 5 PM.")
                .Must(NotBeFriday).WithMessage("Appointments cannot be scheduled on Fridays.");
        }

        private bool BeAFutureDate(DateTime date)
        {
            return date > DateTime.Now;
        }

        private bool BeWithinWorkingHours(DateTime date)
        {
            return date.Hour >= 9 && date.Hour < 17;
        }

        private bool NotBeFriday(DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Friday;
        }
    }
}
