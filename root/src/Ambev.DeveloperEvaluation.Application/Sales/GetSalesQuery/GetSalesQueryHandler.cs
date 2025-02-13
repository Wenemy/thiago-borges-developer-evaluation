using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSalesQuery
{
    public class GetSalesQueryHandler : IRequestHandler<SaleQueryParams, PagedResult<GetSaleResult>>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        public GetSalesQueryHandler(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<GetSaleResult>> Handle(SaleQueryParams query, CancellationToken cancellationToken)
        {
            var queryResponse = await _saleRepository.GetPagedSalesAsync(query.Page, query.PageSize, query.Filters, cancellationToken);
            var mapping = _mapper.Map<List<GetSaleResult>>(queryResponse.Items);
            return new PagedResult<GetSaleResult>(mapping, query.Page, query.PageSize, queryResponse.TotalPages);
        }
    }
}
