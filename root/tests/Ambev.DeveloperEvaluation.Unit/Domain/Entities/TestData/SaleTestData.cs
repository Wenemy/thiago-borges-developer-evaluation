using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;
using Microsoft.CodeAnalysis;
using NSubstitute.ReceivedExtensions;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleTestData
{
    public static readonly Faker<Sale> SaleFake = new Faker<Sale>()
        .CustomInstantiator(f => new Sale(
            saleNumber: $"{f.Random.Number(1, 1000)}-{f.Random.Number(1001, 999999999)}",
            DateTime.Now,
            GenerateValidCustomer(),
            GenerateValidBranch()
        ));

    public static Sale GenerateValidSale()
    {
        return SaleFake.Generate();
    }

    public static Sale GenerateValidSaleWithTwoItems()
    {
        var sale = SaleFake.Generate();
        var product1Id = Guid.NewGuid();
        var product2Id = Guid.NewGuid();
        var item1 = SaleItemTestData.GenerateValidProduct(product1Id);
        var item2 = SaleItemTestData.GenerateValidProduct(product2Id);
        sale.AddItem(item1, 11, 158.9M);
        sale.AddItem(item2, 2, 22.99M);
        return sale;
    }

    public static (Sale sale, Dictionary<Guid, int> productQuantities) GenerateValidSaleMultipleDiscountTiers()
    {
        var productQuantities = new Dictionary<Guid, int>();

        var sale = GenerateValidSale();

        void AddSaleItem(Guid productId, int quantity, decimal price)
        {
            var product = SaleItemTestData.GenerateValidProduct(productId);
            sale.AddItem(product, quantity, price);
            productQuantities[productId] = sale.Items.FirstOrDefault(i => i.ProductId == productId)!.Quantity;
        }

        var product1 = Guid.NewGuid();
        var product2 = Guid.NewGuid();
        var product3 = Guid.NewGuid();
        var product4 = Guid.NewGuid();
        var product5 = Guid.NewGuid();
        var product6 = Guid.NewGuid();

        AddSaleItem(product1, 2, 10);
        AddSaleItem(product2, 3, 20);
        AddSaleItem(product3, 2, 15);
        AddSaleItem(product4, 6, 25);
        AddSaleItem(product5, 10, 30);
        AddSaleItem(product6, 15, 50);
        AddSaleItem(product3, 2, 15);

        return (sale, productQuantities);
    }

    public static Customer GenerateValidCustomer()
    {
        return new Customer(Guid.NewGuid(), "Thiago Borges");
    }

    public static Customer GenerateInvalidCustomer()
    {
        return new Customer(Guid.Empty, "Foo Bar");
    }

    public static Branch GenerateValidBranch()
    {
        return new Branch(Guid.NewGuid(), "Filial do Thiago", "Fica em São Paulo mesmo");
    }

    public static Branch GenerateInvalidBranch()
    {
        return new Branch(Guid.Empty, "Branch Foo Bar", "Address Branch Foo Bar");
    }
}
