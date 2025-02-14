using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Events
{
    public class EventDispatcherTests
    {
        private readonly EventDispatcher _eventDispatcher;

        public EventDispatcherTests()
        {
            var log = new LoggerConfiguration()
                .WriteTo.InMemory()
                .CreateLogger();

            Log.Logger = log;

            _eventDispatcher = new EventDispatcher();
        }

        [Fact]
        public void Publish_Event_Should_Log_Event_Info()
        {
            // Arrange
            var sale = SaleTestData.GenerateValidSaleWithTwoItems();
            var saleCreatedEvent = new SaleCreatedEvent(sale);

            // Act
            _eventDispatcher.Publish(saleCreatedEvent);

            // Assert
            var logEvents = InMemorySink.Instance.LogEvents;
            var logEntry = logEvents.FirstOrDefault();

            Assert.NotNull(logEntry);
            Assert.Equal(LogEventLevel.Information, logEntry.Level);
            Assert.Contains("📢 Evento Publicado", logEntry.RenderMessage());
        }
    }
}
