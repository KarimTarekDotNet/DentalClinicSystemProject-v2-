using DentalClinicProject.Core.DTOs.Core.Update;
using FluentValidation;

namespace DentalClinicProject.Core.Validators.UpdateValid
{
    public class UpdateCartItemDTOValidator : AbstractValidator<UpdateCartItemDTO>
    {
        public UpdateCartItemDTOValidator()
        {
            RuleFor(x => x.ProductIds)
                .Must(ids => ids == null || ids.All(id => id > 0))
                .WithMessage("All product IDs must be greater than 0.")
                .When(x => x.ProductIds != null);
        }
    }
}
