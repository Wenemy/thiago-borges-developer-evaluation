namespace Ambev.DeveloperEvaluation.Domain.Events.SaleEvents
{
    public class SaleCancelledEvent
    {
        public Guid SaleId { get; set; }
        public SaleCancelledEvent(Guid saleId)
        {
            SaleId = saleId;
        }
    }
}
