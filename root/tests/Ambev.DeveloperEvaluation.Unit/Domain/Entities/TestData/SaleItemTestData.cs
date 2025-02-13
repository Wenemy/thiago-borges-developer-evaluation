using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleItemTestData
{
    public static SaleItem GenerateValidSaleItem(Guid? productId = null, int quantity = 1, decimal price = 10)
    {
        var id = productId ?? Guid.NewGuid();

        return new Faker<SaleItem>()
            .CustomInstantiator(f => new SaleItem(
                Guid.NewGuid(),
                GenerateValidProduct(id),
                quantity,
                price
            ))
            .RuleFor(i => i.ProductId, id)
            .Generate();
    }

    public static Product GenerateValidProduct(Guid productId)
    {
        return new Product(productId, new Faker().Commerce.ProductName());
    }

    public static Product GenerateInvalidProduct()
    {
        return new Product(Guid.Empty, "");
    }
}
