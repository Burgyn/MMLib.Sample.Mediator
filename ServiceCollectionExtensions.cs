using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventPublisher(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.TryAddSingleton<IEventPublisher, EventPublisher>();
        services.TryAddSingleton<IEventPublisherStrategy, ForeachAwaitPublisherStrategy>();
        return services;
    }

    public static IServiceCollection AddDomainEventHandler<TEvent, THandler>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TEvent : IDomainEvent
        where THandler : class, IDomainEventHandler<TEvent>
    {
        services.Add(new ServiceDescriptor(typeof(IDomainEventHandler<TEvent>), typeof(THandler), lifetime));

        return services;
    }
}