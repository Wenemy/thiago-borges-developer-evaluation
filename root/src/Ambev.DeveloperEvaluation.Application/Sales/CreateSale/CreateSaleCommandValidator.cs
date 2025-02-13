using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
    {
        public CreateSaleCommandValidator()
        {
            RuleFor(sale => sale.CustomerId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(sale => sale.BranchId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(sale => sale.SaleNumber).NotEmpty();
            RuleFor(sale => sale.SaleDate).NotEmpty();
            RuleFor(sale => sale.Items).NotEmpty();
            RuleForEach(sale => sale.Items).SetValidator(new CreateSaleItemCommandValidator());
            RuleFor(sale => sale.Items)
                .Must(ValidateProductQuantitySum).WithMessage("The grouped quantity cannot be greater than 20.")
                .When(sale => sale.Items.Any());
        }

        private bool ValidateProductQuantitySum(IEnumerable<CreateSaleItemCommand> items)
        {
            var groupedItems = items
                .GroupBy(i => i.ProductId)
                .Select(group => new
                {
                    ProductId = group.Key,
                    TotalQuantity = group.Sum(i => i.Quantity)
                });

            return groupedItems.All(group => group.TotalQuantity <= 20);
        }
    }
}
