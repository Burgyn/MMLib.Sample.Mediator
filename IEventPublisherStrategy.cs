public interface IEventPublisherStrategy
{
    Task PublishAsync<TEvent>(
        IEnumerable<IDomainEventHandler<TEvent>> handlers,
        TEvent domainEvent,
        CancellationToken cancellationToken)
        where TEvent : IDomainEvent;
}
