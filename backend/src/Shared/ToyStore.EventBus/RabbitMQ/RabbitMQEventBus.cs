using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using ToyStore.EventBus.Abstractions;
using ToyStore.EventBus.Events;

namespace ToyStore.EventBus.RabbitMQ;

public class RabbitMQEventBus : IEventBus, IDisposable
{
    private readonly RabbitMQSettings _settings;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMQEventBus> _logger;
    private readonly Dictionary<string, List<Type>> _handlers;
    private readonly Dictionary<string, Type> _eventTypes;

    private IConnection? _connection;
    private IModel? _channel;
    private string? _queueName;

    public RabbitMQEventBus(
        IOptions<RabbitMQSettings> settings,
        IServiceProvider serviceProvider,
        ILogger<RabbitMQEventBus> logger)
    {
        _settings = settings.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _handlers = new Dictionary<string, List<Type>>();
        _eventTypes = new Dictionary<string, Type>();

        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_settings.ConnectionString),
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                exchange: _settings.ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            _queueName = $"{_settings.QueueName}_{Environment.MachineName}_{Guid.NewGuid()}";
            
            _channel.QueueDeclare(
                queue: _queueName,
                durable: false,
                exclusive: false,
                autoDelete: true);

            _logger.LogInformation("RabbitMQ connection established. Queue: {QueueName}", _queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing RabbitMQ connection");
            throw;
        }
    }

    public async Task PublishAsync<T>(T @event) where T : IntegrationEvent
    {
        if (_channel == null || _channel.IsClosed)
        {
            _logger.LogWarning("RabbitMQ channel is not available. Attempting to reconnect.");
            InitializeRabbitMQ();
        }

        var eventName = @event.GetType().Name;
        var message = JsonSerializer.Serialize(@event, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var body = Encoding.UTF8.GetBytes(message);

        var properties = _channel!.CreateBasicProperties();
        properties.Persistent = true;
        properties.DeliveryMode = 2;

        try
        {
            _channel.BasicPublish(
                exchange: _settings.ExchangeName,
                routingKey: eventName,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Published event {EventName} with ID {EventId}", eventName, @event.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing event {EventName} with ID {EventId}", eventName, @event.Id);
            throw;
        }

        await Task.CompletedTask;
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = GetEventKey<T>();
        var handlerType = typeof(TH);

        if (!_handlers.ContainsKey(eventName))
        {
            _handlers[eventName] = new List<Type>();
        }

        if (_handlers[eventName].Any(s => s == handlerType))
        {
            _logger.LogWarning("Handler {HandlerType} already registered for event {EventName}", handlerType.Name, eventName);
            return;
        }

        _handlers[eventName].Add(handlerType);
        _eventTypes[eventName] = typeof(T);

        StartBasicConsume();

        _logger.LogInformation("Subscribed to event {EventName} with handler {HandlerType}", eventName, handlerType.Name);
    }

    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = GetEventKey<T>();
        var handlerType = typeof(TH);

        if (_handlers.ContainsKey(eventName))
        {
            _handlers[eventName].Remove(handlerType);
            if (!_handlers[eventName].Any())
            {
                _handlers.Remove(eventName);
                _eventTypes.Remove(eventName);
            }
        }

        _logger.LogInformation("Unsubscribed from event {EventName} with handler {HandlerType}", eventName, handlerType.Name);
    }

    private void StartBasicConsume()
    {
        if (_channel == null) return;

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += OnMessageReceived;

        _channel.BasicConsume(
            queue: _queueName,
            autoAck: false,
            consumer: consumer);
    }

    private async Task OnMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;
        var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

        try
        {
            await ProcessEvent(eventName, message);
            _channel?.BasicAck(eventArgs.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event {EventName}: {Message}", eventName, message);
            _channel?.BasicNack(eventArgs.DeliveryTag, false, false);
        }
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (!_handlers.ContainsKey(eventName))
        {
            _logger.LogWarning("No handlers registered for event {EventName}", eventName);
            return;
        }

        var eventType = _eventTypes[eventName];
        var integrationEvent = JsonSerializer.Deserialize(message, eventType, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var handlers = _handlers[eventName];
        
        foreach (var handlerType in handlers)
        {
            var handler = _serviceProvider.GetService(handlerType);
            if (handler == null)
            {
                _logger.LogWarning("Handler {HandlerType} not found in service provider", handlerType.Name);
                continue;
            }

            var method = handlerType.GetMethod("Handle");
            if (method != null)
            {
                await (Task)method.Invoke(handler, new[] { integrationEvent })!;
            }
        }
    }

    private static string GetEventKey<T>() => typeof(T).Name;

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}

public class RabbitMQSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ExchangeName { get; set; } = "toystore_eventbus";
    public string QueueName { get; set; } = "toystore_queue";
}
