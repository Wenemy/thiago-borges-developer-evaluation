using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale
{
    public class DeleteSaleProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for DeleteSale operation
        /// </summary>
        public DeleteSaleProfile()
        {
            CreateMap<DeleteSaleCommand, Sale>();
            CreateMap<Sale, DeleteSaleResult>();
        }
    }
}
