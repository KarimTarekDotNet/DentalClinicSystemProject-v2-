using DentalClinicProject.Core.DTOs.Core.Create;
using FluentValidation;

namespace DentalClinicProject.Core.Validators.CreateValid
{
    public class AddPaymentDTOValidator : AbstractValidator<AddPaymentDTO>
    {
        public AddPaymentDTOValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .Must(a => decimal.Round(a, 2) == a)
                .WithMessage("Amount must have max 2 decimal places.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .Length(3)
                .Matches("^[A-Z]{3}$")
                .WithMessage("Currency must be a valid ISO code (e.g. USD, EUR).");

            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .Must(id => !string.IsNullOrWhiteSpace(id))
                .WithMessage("CustomerId cannot be empty.");
        }
    }
}
