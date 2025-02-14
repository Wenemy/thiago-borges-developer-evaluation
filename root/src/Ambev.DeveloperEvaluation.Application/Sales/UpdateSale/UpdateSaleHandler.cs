using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Serilog;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleCommand requests
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of UpdateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="eventDispatcher">The dispatcher of SaleUpdatedEvent</param>
    /// <param name="validator">The validator for UpdateSaleCommand</param>
    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IEventDispatcher eventDispatcher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        Log.Information("Handling UpdateSaleCommand: {@Command}", command);
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            Log.Warning("Validation failed for UpdateSaleCommand: {@Errors}", validationResult.Errors);
            throw new ValidationException(validationResult.Errors);
        }

        var existingSale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingSale == null)
        {
            Log.Warning("Update failed: Sale with ID {SaleId} not found", command.Id);
            throw new InvalidOperationException($"Sale with ID {command.Id} not found");
        }

        var saleWithSameNumber = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (saleWithSameNumber != null && saleWithSameNumber.Id != command.Id)
        {
            Log.Warning("Update failed: Sale number {SaleNumber} already exists for Sale ID {ExistingSaleId}", command.SaleNumber, saleWithSameNumber.Id);
            throw new InvalidOperationException($"Sale with number {command.SaleNumber} already exists");
        }

        _mapper.Map(command, existingSale);

        var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        if (updatedSale != null)
            _eventDispatcher.Publish(new SaleUpdatedEvent(updatedSale));

        Log.Information("Successfully updated sale with ID {SaleId}", command.Id);
        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}
