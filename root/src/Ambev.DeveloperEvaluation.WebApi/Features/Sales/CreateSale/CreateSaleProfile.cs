using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    public class CreateSaleProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for CreateSale feature
        /// </summary>
        public CreateSaleProfile()
        {
            CreateMap<CreateSaleRequest, CreateSaleResponse>();
            CreateMap<CreateSaleItemRequest, CreateSaleItemResponse>();
            CreateMap<CreateSaleResult, CreateSaleResponse>();
            CreateMap<CreateSaleItemResult, CreateSaleItemResponse>();
        }
    }
}
