using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    public class CreateSaleItemRequestValidator : AbstractValidator<CreateSaleItemRequest>
    {
        public CreateSaleItemRequestValidator()
        {
            RuleFor(saleItem => saleItem.ProductId).NotEmpty().NotEqual(Guid.Empty);

            RuleFor(saleItem => saleItem.Quantity)
            .GreaterThan(0).WithMessage("The quantity must be greater than zero.");

            RuleFor(saleItem => saleItem.UnitPrice)
            .GreaterThan(0).WithMessage("The unit price must be greater than zero.");
        }
    }
}
