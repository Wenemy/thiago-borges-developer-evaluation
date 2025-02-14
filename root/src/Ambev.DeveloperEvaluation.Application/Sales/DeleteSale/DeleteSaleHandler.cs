using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Serilog;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Handler for processing DeleteSaleCommand requests
/// </summary>
public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of CancelSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="validator">The validator for DeleteSaleCommand</param>
    /// <param name="eventDispatcher">The dispatcher of SaleDeletedEvent</param>
    public DeleteSaleHandler(ISaleRepository saleRepository, IEventDispatcher eventDispatcher)
    {
        _saleRepository = saleRepository;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<DeleteSaleResult> Handle(DeleteSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            Log.Warning("Validation failed for DeleteSaleCommand: {@Errors}", validationResult.Errors);
            throw new ValidationException(validationResult.Errors);
        }

        var deletedSale = await _saleRepository.DeleteAsync(command.Id, cancellationToken);

        if (deletedSale)
        {
            _eventDispatcher.Publish(new SaleDeletedEvent(command.Id));
            Log.Information("Successfully deleted sale with ID {SaleId}", command.Id);
        }
           
        return new DeleteSaleResult() { Id = command.Id, Deleted = deletedSale };
    }
}
