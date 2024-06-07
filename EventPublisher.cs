internal class EventPublisher : IEventPublisher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventPublisherStrategy _publisherStrategy;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EventPublisher(
        IServiceProvider serviceProvider,
        IHttpContextAccessor httpContextAccessor,
        IEventPublisherStrategy publisherStrategy)
    {
        _serviceProvider = serviceProvider;
        _publisherStrategy = publisherStrategy;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        IEnumerable<IDomainEventHandler<TEvent>> handlers;
        if (_httpContextAccessor.HttpContext is not null)
        {
            handlers = GetHandlers<TEvent>(_httpContextAccessor.HttpContext.RequestServices);
            await _publisherStrategy.PublishAsync(handlers, domainEvent, cancellationToken);
        }
        else
        {
            using var scope = _serviceProvider.CreateScope();
            handlers = GetHandlers<TEvent>(scope.ServiceProvider);
            await _publisherStrategy.PublishAsync(handlers, domainEvent, cancellationToken);
        }
    }

    private static IEnumerable<IDomainEventHandler<TEvent>> GetHandlers<TEvent>(IServiceProvider serviceProvider)
        where TEvent : IDomainEvent
        => serviceProvider.GetServices<IDomainEventHandler<TEvent>>();
}
