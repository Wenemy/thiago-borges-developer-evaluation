﻿using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale
{
    public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
    {
        public UpdateSaleRequestValidator()
        {
            RuleFor(sale => sale.CustomerId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(sale => sale.BranchId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(sale => sale.SaleNumber).NotEmpty();
            RuleFor(sale => sale.SaleDate).NotEmpty();
            RuleFor(sale => sale.Items).NotEmpty();
            RuleForEach(sale => sale.Items).SetValidator(new UpdateSaleItemRequestValidator());
            RuleFor(sale => sale.Items)
                .Must(ValidateProductQuantitySum).WithMessage("The grouped quantity cannot be greater than 20.")
                .When(sale => sale.Items.Any());
        }


        private bool ValidateProductQuantitySum(IEnumerable<UpdateSaleItemRequest> items)
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
