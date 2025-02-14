using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Serilog;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Handler for processing CancelSaleCommand requests
/// </summary>
public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of CancelSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="validator">The validator for CancelSaleCommand</param>
    /// <param name="eventDispatcher">The dispatcher of SaleCancelledEvent</param>
    public CancelSaleHandler(ISaleRepository saleRepository, IEventDispatcher eventDispatcher)
    {
        _saleRepository = saleRepository;
        _eventDispatcher = eventDispatcher;
    }

    /// <summary>
    /// Handles the CancelSaleCommand request
    /// </summary>
    /// <param name="command">The CancelSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>If sales was cancelled</returns>
    public async Task<CancelSaleResult> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            Log.Warning("Validation failed for CancelSaleCommand: {@Errors}", validationResult.Errors);
            throw new ValidationException(validationResult.Errors);
        }

        var cancelledSale = await _saleRepository.CancelAsync(command.Id, cancellationToken);

        if (cancelledSale)
        {
            _eventDispatcher.Publish(new SaleCancelledEvent(command.Id));
            Log.Information("Successfully cancelled sale with ID {SaleId}", command.Id);
        }

        return new CancelSaleResult() { Id = command.Id, Cancelled = cancelledSale };
    }
}
