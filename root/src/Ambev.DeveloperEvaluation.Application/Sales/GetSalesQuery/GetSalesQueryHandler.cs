using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSalesQuery
{
    /// <summary>
    /// Handler for processing GetSalesQueryHandler requests
    /// </summary>
    public class GetSalesQueryHandler : IRequestHandler<SaleQueryParams, PagedResult<GetSaleResult>>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of GetSaleHandler
        /// </summary>
        /// <param name="saleRepository">The sale repository</param>
        /// <param name="mapper">The AutoMapper instance</param>
        /// <param name="validator">The validator for GetSaleCommand</param>
        public GetSalesQueryHandler(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the GetSalesQueryHandler request
        /// </summary>
        /// <param name="request">The GetSalesQueryHandler command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The sale details if found</returns>
        public async Task<PagedResult<GetSaleResult>> Handle(SaleQueryParams query, CancellationToken cancellationToken)
        {
            var queryResponse = await _saleRepository.GetPagedSalesAsync(query.Page, query.PageSize, query.Filters, cancellationToken);
            var mapping = _mapper.Map<List<GetSaleResult>>(queryResponse.Items);
            return new PagedResult<GetSaleResult>(mapping, query.Page, query.PageSize, queryResponse.TotalPages);
        }
    }
}
