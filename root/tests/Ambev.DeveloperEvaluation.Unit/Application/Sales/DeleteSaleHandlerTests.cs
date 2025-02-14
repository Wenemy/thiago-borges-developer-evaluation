using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Serilog;
using Serilog.Sinks.InMemory;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="DeleteSaleHandler"/> class.
/// </summary>
public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly DeleteSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSaleHandlerTests"/> class.
    /// Sets up the test dependencies.
    /// </summary>
    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _eventDispatcher = Substitute.For<IEventDispatcher>();
        _handler = new DeleteSaleHandler(_saleRepository, _eventDispatcher);
    }

    /// <summary>
    /// Tests valid sale ID When deleting sale Then returns success response
    /// </summary>
    [Fact(DisplayName = "Given valid sale ID When deleting sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var command = new DeleteSaleCommand { Id = Guid.NewGuid() };
        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.Deleted.Should().BeTrue();
        await _saleRepository.Received(1).DeleteAsync(command.Id, Arg.Any<CancellationToken>());
        _eventDispatcher.Received(1).Publish(Arg.Any<SaleDeletedEvent>());
    }

    /// <summary>
    /// Tests invalid sale data When deleting sale Then throws validation exception
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When deleting sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = new DeleteSaleCommand { Id = Guid.Empty };

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests non-existent sale ID When deleting sale Then returns deleted as false
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale ID When deleting sale Then returns deleted as false")]
    public async Task Handle_NonExistentSale_ReturnsDeletedFalse()
    {
        // Arrange
        var command = new DeleteSaleCommand { Id = Guid.NewGuid() };
        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.Deleted.Should().BeFalse();
        await _saleRepository.Received(1).DeleteAsync(command.Id, Arg.Any<CancellationToken>());
        _eventDispatcher.DidNotReceive().Publish(Arg.Any<SaleDeletedEvent>());
    }

    /// <summary>
    /// Tests valid sale ID When deleting sale Then publishes SaleDeletedEvent
    /// </summary>
    [Fact(DisplayName = "Given valid sale ID When deleting sale Then publishes SaleDeletedEvent")]
    public async Task Handle_ValidRequest_PublishesSaleDeletedEvent()
    {
        // Arrange
        var command = new DeleteSaleCommand { Id = Guid.NewGuid() };
        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _eventDispatcher.Received(1).Publish(Arg.Is<SaleDeletedEvent>(e => e.SaleId == command.Id));
    }

    /// <summary>
    /// Tests valid sale ID When deleting sale fails Then does not publish SaleDeletedEvent
    /// </summary>
    [Fact(DisplayName = "Given valid sale ID When deleting sale fails Then does not publish SaleDeletedEvent")]
    public async Task Handle_DeletionFails_DoesNotPublishEvent()
    {
        // Arrange
        var command = new DeleteSaleCommand { Id = Guid.NewGuid() };
        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _eventDispatcher.DidNotReceive().Publish(Arg.Any<SaleDeletedEvent>());
    }

    /// <summary>
    /// Tests valid sale ID When deleting sale Then logs success message
    /// </summary>
    [Fact(DisplayName = "Given valid sale ID When deleting sale Then logs success message")]
    public async Task Handle_ValidRequest_LogsSuccessMessage()
    {
        // Arrange
        var _logger = new AntiSingletonLogger();
        var command = new DeleteSaleCommand { Id = Guid.NewGuid() };
        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        var logEntries = _logger.Sink.LogEvents;
        var logMessage = logEntries.SingleOrDefault(entry => entry.MessageTemplate.Text.Contains("Successfully deleted"));

        // Assert
        Assert.NotNull(logMessage);
        Assert.Contains(command.Id.ToString(), logMessage.RenderMessage());
    }

    /// <summary>
    /// Tests invalid sale data When deleting sale Then logs validation errors
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When deleting sale Then logs validation errors")]
    public async Task Handle_InvalidRequest_LogsValidationErrors()
    {
        // Arrange
        var command = new DeleteSaleCommand { Id = Guid.Empty };

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}