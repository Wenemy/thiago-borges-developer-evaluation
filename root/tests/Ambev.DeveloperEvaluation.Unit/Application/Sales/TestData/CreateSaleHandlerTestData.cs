using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

public static class CreateSaleHandlerTestData
{

    private static readonly Faker<CreateSaleCommand> createSaleHandlerFaker = new Faker<CreateSaleCommand>()
    .RuleFor(u => u.BranchId, f => Guid.NewGuid())
    .RuleFor(u => u.SaleNumber, f => Guid.NewGuid().ToString())
    .RuleFor(u => u.SaleDate, f => f.Date.Past(1))
    .RuleFor(u => u.CustomerId, f => Guid.NewGuid())
    .RuleFor(u => u.Items, f => createSaleItemFaker.Generate(f.Random.Int(1, 5)));

    private static readonly Faker<CreateSaleItemCommand> createSaleItemFaker = new Faker<CreateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => Guid.NewGuid())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10))
        .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(10, 500));

    public static CreateSaleCommand GenerateValidCommand()
    {
        return createSaleHandlerFaker.Generate();
    }
}
