namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale
{
    public class GetSaleItemResponse
    {
        public Guid ProductId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
