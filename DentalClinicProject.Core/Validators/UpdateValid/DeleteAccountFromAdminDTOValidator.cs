using DentalClinicProject.Core.DTOs.Core.Update;
using FluentValidation;

namespace DentalClinicProject.Core.Validators.UpdateValid
{
    public class DeleteAccountFromAdminDTOValidator : AbstractValidator<DeleteAccountFromAdminDTO>
    {
        public DeleteAccountFromAdminDTOValidator()
        {
            RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Please enter a valid email address.");

            RuleFor(x => x.Password)
               .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$")
               .WithMessage("Password must be 8–15 characters long and include at least one uppercase letter, one lowercase letter," +
               " one number, and one special character.");
        }
    }
}
