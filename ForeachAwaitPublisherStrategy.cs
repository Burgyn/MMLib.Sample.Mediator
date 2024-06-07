public sealed class ForeachAwaitPublisherStrategy : IEventPublisherStrategy
{
    public async Task PublishAsync<TEvent>(
        IEnumerable<IDomainEventHandler<TEvent>> handlers,
        TEvent domainEvent,
        CancellationToken cancellationToken)
        where TEvent : IDomainEvent
    {
        foreach (var handler in handlers)
        {
            await handler.HandleAsync(domainEvent, cancellationToken);
        }
    }
}
