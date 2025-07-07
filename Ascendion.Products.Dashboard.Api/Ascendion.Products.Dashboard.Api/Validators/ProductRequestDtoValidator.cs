using Ascendion.Products.Dashboard.DTO.Product;
using FluentValidation;

namespace Ascendion.Products.Dashboard.Validators;

public class ProductRequestDtoValidator:AbstractValidator<ProductsRequestDto>
{
    private static readonly string[] AllowedStatus = { "Approved", "Pending","Rejected" };
    public ProductRequestDtoValidator()
    {
        RuleFor(products => products.Name).NotNull().WithMessage("Products Name should be present");
        RuleFor(products => products.Status).NotNull()
            .Must(Status => AllowedStatus.Contains(Status))
            .WithMessage($"Status must be one of the following {string.Join(", ",AllowedStatus)}");

    }
}
