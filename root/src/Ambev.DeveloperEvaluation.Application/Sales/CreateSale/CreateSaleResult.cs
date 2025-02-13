namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleResult
    {
        public Guid Id { get; set; }
        public Guid BranchId { get; set; }
        public Guid CustomerId { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsCancelled { get; set; }
        public ICollection<CreateSaleItemResult> Items { get; set; }
    }
}
