using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Serilog;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of CreateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="eventDispatcher">The dispatcher of SaleCreatedEvent</param>
    /// <param name="validator">The validator for CreateSaleCommand</param>
    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IEventDispatcher eventDispatcher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        Log.Information("Handling CreateSaleCommand: {@Command}", command);
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            Log.Warning("Validation failed for CreateSaleCommand: {@Errors}", validationResult.Errors);
            throw new ValidationException(validationResult.Errors);
        }

        var existingSale = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (existingSale != null)
        {
            Log.Warning("Create failed: Sale number {SaleNumber} already exists for Sale ID {ExistingSaleId}", command.SaleNumber, existingSale.Id);
            throw new InvalidOperationException($"Sale with number {command.SaleNumber} already exists");
        }
            
        var sale = _mapper.Map<Sale>(command);

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        if (createdSale != null)
            _eventDispatcher.Publish(new SaleCreatedEvent(createdSale));

        Log.Information("Successfully created sale with ID {SaleId}", createdSale?.Id);
        var result = _mapper.Map<CreateSaleResult>(createdSale);
        return result;
    }
}
