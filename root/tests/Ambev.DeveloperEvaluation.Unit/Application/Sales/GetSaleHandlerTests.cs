using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="GetSaleHandler"/> class.
/// </summary>
public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSaleHandlerTests"/> class.
    /// Sets up the test dependencies.
    /// </summary>
    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests valid sale ID When getting sale Then returns sale details
    /// </summary>
    [Fact(DisplayName = "Given valid sale ID When getting sale Then returns sale details")]
    public async Task Handle_ValidRequest_ReturnsSaleDetails()
    {
        // Arrange
        var command = new GetSaleCommand(Guid.NewGuid());
        var sale = new Sale(command.Id, "Venda do Thiago", DateTime.UtcNow, new Customer(Guid.NewGuid(), "Thiago Borges"), new Branch(Guid.NewGuid(), "Filial São Paulo", "Rio de Janeiro"));
        var expectedResult = new GetSaleResult { Id = sale.Id, SaleNumber = sale.SaleNumber };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        result.SaleNumber.Should().Be(sale.SaleNumber);
        await _saleRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<GetSaleResult>(sale);
    }

    /// <summary>
    /// Tests invalid sale data When getting sale Then throws validation exception
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When getting sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = new GetSaleCommand(Guid.Empty);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests non-existent sale ID When getting sale Then throws key not found exception
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale ID When getting sale Then throws key not found exception")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = new GetSaleCommand(Guid.NewGuid());
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"Sale with ID {command.Id} not found");
    }

    /// <summary>
    /// Tests valid sale ID When getting sale Then maps sale entity to result
    /// </summary>
    [Fact(DisplayName = "Given valid sale ID When getting sale Then maps sale entity to result")]
    public async Task Handle_ValidRequest_MapsSaleToResult()
    {
        // Arrange
        var command = new GetSaleCommand(Guid.NewGuid());
        var sale = new Sale(command.Id, "Venda do Thiago", DateTime.UtcNow, new Customer(Guid.NewGuid(), "Thiago Borges"), new Branch(Guid.NewGuid(), "Rio de Janeiro", "Brasil-sil-sil"));
        var expectedResult = new GetSaleResult { Id = sale.Id, SaleNumber = sale.SaleNumber };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(expectedResult);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mapper.Received(1).Map<GetSaleResult>(sale);
    }
}