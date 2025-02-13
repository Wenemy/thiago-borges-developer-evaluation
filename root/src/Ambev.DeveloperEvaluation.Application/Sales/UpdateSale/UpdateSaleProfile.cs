using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for UpdateSale operation
        /// </summary>
        public UpdateSaleProfile()
        {
            CreateMap<UpdateSaleCommand, Sale>()
                .AfterMap((command, sale) =>
                {
                    sale.ClearItems();
                    foreach (var itemCommand in command.Items)
                    {
                        var product = new Domain.ValueObjects.Product(itemCommand.ProductId, "Após consultar o External Identity de Product");
                        sale.AddItem(product, itemCommand.Quantity, itemCommand.UnitPrice);
                    }
                });
            CreateMap<UpdateSaleItemCommand, SaleItem>();
            CreateMap<Sale, UpdateSaleResult>();
            CreateMap<SaleItem, UpdateSaleItemResult>();
        }
    }
}
