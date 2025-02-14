using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSalesQuery;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// Controller for managing sales operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<SalesController> _logger;

    /// <summary>
    /// Initializes a new instance of SalesController
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public SalesController(IMediator mediator, IMapper mapper, ILogger<SalesController> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new sale
    /// </summary>
    /// <param name="request">The sale creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to create sale: {@Request}", request);

        var validator = new CreateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for CreateSale: {@Errors}", validationResult.Errors);
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var command = _mapper.Map<CreateSaleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Sale created successfully: {@Response}", response);

            return Created(string.Empty, new ApiResponseWithData<CreateSaleResponse>
            {
                Success = true,
                Message = "Sale created successfully",
                Data = _mapper.Map<CreateSaleResponse>(response)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating sale");
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }

    /// <summary>
    /// Updates a sale
    /// </summary>
    /// <param name="request">The sale creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateSaleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateSale([FromRoute] Guid id, [FromBody] UpdateSaleRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to update sale with ID {Id}: {@Request}", id, request);

        request.SetId(id);
        var validator = new UpdateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for UpdateSale: {@Errors}", validationResult.Errors);
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var command = _mapper.Map<UpdateSaleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Sale updated successfully: {@Response}", response);

            return Created(string.Empty, new ApiResponseWithData<UpdateSaleResponse>
            {
                Success = true,
                Message = "Sale updated successfully",
                Data = _mapper.Map<UpdateSaleResponse>(response)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating sale {Id}", id);
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }

    /// <summary>
    /// Cancel sale by id
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale status details if found</returns>
    [HttpPatch("{id}/cancel")]
    [ProducesResponseType(typeof(ApiResponseWithData<CancelSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to cancel sale with ID {Id}", id);

        var request = new CancelSaleRequest { Id = id };
        var validator = new CancelSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for CancelSale: {@Errors}", validationResult.Errors);
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var command = _mapper.Map<CancelSaleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            if (!response.Cancelled)
            {
                _logger.LogWarning("Sale {Id} not found for cancellation", id);
                return NotFound();
            }

            _logger.LogInformation("Sale {Id} cancelled successfully", id);

            return Ok(new ApiResponseWithData<CancelSaleResponse>
            {
                Success = true,
                Message = "Sale cancelled successfully",
                Data = _mapper.Map<CancelSaleResponse>(response)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while cancelling sale {Id}", id);
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }

    /// <summary>
    /// Delete sale by id
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale status details if found</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<DeleteSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to delete sale with ID {Id}", id);

        var request = new DeleteSaleRequest { Id = id };
        var validator = new DeleteSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for DeleteSale: {@Errors}", validationResult.Errors);
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var command = _mapper.Map<DeleteSaleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            if (!response.Deleted)
            {
                _logger.LogWarning("Sale {Id} not found for deletion", id);
                return NotFound();
            }

            _logger.LogInformation("Sale {Id} deleted successfully", id);

            return Ok(new ApiResponseWithData<DeleteSaleResponse>
            {
                Success = true,
                Message = "Sale deleted successfully",
                Data = _mapper.Map<DeleteSaleResponse>(response)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting sale {Id}", id);
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }

    /// <summary>
    /// Retrieves a sale by their ID
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale details if found</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to get sale with ID {Id}", id);

        var request = new GetSaleRequest { Id = id };
        var validator = new GetSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for GetSale: {@Errors}", validationResult.Errors);
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var command = _mapper.Map<GetSaleCommand>(request.Id);
            var response = await _mediator.Send(command, cancellationToken);

            if (response == null)
            {
                _logger.LogWarning("Sale {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Sale {Id} retrieved successfully", id);
            return Ok(_mapper.Map<GetSaleResponse>(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving sale {Id}", id);
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }

    /// <summary>
    /// Retrieves a sale paginated
    /// </summary>
    /// <param name="query">Query to filter sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale details if found</returns>
    [HttpGet()]
    [ProducesResponseType(typeof(ApiResponseWithData<PagedResult<GetSaleResult>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPaginated([FromQuery] SaleQueryParams query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to get paginated sales: {@Query}", query);

        try
        {
            var result = await _mediator.Send(query, cancellationToken);
            _logger.LogInformation("Successfully retrieved paginated sales");

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving paginated sales");
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }
}
