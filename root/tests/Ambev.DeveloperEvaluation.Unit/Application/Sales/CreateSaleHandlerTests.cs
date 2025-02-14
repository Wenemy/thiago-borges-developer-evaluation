using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="CreateSaleHandler"/> class.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly CreateSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _eventDispatcher = Substitute.For<IEventDispatcher>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _eventDispatcher);
    }

    /// <summary>
    /// Tests that a valid sale creation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When creating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = new Sale(command.SaleNumber, command.SaleDate, new Customer(command.CustomerId, ""), new Branch(command.BranchId, "", ""));
        sale.AddItems(command.Items.Select(item => new SaleItem(sale.Id, new Product(item.ProductId, ""), item.Quantity, item.UnitPrice)));
        var result = new CreateSaleResult { Id = sale.Id };

        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(result);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>()).Returns((Sale?)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var createSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        createSaleResult.Should().NotBeNull();
        createSaleResult.Id.Should().Be(sale.Id);
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        _eventDispatcher.Received(1).Publish(Arg.Any<SaleCreatedEvent>());
    }

    /// <summary>
    /// Tests that an invalid sale creation request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When creating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CreateSaleCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    // <summary>
    // Tests that a duplicate sale number request throws an invalid operation exception.
    // </summary>
    [Fact(DisplayName = "Given duplicate sale number When creating sale Then throws invalid operation exception")]
    public async Task Handle_DuplicateSaleNumber_ThrowsInvalidOperationException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = new Sale(command.SaleNumber, command.SaleDate, new Customer(command.CustomerId, ""), new Branch(command.BranchId, "", ""));

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>()).Returns(existingSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Sale with number {command.SaleNumber} already exists");
    }

    /// <summary>
    /// Tests that the mapper is called with the correct command.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then maps command to sale entity")]
    public async Task Handle_ValidRequest_MapsCommandToSale()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = new Sale(command.SaleNumber, command.SaleDate, new Customer(command.CustomerId, ""), new Branch(command.BranchId, "", ""));

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<Sale>(Arg.Is<CreateSaleCommand>(c =>
            c.SaleNumber == command.SaleNumber));
    }

    /// <summary>
    /// Tests that an event is published after a successful sale creation.
    /// </summary>
    [Fact(DisplayName = "Given successful sale creation When handling Then publishes SaleCreatedEvent")]
    public async Task Handle_SuccessfulCreation_PublishesSaleCreatedEvent()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = new Sale(command.SaleNumber, command.SaleDate, new Customer(command.CustomerId, ""), new Branch(command.BranchId, "", ""));

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _eventDispatcher.Received(1).Publish(Arg.Any<SaleCreatedEvent>());
    }
}
