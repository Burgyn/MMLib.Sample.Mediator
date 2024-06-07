var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEventPublisher();
builder.Services.AddDomainEventHandler<ProductCreated, ProductCreatedHandler>();

var app = builder.Build();

app.MapGet("/products", () => new[]
{
    new Product(1, "Keyboard", 20),
    new Product(2, "Mouse", 10),
    new Product(3, "Monitor", 200)
});

app.MapPost("/products", async (Product product, IEventPublisher publisher) =>
{
    // Save the product
    await publisher.PublishAsync(new ProductCreated(product.Id, product.Name, product.Price));
    return product;
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

public record Product(int Id, string Name, decimal Price);

public record ProductCreated(int Id, string Name, decimal Price) : IDomainEvent;

public class ProductCreatedHandler : IDomainEventHandler<ProductCreated>
{
    public Task HandleAsync(ProductCreated domainEvent, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Product created: {domainEvent.Name}");
        return Task.CompletedTask;
    }
}