using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
    {
        public UpdateSaleCommandValidator()
        {
            RuleFor(sale => sale.Id).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(sale => sale.CustomerId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(sale => sale.BranchId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(sale => sale.SaleNumber).NotEmpty();
            RuleFor(sale => sale.SaleDate).NotEmpty();
            RuleFor(sale => sale.Items).NotEmpty();
            RuleForEach(sale => sale.Items).SetValidator(new UpdateSaleItemCommandValidator());
            RuleFor(sale => sale.Items)
                .Must(ValidateProductQuantitySum).WithMessage("The grouped quantity cannot be greater than 20.")
                .When(sale => sale.Items.Any());
        }

        private bool ValidateProductQuantitySum(IEnumerable<UpdateSaleItemCommand> items)
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
