using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToyStore.Shared.Models;

namespace ToyStore.UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserProfile>>> GetProfile()
    {
        try
        {
            // Mock user profile
            var profile = new UserProfile
            {
                Id = "1",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "+1234567890",
                DateOfBirth = new DateTime(1990, 1, 1),
                CreatedAt = DateTime.UtcNow.AddMonths(-6)
            };

            return Ok(new ApiResponse<UserProfile>
            {
                Success = true,
                Data = profile,
                Message = "Profile retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile");
            return StatusCode(500, new ApiResponse<UserProfile>
            {
                Success = false,
                Message = "Error retrieving profile",
                StatusCode = 500
            });
        }
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserProfile>>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            // Mock profile update
            var profile = new UserProfile
            {
                Id = "1",
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                DateOfBirth = request.DateOfBirth,
                CreatedAt = DateTime.UtcNow.AddMonths(-6)
            };

            return Ok(new ApiResponse<UserProfile>
            {
                Success = true,
                Data = profile,
                Message = "Profile updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            return StatusCode(500, new ApiResponse<UserProfile>
            {
                Success = false,
                Message = "Error updating profile",
                StatusCode = 500
            });
        }
    }

    [HttpGet("addresses")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<UserAddress>>>> GetAddresses()
    {
        try
        {
            var addresses = new List<UserAddress>
            {
                new UserAddress
                {
                    Id = "1",
                    Title = "Home",
                    FirstName = "John",
                    LastName = "Doe",
                    Address = "123 Main St",
                    City = "New York",
                    State = "NY",
                    ZipCode = "10001",
                    Country = "USA",
                    Phone = "+1234567890",
                    IsDefault = true
                }
            };

            return Ok(new ApiResponse<List<UserAddress>>
            {
                Success = true,
                Data = addresses,
                Message = "Addresses retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving addresses");
            return StatusCode(500, new ApiResponse<List<UserAddress>>
            {
                Success = false,
                Message = "Error retrieving addresses",
                StatusCode = 500
            });
        }
    }
}

public class UserProfile
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateProfileRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
}

public class UserAddress
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
