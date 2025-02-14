using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Domain;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="UpdateSaleHandler"/> class.
/// </summary>
public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly UpdateSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _eventDispatcher = Substitute.For<IEventDispatcher>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventDispatcher);
    }

    /// <summary>
    /// Tests valid sale data When updating sale Then returns success response
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When updating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = new Sale(command.Id, command.SaleNumber, command.SaleDate, new Customer(command.CustomerId, ""), new Branch(command.BranchId, "", ""));
        var updatedSale = existingSale;
        var result = new UpdateSaleResult { Id = updatedSale.Id };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(existingSale);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>()).Returns((Sale?)null);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(updatedSale);
        _mapper.Map<UpdateSaleResult>(updatedSale).Returns(result);

        var updateSaleResult = await _handler.Handle(command, CancellationToken.None);

        updateSaleResult.Should().NotBeNull();
        updateSaleResult.Id.Should().Be(updatedSale.Id);
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        _eventDispatcher.Received(1).Publish(Arg.Any<SaleUpdatedEvent>());
    }


    /// <summary>
    /// Tests invalid sale data When updating sale Then throws validation exception
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When updating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        var command = new UpdateSaleCommand();
        var act = () => _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ValidationException>();
    }


    /// <summary>
    /// Tests non-existent sale ID When updating sale Then throws invalid operation exception
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale ID When updating sale Then throws invalid operation exception")]
    public async Task Handle_NonExistentSale_ThrowsInvalidOperationException()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);
        var act = () => _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Sale with ID {command.Id} not found");
    }

    /// <summary>
    /// Tests duplicate sale number When updating sale Then throws invalid operation exception
    /// </summary>
    [Fact(DisplayName = "Given duplicate sale number When updating sale Then throws invalid operation exception")]
    public async Task Handle_DuplicateSaleNumber_ThrowsInvalidOperationException()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = new Sale(command.Id, command.SaleNumber, command.SaleDate, new Customer(command.CustomerId, ""), new Branch(command.BranchId, "", ""));
        var anotherSale = new Sale(Guid.NewGuid(), command.SaleNumber, command.SaleDate, new Customer(command.CustomerId, ""), new Branch(command.BranchId, "", ""));

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(existingSale);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>()).Returns(anotherSale);

        var act = () => _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Sale with number {command.SaleNumber} already exists");
    }

    /// <summary>
    /// Tests valid command When handling Then maps command to sale entity
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then maps command to sale entity")]
    public async Task Handle_ValidRequest_MapsCommandToSale()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = new Sale(command.Id, command.SaleNumber, command.SaleDate, new Customer(command.CustomerId, ""), new Branch(command.BranchId, "", ""));

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(existingSale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(existingSale);

        await _handler.Handle(command, CancellationToken.None);
        _mapper.Received(1).Map(Arg.Any<UpdateSaleCommand>(), existingSale);
    }

    /// <summary>
    /// Tests successful sale update When handling Then publishes SaleUpdatedEvent
    /// </summary>
    [Fact(DisplayName = "Given successful sale update When handling Then publishes SaleUpdatedEvent")]
    public async Task Handle_SuccessfulUpdate_PublishesSaleUpdatedEvent()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = new Sale(command.Id, command.SaleNumber, command.SaleDate, new Customer(command.CustomerId, ""), new Branch(command.BranchId, "", ""));

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(existingSale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(existingSale);

        await _handler.Handle(command, CancellationToken.None);
        _eventDispatcher.Received(1).Publish(Arg.Any<SaleUpdatedEvent>());
    }
}
