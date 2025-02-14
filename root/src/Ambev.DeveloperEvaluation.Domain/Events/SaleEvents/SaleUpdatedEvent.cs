using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events.SaleEvents
{
    public class SaleUpdatedEvent
    {
        public Sale Sale { get; set; }
        public SaleUpdatedEvent(Sale sale)
        {
            Sale = sale;
        }
    }
}
