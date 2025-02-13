using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;
using FluentValidation.TestHelper;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

/// <summary>
/// Contains unit tests for the SaleValidator class.
/// Tests cover validation of all sale properties
/// </summary>
public class SaleValidatorTests
{
    private readonly SaleValidator _validator;

    public SaleValidatorTests()
    {
        _validator = new SaleValidator();
    }

    /// <summary>
    /// Validation should fail if sale number is empty
    /// </summary>
    [Fact(DisplayName = "Validation should fail if sale number is empty")]
    public void Given_SaleWithEmptySaleNumber_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var customer = SaleTestData.GenerateValidCustomer();
        var branch = SaleTestData.GenerateValidBranch();
        var sale = new Sale("", DateTime.Now, customer, branch);

        // Act
        var result = _validator.Validate(sale);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "SaleNumber");
    }

    /// <summary>
    /// Validation should fail if customer ID is empty
    /// </summary>
    [Fact(DisplayName = "Validation should fail if customer ID is empty")]
    public void Given_SaleWithEmptyCustomerId_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var customer = SaleTestData.GenerateValidCustomer();
        customer = null;
        var branch = SaleTestData.GenerateValidBranch();
        var sale = new Sale("1234", DateTime.Now, customer!, branch);

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        Assert.False(result.IsValid);
        result.ShouldHaveValidationErrorFor(x => x.CustomerId);
    }

    /// <summary>
    /// Validation should fail if branch ID is empty
    /// </summary>
    [Fact(DisplayName = "Validation should fail if branch ID is empty")]
    public void Given_SaleWithEmptyBranchId_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var customer = SaleTestData.GenerateValidCustomer();
        var branch = SaleTestData.GenerateValidBranch();
        branch = null;
        var sale = new Sale("fa9we84f19a8w4e1f", DateTime.Now, customer, branch!);

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        Assert.False(result.IsValid);
        result.ShouldHaveValidationErrorFor(x => x.BranchId);
    }

    /// <summary>
    /// Validation should fail if any sale item is invalid
    /// </summary>
    [Fact(DisplayName = "Validation should fail if any sale item is invalid")]
    public void Given_SaleWithInvalidItems_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var customer = SaleTestData.GenerateValidCustomer();
        var branch = SaleTestData.GenerateValidBranch();
        var sale = new Sale("1234", DateTime.Now, customer, branch);
        sale.AddItem(new Product(Guid.Empty, "Invalid Product"), -1, -10);

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    /// <summary>
    /// Validation should pass for sale with valid data
    /// </summary>
    [Fact(DisplayName = "Validation should pass for sale with valid data")]
    public void Given_SaleWithValidData_When_Validated_Then_ShouldReturnValid()
    {
        // Arrange
        var customer = SaleTestData.GenerateValidCustomer();
        var branch = SaleTestData.GenerateValidBranch();
        var sale = new Sale("2345 meia 78 Tá na hora de molhar o biscoito", DateTime.Now, customer, branch);
        sale.AddItem(new Product(Guid.NewGuid(), "Product Name"), 1, 10);

        // Act
        var result = _validator.Validate(sale);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Validation should pass for sale with multiple valid items
    /// </summary>
    [Fact(DisplayName = "Validation should pass for sale with multiple valid items")]
    public void Given_SaleWithMultipleValidItems_When_Validated_Then_ShouldReturnValid()
    {
        // Arrange
        var customer = SaleTestData.GenerateValidCustomer();
        var branch = SaleTestData.GenerateValidBranch();
        var sale = new Sale("2345 meia 78 Tá na hora de molhar o biscoito", DateTime.Now, customer, branch);
        sale.AddItem(new Product(Guid.NewGuid(), "Produto A"), 2, 10);
        sale.AddItem(new Product(Guid.NewGuid(), "Produto B"), 3, 15);

        // Act
        var result = _validator.Validate(sale);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
