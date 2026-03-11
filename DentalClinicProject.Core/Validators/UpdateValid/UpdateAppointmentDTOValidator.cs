using DentalClinicProject.Core.DTOs.Core.Update;
using FluentValidation;

namespace DentalClinicProject.Core.Validators.UpdateValid
{
    public class UpdateAppointmentDTOValidator : AbstractValidator<UpdateAppointmentDTO>
    {
        public UpdateAppointmentDTOValidator()
        {
            RuleFor(x => x.DoctorId)
                .GreaterThan(0).WithMessage("Doctor ID must be greater than 0.")
                .When(x => x.DoctorId.HasValue);

            RuleFor(x => x.ServiceId)
                .GreaterThan(0).WithMessage("Service ID must be greater than 0.")
                .When(x => x.ServiceId.HasValue);

            RuleFor(x => x.ExaminationAppointment)
                .Must(BeAFutureDate).WithMessage("Examination appointment must be in the future.")
                .Must(BeWithinWorkingHours).WithMessage("Examination appointment must be between 9 AM and 5 PM.")
                .Must(NotBeFriday).WithMessage("Appointments cannot be scheduled on Fridays.")
                .When(x => x.ExaminationAppointment.HasValue);
        }

        private bool BeAFutureDate(DateTime? date)
        {
            return date.HasValue && date.Value > DateTime.Now;
        }

        private bool BeWithinWorkingHours(DateTime? date)
        {
            return date.HasValue && date.Value.Hour >= 9 && date.Value.Hour < 17;
        }

        private bool NotBeFriday(DateTime? date)
        {
            return date.HasValue && date.Value.DayOfWeek != DayOfWeek.Friday;
        }
    }
}
