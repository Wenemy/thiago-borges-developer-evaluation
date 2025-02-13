namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale
{
    public class UpdateSaleItemResponse
    {
        public Guid ProductId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
