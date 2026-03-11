using DentalClinicProject.Core.DTOs.Core.Create;
using FluentValidation;

namespace DentalClinicProject.Core.Validators.CreateValid
{
    public class CreateCartItemDTOValidator : AbstractValidator<CreateCartItemDTO>
    {
        public CreateCartItemDTOValidator()
        {
            RuleFor(x => x.ProductIds)
                .NotEmpty().WithMessage("At least one product must be added to the cart.")
                .Must(ids => ids.All(id => id > 0)).WithMessage("All product IDs must be greater than 0.");
        }
    }
}
