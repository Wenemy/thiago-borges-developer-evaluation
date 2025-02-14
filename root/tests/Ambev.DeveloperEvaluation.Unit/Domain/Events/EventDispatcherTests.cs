using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Events
{
    public class EventDispatcherTests
    {
        private readonly AntiSingletonLogger _logger;
        public EventDispatcherTests()
        {
            _logger = new AntiSingletonLogger();
        }

        [Fact]
        public void Publish_Event_Should_Log_Event_Info()
        {
            // Arrange
            var sale = SaleTestData.GenerateValidSaleWithTwoItems();
            var saleCreatedEvent = new SaleCreatedEvent(sale);


            var eventDispatcher = new EventDispatcher();


            // Act
            eventDispatcher.Publish(saleCreatedEvent);

            // Assert
            var logEvents = _logger.Sink.LogEvents;
            var logEntry = logEvents.FirstOrDefault();

            Assert.NotNull(logEntry);
            Assert.Contains("📢 Evento Publicado", logEntry.RenderMessage());
        }
    }
}
