using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {
        RuleFor(saleItem => saleItem.ProductId).NotEmpty().NotEqual(Guid.Empty);

        RuleFor(saleItem => saleItem.Quantity)
            .GreaterThan(0).WithMessage("The quantity must be greater than zero.")
            .LessThanOrEqualTo(20).WithMessage("The quantity cannot be greater than 20.");
    }
}