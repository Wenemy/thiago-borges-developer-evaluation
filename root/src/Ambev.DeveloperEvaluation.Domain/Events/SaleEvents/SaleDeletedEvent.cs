namespace Ambev.DeveloperEvaluation.Domain.Events.SaleEvents
{
    public class SaleDeletedEvent
    {
        public Guid SaleId { get; set; }
        public SaleDeletedEvent(Guid saleId)
        {
            SaleId = saleId;
        }
    }
}
