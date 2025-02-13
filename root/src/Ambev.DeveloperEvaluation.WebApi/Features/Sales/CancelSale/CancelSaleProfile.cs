using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale
{
    public class CancelSaleProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for CancelSale feature
        /// </summary>
        public CancelSaleProfile()
        {
            CreateMap<CancelSaleRequest, CancelSaleResponse>();
            CreateMap<CancelSaleResult, CancelSaleResponse>();
        }
    }
}
