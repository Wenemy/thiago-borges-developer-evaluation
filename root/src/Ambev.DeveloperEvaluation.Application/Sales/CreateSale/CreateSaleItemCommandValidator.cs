using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleItemCommandValidator : AbstractValidator<CreateSaleItemCommand>
    {
        public CreateSaleItemCommandValidator()
        {
            RuleFor(saleItem => saleItem.ProductId).NotEmpty().NotEqual(Guid.Empty);

            RuleFor(saleItem => saleItem.Quantity)
            .GreaterThan(0).WithMessage("The quantity must be greater than zero.");

            RuleFor(saleItem => saleItem.UnitPrice)
            .GreaterThan(0).WithMessage("The unit price must be greater than zero.");
        }
    }
}
