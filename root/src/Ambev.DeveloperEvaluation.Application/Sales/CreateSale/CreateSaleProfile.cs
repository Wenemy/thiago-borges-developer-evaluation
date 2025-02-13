using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for CreateSale operation
        /// </summary>
        public CreateSaleProfile()
        {
            CreateMap<CreateSaleCommand, Sale>()
                .AfterMap((command, sale) =>
                {
                    sale.ClearItems();
                    foreach (var itemCommand in command.Items)
                    {
                        var product = new Domain.ValueObjects.Product(itemCommand.ProductId, "Após consultar o External Identity de Product");
                        sale.AddItem(product, itemCommand.Quantity, itemCommand.UnitPrice);
                    }
                });
            CreateMap<CreateSaleItemCommand, SaleItem>();
            CreateMap<Sale, CreateSaleResult>();
            CreateMap<SaleItem, CreateSaleItemResult>();
        }
    }
}
