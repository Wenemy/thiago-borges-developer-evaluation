using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

//Os testes foram feitos abrangendo os cenários de desconto propostos
//Cenários como "Data da venda não pode ser menor que hoje" não foram implementados


/// <summary>
/// Contains unit tests for the Sale entity class.
/// Tests cover status changes and validation scenarios.
/// </summary>
public class SaleTests
{
    /// <summary>
    /// Tests that validation fails when the sale contains more than 20 identical items.
    /// </summary>
    [Fact(DisplayName = "Validation should fail for sale with 20 or more identical items")]
    public void Given_SaleDataWith20OrMoreIdenticalItem_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = Guid.NewGuid();
        var item = SaleItemTestData.GenerateValidSaleItem(productId, 21, 10);

        sale.AddItem(item.Product, item.Quantity, item.UnitPrice);

        // Act
        var result = sale.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Detail == "The quantity cannot be greater than 20.");
    }

    /// <summary>
    /// Tests that validation passes for a sale with less than 4 identical items, applying no discount.
    /// </summary>
    [Fact(DisplayName = "Validation should pass for 0% discount in sale with below 4 identical items")]
    public void Given_SaleDataWithItemsCountLessThan4_When_Validated_Then_ShouldReturn0PercentDiscount()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = Guid.NewGuid();
        var item = SaleItemTestData.GenerateValidSaleItem(productId, 3, 10);

        sale.AddItem(item.Product, item.Quantity, item.UnitPrice);

        // Act
        decimal expectedTotal = item.Quantity * item.UnitPrice;
        decimal actualTotal = sale.TotalAmount;

        // Assert
        Assert.Equal(expectedTotal, actualTotal);
    }

    /// <summary>
    /// Tests that validation applies a 10% discount for sales with between 4 and 9 identical items.
    /// </summary>
    [Fact(DisplayName = "Validation should pass for 10% discount in sale between 4 and 9 identical items")]
    public void Given_SaleDataWithItemsCountBetween4And9_When_Validated_Then_ShouldReturn10PercentDiscount()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = Guid.NewGuid();
        var item = SaleItemTestData.GenerateValidSaleItem(productId, 5, 10);

        sale.AddItem(item.Product, item.Quantity, item.UnitPrice);

        // Act
        decimal expectedTotal = (item.Quantity * item.UnitPrice) * 0.9m;
        decimal actualTotal = sale.TotalAmount;

        // Assert
        Assert.Equal(expectedTotal, actualTotal);
    }

    /// <summary>
    /// Tests that validation applies a 20% discount for sales with between 10 and 20 identical items.
    /// </summary>
    [Fact(DisplayName = "Validation should pass for 20% discount in sale between 10 and 20 identical items")]
    public void Given_SaleDataWithItemsCountBetween10And20_When_Validated_Then_ShouldReturn20PercentDiscount()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = Guid.NewGuid();
        var item = SaleItemTestData.GenerateValidSaleItem(productId, 15, 10);

        sale.AddItem(item.Product, item.Quantity, item.UnitPrice);

        // Act
        decimal expectedTotal = (item.Quantity * item.UnitPrice) * 0.8m;
        decimal actualTotal = sale.TotalAmount;

        // Assert
        Assert.Equal(expectedTotal, actualTotal);
    }

    /// <summary>
    /// Tests that validation fails when there are 20 or more identical items added in multiple loops.
    /// </summary>
    [Fact(DisplayName = "Validation should fail for sale with 20 or more identical items in different loop")]
    public void Given_SaleDataWith20OrMoreIdenticalItemInDifferentLoop_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = Guid.NewGuid();
        var item = SaleItemTestData.GenerateValidSaleItem(productId, 15, 10);

        sale.AddItem(item.Product, item.Quantity, item.UnitPrice);
        sale.AddItem(item.Product, 15, item.UnitPrice);

        // Act
        var result = sale.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Detail == "The quantity cannot be greater than 20.");
    }

    /// <summary>
    /// Tests that validation applies the correct discounts based on quantity tiers in the sale.
    /// </summary>
    [Fact(DisplayName = "Validation should apply correct discounts based on quantity tiers")]
    public void Given_SaleWithDifferentQuantities_When_Validated_Then_ShouldApplyCorrectDiscounts()
    {
        // Arrange
        var (sale, productQuantities) = SaleTestData.GenerateValidSaleMultipleDiscountTiers();

        // Act
        var result = sale.Validate();

        // Assert
        Assert.True(result.IsValid);

        foreach (var item in sale.Items)
        {
            var expectedQuantity = productQuantities[item.Product.ProductId];

            if (expectedQuantity < 4)
                Assert.Equal(0, item.Discount);
            else if (expectedQuantity >= 4 && expectedQuantity < 10)
                Assert.Equal(item.UnitPrice * expectedQuantity * 0.10m, item.Discount);
            else if (expectedQuantity >= 10 && expectedQuantity <= 20)
                Assert.Equal(item.UnitPrice * expectedQuantity * 0.20m, item.Discount);
        }
    }

    /// <summary>
    /// Tests that multiple items can be added to the sale correctly.
    /// </summary>
    [Fact(DisplayName = "AddItems should add multiple items to sale")]
    public void Given_MultipleSaleItems_When_Added_Then_ShouldAddAllItemsToSale()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var items = new List<SaleItem>
        {
            SaleItemTestData.GenerateValidSaleItem(Guid.NewGuid(), 5, 10),
            SaleItemTestData.GenerateValidSaleItem(Guid.NewGuid(), 3, 20)
        };

        // Act
        sale.AddItems(items);

        // Assert
        Assert.Equal(2, sale.Items.Count);
        Assert.Contains(sale.Items, i => i.Product.ProductId == items[0].Product.ProductId && i.Quantity == 5);
        Assert.Contains(sale.Items, i => i.Product.ProductId == items[1].Product.ProductId && i.Quantity == 3);
    }

    /// <summary>
    /// Tests that removing an existing item from the sale updates the sale correctly.
    /// </summary>
    [Fact(DisplayName = "RemoveItem should remove an existing item from sale")]
    public void Given_SaleWithItems_When_ItemRemoved_Then_ShouldRemoveItemFromSale()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var product1 = SaleItemTestData.GenerateValidProduct(Guid.NewGuid());
        sale.AddItem(product1, 5, 10);
        sale.AddItem(product1, 3, 10);

        // Act
        sale.RemoveItem(product1.ProductId);

        // Assert
        Assert.Empty(sale.Items);
    }

    /// <summary>
    /// Tests that removing an item that does not exist in the sale does not affect the sale.
    /// </summary>
    [Fact(DisplayName = "RemoveItem should not alter sale if item does not exist")]
    public void Given_SaleWithoutItem_When_ItemRemoved_Then_ShouldNotAlterSale()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var nonExistentProductId = Guid.NewGuid();

        // Act
        sale.RemoveItem(nonExistentProductId);

        // Assert
        Assert.Empty(sale.Items);
    }

    /// <summary>
    /// Tests that canceling the sale marks the sale as cancelled.
    /// </summary>
    [Fact(DisplayName = "Cancel should mark the sale as cancelled")]
    public void Given_Sale_When_CancelCalled_Then_ShouldMarkSaleAsCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.Cancel();

        // Assert
        Assert.True(sale.IsCancelled);
    }

    /// <summary>
    /// Tests that canceling the sale does not alter other attributes of the sale.
    /// </summary>
    [Fact(DisplayName = "Cancel should not affect other attributes of sale")]
    public void Given_Sale_When_CancelCalled_Then_ShouldNotAlterOtherAttributes()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var originalSaleNumber = sale.SaleNumber;
        var originalSaleDate = sale.SaleDate;

        // Act
        sale.Cancel();

        // Assert
        Assert.Equal(originalSaleNumber, sale.SaleNumber);
        Assert.Equal(originalSaleDate, sale.SaleDate);
    }

    /// <summary>
    /// Tests that TotalAmount calculates correctly for a sale with multiple items and discounts.
    /// </summary>
    [Fact(DisplayName = "TotalAmount should return correct total for sale with multiple items")]
    public void Given_SaleWithMultipleItems_When_TotalAmountCalculated_Then_ShouldReturnCorrectAmount()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var product1 = SaleItemTestData.GenerateValidProduct(Guid.NewGuid());
        var product2 = SaleItemTestData.GenerateValidProduct(Guid.NewGuid());

        sale.AddItem(product1, 5, 10); // 10% de desconto (5 * 10) = 50 - 10% = 45
        sale.AddItem(product2, 3, 20); // sem desconto (3 * 20) = 60
                                       // 105

        // Act
        var totalAmount = sale.TotalAmount;

        // Assert
        Assert.Equal(105, totalAmount);
    }

    /// <summary>
    /// Tests that TotalAmount returns 0 for a sale without items.
    /// </summary>
    [Fact(DisplayName = "TotalAmount should return 0 for sale without items")]
    public void Given_SaleWithoutItems_When_TotalAmountCalculated_Then_ShouldReturnZero()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        var totalAmount = sale.TotalAmount;

        // Assert
        Assert.Equal(0, totalAmount);
    }

    /// <summary>
    /// Tests that TotalAmount is updated correctly when items are added or removed from the sale.
    /// </summary>
    [Fact(DisplayName = "TotalAmount should update correctly when items are added or removed")]
    public void Given_Sale_When_ItemsAddedOrRemoved_Then_ShouldUpdateTotalAmount()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var product1 = SaleItemTestData.GenerateValidProduct(Guid.NewGuid());

        // Act
        sale.AddItem(product1, 3, 10);

        // Assert
        Assert.Equal(30, sale.TotalAmount); // Trintão.. não tem desconto

        // Act
        sale.AddItem(product1, 5, 10);

        // Assert
        Assert.Equal(72, sale.TotalAmount); // Entrou na cota dos 10%

        // Act
        sale.RemoveItem(product1.ProductId);

        // Assert
        Assert.Equal(0, sale.TotalAmount);
    }
}
