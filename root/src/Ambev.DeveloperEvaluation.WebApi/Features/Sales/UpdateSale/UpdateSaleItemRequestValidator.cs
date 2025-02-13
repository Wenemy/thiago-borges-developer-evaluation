using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale
{
    public class UpdateSaleItemRequestValidator : AbstractValidator<UpdateSaleItemRequest>
    {
        public UpdateSaleItemRequestValidator()
        {
            RuleFor(saleItem => saleItem.ProductId).NotEmpty().NotEqual(Guid.Empty);

            RuleFor(saleItem => saleItem.Quantity)
            .GreaterThan(0).WithMessage("The quantity must be greater than zero.");

            RuleFor(saleItem => saleItem.UnitPrice)
            .GreaterThan(0).WithMessage("The unit price must be greater than zero.");
        }
    }
}
