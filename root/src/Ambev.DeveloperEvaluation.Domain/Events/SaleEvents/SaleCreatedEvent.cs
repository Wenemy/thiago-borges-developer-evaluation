using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events.SaleEvents
{
    public class SaleCreatedEvent
    {
        public Sale Sale { get; set; }
        public SaleCreatedEvent(Sale sale)
        {
            Sale = sale;
        }
    }
}
