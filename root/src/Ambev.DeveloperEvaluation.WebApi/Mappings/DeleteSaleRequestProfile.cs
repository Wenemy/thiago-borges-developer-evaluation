using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings
{
    public class DeleteSaleRequestProfile : Profile
    {
        public DeleteSaleRequestProfile()
        {
            CreateMap<DeleteSaleRequest, DeleteSaleCommand>();
        }
    }
}
