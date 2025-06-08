using System.Text.Json.Serialization;

namespace ToyStore.EventBus.Events;

public abstract class IntegrationEvent
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }

    [JsonConstructor]
    public IntegrationEvent(Guid id, DateTime createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
    }

    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}

// Product Events
public class ProductCreatedEvent : IntegrationEvent
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string CategoryId { get; set; } = string.Empty;
}

public class ProductUpdatedEvent : IntegrationEvent
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}

public class ProductDeletedEvent : IntegrationEvent
{
    public Guid ProductId { get; set; }
}

// Order Events
public class OrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public string CustomerEmail { get; set; } = string.Empty;
}

public class OrderConfirmedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}

public class OrderCancelledEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public string Reason { get; set; } = string.Empty;
}

// Inventory Events
public class StockUpdatedEvent : IntegrationEvent
{
    public Guid ProductId { get; set; }
    public int NewQuantity { get; set; }
    public int PreviousQuantity { get; set; }
    public string UpdateReason { get; set; } = string.Empty;
}

public class StockReservedEvent : IntegrationEvent
{
    public Guid ProductId { get; set; }
    public int ReservedQuantity { get; set; }
    public Guid OrderId { get; set; }
}

public class StockReleasedEvent : IntegrationEvent
{
    public Guid ProductId { get; set; }
    public int ReleasedQuantity { get; set; }
    public Guid OrderId { get; set; }
}

// User Events
public class UserRegisteredEvent : IntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class UserUpdatedEvent : IntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

// DTOs
public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
