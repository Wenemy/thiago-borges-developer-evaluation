using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Common.Pagination;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSalesQuery
{
    public class SaleQueryParams : IRequest<PagedResult<GetSaleResult>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
    }
}
