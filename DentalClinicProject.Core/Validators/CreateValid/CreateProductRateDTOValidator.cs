using DentalClinicProject.Core.DTOs.Core.Create;
using FluentValidation;

namespace DentalClinicProject.Core.Validators.CreateValid
{
    public class CreateProductRateDTOValidator : AbstractValidator<CreateProductRateDTO>
    {
        public CreateProductRateDTOValidator()
        {

            RuleFor(x => x.Value)
                .IsInEnum().WithMessage("Rating value must be a valid rating category.");

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Comment));

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product ID must be greater than 0.");
        }
    }
}
