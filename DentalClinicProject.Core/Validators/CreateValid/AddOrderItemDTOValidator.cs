using DentalClinicProject.Core.DTOs.Core.Create;
using FluentValidation;

namespace DentalClinicProject.Core.Validators.CreateValid
{
    public class AddOrderItemDTOValidator : AbstractValidator<CreateOrderItemDTO>
    {
        public AddOrderItemDTOValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("ProductId must be greater than 0.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }
}
