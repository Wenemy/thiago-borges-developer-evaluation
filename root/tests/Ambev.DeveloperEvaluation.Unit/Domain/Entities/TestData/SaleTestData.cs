using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

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
