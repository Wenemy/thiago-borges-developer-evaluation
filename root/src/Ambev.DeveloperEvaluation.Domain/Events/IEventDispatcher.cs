namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public interface IEventDispatcher
    {
        void Publish<TEvent>(TEvent @event) where TEvent : class;
    }
}
