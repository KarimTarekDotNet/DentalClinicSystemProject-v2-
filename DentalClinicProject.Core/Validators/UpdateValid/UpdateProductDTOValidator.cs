using DentalClinicProject.Core.DTOs.Core.Update;
using FluentValidation;

namespace DentalClinicProject.Core.Validators.UpdateValid
{
    public class UpdateProductDTOValidator : AbstractValidator<UpdateProductDTO>
    {
        public UpdateProductDTOValidator()
        {
            RuleFor(x => x.Name)
                .MinimumLength(3).WithMessage("Product name must be at least 3 characters.")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Description)
                .MinimumLength(10).WithMessage("Product description must be at least 10 characters.")
                .MaximumLength(1000).WithMessage("Product description cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Product price must be greater than 0.")
                .When(x => x.Price.HasValue);
        }
    }
}
