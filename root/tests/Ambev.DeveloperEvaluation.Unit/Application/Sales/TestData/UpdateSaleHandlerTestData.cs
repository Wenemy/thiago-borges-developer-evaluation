using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

public static class UpdateSaleHandlerTestData
{

    private static readonly Faker<UpdateSaleCommand> saleHandlerFaker = new Faker<UpdateSaleCommand>()
    .RuleFor(u => u.Id, f => Guid.NewGuid())
    .RuleFor(u => u.BranchId, f => Guid.NewGuid())
    .RuleFor(u => u.SaleNumber, f => Guid.NewGuid().ToString())
    .RuleFor(u => u.SaleDate, f => f.Date.Past(1))
    .RuleFor(u => u.CustomerId, f => Guid.NewGuid())
    .RuleFor(u => u.Items, f => saleItemFaker.Generate(f.Random.Int(1, 5)));

    private static readonly Faker<UpdateSaleItemCommand> saleItemFaker = new Faker<UpdateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => Guid.NewGuid())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10))
        .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(10, 500));

    public static UpdateSaleCommand GenerateValidCommand()
    {
        return saleHandlerFaker.Generate();
    }
}
