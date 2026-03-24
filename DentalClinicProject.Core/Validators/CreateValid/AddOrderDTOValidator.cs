using DentalClinicProject.Core.DTOs.Core.Create;
using FluentValidation;

namespace DentalClinicProject.Core.Validators.CreateValid
{
    public class AddOrderDTOValidator : AbstractValidator<CreateOrderDTO>
    {
        public AddOrderDTOValidator()
        {
            RuleFor(x => x.DeliveryId)
                .GreaterThan(0).WithMessage("DeliveryId must be greater than 0.");

            RuleFor(x => x.DeliveryDate)
                .NotEmpty().WithMessage("DeliveryDate is required.")
                .Must(date => date >= DateTime.UtcNow.Date)
                .WithMessage("DeliveryDate cannot be in the past.");

            RuleFor(x => x.Items)
                .NotNull().WithMessage("Items cannot be null.")
                .Must(items => items.Any())
                .WithMessage("Order must contain at least one item.");

            RuleForEach(x => x.Items)
                .SetValidator(new AddOrderItemDTOValidator());
        }
    }
}
