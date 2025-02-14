using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Serilog;
using Xunit;
using Serilog.Sinks.InMemory;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="CancelSaleHandler"/> class.
/// </summary>
public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly CancelSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelSaleHandlerTests"/> class.
    /// Sets up the test dependencies.
    /// </summary>
    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _eventDispatcher = Substitute.For<IEventDispatcher>();
        _handler = new CancelSaleHandler(_saleRepository, _eventDispatcher);
    }

    /// <summary>
    /// Tests valid sale ID When cancelling sale Then returns success response
    /// </summary>
    [Fact(DisplayName = "Given valid sale ID When cancelling sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var command = new CancelSaleCommand { Id = Guid.NewGuid() };
        _saleRepository.CancelAsync(command.Id, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.Cancelled.Should().BeTrue();
        await _saleRepository.Received(1).CancelAsync(command.Id, Arg.Any<CancellationToken>());
        _eventDispatcher.Received(1).Publish(Arg.Any<SaleCancelledEvent>());
    }

    /// <summary>
    /// Tests invalid sale data When cancelling sale Then throws validation exception
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When cancelling sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = new CancelSaleCommand { Id = Guid.Empty };

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests non-existent sale ID When cancelling sale Then returns cancelled as false
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale ID When cancelling sale Then returns cancelled as false")]
    public async Task Handle_NonExistentSale_ReturnsCancelledFalse()
    {
        // Arrange
        var command = new CancelSaleCommand { Id = Guid.NewGuid() };
        _saleRepository.CancelAsync(command.Id, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.Cancelled.Should().BeFalse();
        await _saleRepository.Received(1).CancelAsync(command.Id, Arg.Any<CancellationToken>());
        _eventDispatcher.DidNotReceive().Publish(Arg.Any<SaleCancelledEvent>());
    }

    /// <summary>
    /// Tests valid sale ID When cancelling sale Then publishes SaleCancelledEvent
    /// </summary>
    [Fact(DisplayName = "Given valid sale ID When cancelling sale Then publishes SaleCancelledEvent")]
    public async Task Handle_ValidRequest_PublishesSaleCancelledEvent()
    {
        // Arrange
        var command = new CancelSaleCommand { Id = Guid.NewGuid() };
        _saleRepository.CancelAsync(command.Id, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _eventDispatcher.Received(1).Publish(Arg.Is<SaleCancelledEvent>(e => e.SaleId == command.Id));
    }

    /// <summary>
    /// Tests valid sale ID When cancelling sale fails Then does not publish SaleCancelledEvent
    /// </summary>
    [Fact(DisplayName = "Given valid sale ID When cancelling sale fails Then does not publish SaleCancelledEvent")]
    public async Task Handle_CancellationFails_DoesNotPublishEvent()
    {
        // Arrange
        var command = new CancelSaleCommand { Id = Guid.NewGuid() };
        _saleRepository.CancelAsync(command.Id, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _eventDispatcher.DidNotReceive().Publish(Arg.Any<SaleCancelledEvent>());
    }

    /// <summary>
    /// Tests valid sale ID When cancelling sale Then logs success message
    /// </summary>
    /// 
    /// 
    /// Esse vou testar a string de retorno, seria bom adicionar em uma constante, ou em cenários de i18n
    [Fact(DisplayName = "Given valid sale ID When cancelling sale Then logs success message")]
    public async Task Handle_ValidRequest_LogsSuccessMessage()
    {
        // Arrange
        var inMemorySink = new InMemorySink();
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Sink(inMemorySink)
            .CreateLogger();

        var command = new CancelSaleCommand { Id = Guid.NewGuid() };
        _saleRepository.CancelAsync(command.Id, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var logEntries = inMemorySink.LogEvents;
        var logMessage = logEntries.SingleOrDefault(entry => entry.MessageTemplate.Text.Contains("Successfully cancelled sale with ID"));

        Assert.NotNull(logMessage);
        Assert.Contains(command.Id.ToString(), logMessage.RenderMessage());
    }

    /// <summary>
    /// Tests invalid sale data When cancelling sale Then logs validation errors
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When cancelling sale Then logs validation errors")]
    public async Task Handle_InvalidRequest_LogsValidationErrors()
    {
        // Arrange
        var command = new CancelSaleCommand { Id = Guid.Empty };

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}