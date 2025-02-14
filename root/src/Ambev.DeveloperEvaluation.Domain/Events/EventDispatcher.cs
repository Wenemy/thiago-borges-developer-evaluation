using Serilog;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class EventDispatcher : IEventDispatcher
    {
        public void Publish<TEvent>(TEvent @event) where TEvent : class
        {
            Log.Information("📢 Evento Publicado: {EventType} | Dados: {@EventData}", typeof(TEvent).Name, @event);
        }
    }
}
