using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(sale => sale.SaleNumber).NotEmpty();
        RuleFor(sale => sale.CustomerId).NotEmpty().NotEqual(Guid.Empty);
        RuleFor(sale => sale.BranchId).NotEmpty().NotEqual(Guid.Empty);
        RuleForEach(sale => sale.Items).SetValidator(new SaleItemValidator());
    }
}
