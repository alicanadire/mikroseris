using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ToyStore.Shared.Models;

namespace ToyStore.NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IMongoDatabase? _database;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(IMongoDatabase? database, ILogger<NotificationController> logger)
    {
        _database = database;
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<ActionResult<ApiResponse<bool>>> SendNotification([FromBody] SendNotificationRequest request)
    {
        try
        {
            var notification = new NotificationDocument
            {
                Id = Guid.NewGuid().ToString(),
                Type = request.Type,
                Recipient = request.Recipient,
                Subject = request.Subject,
                Message = request.Message,
                Status = "Sent",
                CreatedAt = DateTime.UtcNow,
                SentAt = DateTime.UtcNow
            };

            if (_database != null)
            {
                var collection = _database.GetCollection<NotificationDocument>("notifications");
                await collection.InsertOneAsync(notification);
            }

            _logger.LogInformation("Notification sent to {Recipient}: {Subject}", request.Recipient, request.Subject);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "Notification sent successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to {Recipient}", request.Recipient);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "Error sending notification",
                StatusCode = 500
            });
        }
    }

    [HttpGet("history/{userId}")]
    public async Task<ActionResult<ApiResponse<List<NotificationDto>>>> GetNotificationHistory(string userId)
    {
        try
        {
            var notifications = new List<NotificationDto>();

            if (_database != null)
            {
                var collection = _database.GetCollection<NotificationDocument>("notifications");
                var filter = Builders<NotificationDocument>.Filter.Eq(n => n.Recipient, userId);
                var docs = await collection.Find(filter)
                    .SortByDescending(n => n.CreatedAt)
                    .Limit(50)
                    .ToListAsync();

                notifications = docs.Select(doc => new NotificationDto
                {
                    Id = doc.Id,
                    Type = doc.Type,
                    Subject = doc.Subject,
                    Message = doc.Message,
                    Status = doc.Status,
                    CreatedAt = doc.CreatedAt,
                    SentAt = doc.SentAt
                }).ToList();
            }
            else
            {
                // Mock data when MongoDB is not available
                notifications = new List<NotificationDto>
                {
                    new NotificationDto
                    {
                        Id = "1",
                        Type = "Order",
                        Subject = "Order Confirmation",
                        Message = "Your order has been confirmed",
                        Status = "Sent",
                        CreatedAt = DateTime.UtcNow.AddHours(-2),
                        SentAt = DateTime.UtcNow.AddHours(-2)
                    }
                };
            }

            return Ok(new ApiResponse<List<NotificationDto>>
            {
                Success = true,
                Data = notifications,
                Message = "Notification history retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notification history for user {UserId}", userId);
            return StatusCode(500, new ApiResponse<List<NotificationDto>>
            {
                Success = false,
                Message = "Error retrieving notification history",
                StatusCode = 500
            });
        }
    }

    [HttpGet("templates")]
    public async Task<ActionResult<ApiResponse<List<NotificationTemplate>>>> GetTemplates()
    {
        try
        {
            var templates = new List<NotificationTemplate>
            {
                new NotificationTemplate
                {
                    Id = "order_confirmation",
                    Name = "Order Confirmation",
                    Subject = "Your Order is Confirmed - #{OrderNumber}",
                    Template = "Dear {CustomerName}, your order #{OrderNumber} has been confirmed. Total: ${Total}",
                    Type = "Email"
                },
                new NotificationTemplate
                {
                    Id = "shipping_notification",
                    Name = "Shipping Notification",
                    Subject = "Your Order is On the Way - #{OrderNumber}",
                    Template = "Your order #{OrderNumber} has been shipped. Tracking: {TrackingNumber}",
                    Type = "Email"
                },
                new NotificationTemplate
                {
                    Id = "welcome_email",
                    Name = "Welcome Email",
                    Subject = "Welcome to ToyStore!",
                    Template = "Welcome {CustomerName}! Thank you for joining ToyStore family.",
                    Type = "Email"
                }
            };

            return Ok(new ApiResponse<List<NotificationTemplate>>
            {
                Success = true,
                Data = templates,
                Message = "Templates retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notification templates");
            return StatusCode(500, new ApiResponse<List<NotificationTemplate>>
            {
                Success = false,
                Message = "Error retrieving templates",
                StatusCode = 500
            });
        }
    }
}

public class NotificationDocument
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
}

public class NotificationDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
}

public class SendNotificationRequest
{
    public string Type { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class NotificationTemplate
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Template { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
