using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using ToyStore.Shared.Models;
using ToyStore.UserService.DTOs;

namespace ToyStore.UserService.Controllers;

/// <summary>
/// User management controller for profile and account operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[SwaggerTag("User profile management, addresses, preferences, and account operations")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly UserDbContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserDbContext context, ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <returns>User profile information</returns>
    /// <response code="200">Profile retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("profile")]
    [SwaggerOperation(
        Summary = "Get user profile",
        Description = "Retrieves the profile information for the currently authenticated user.",
        OperationId = "GetUserProfile",
        Tags = new[] { "Profile" })]
    [SwaggerResponse(200, "Profile retrieved successfully", typeof(ApiResponse<UserProfileDto>))]
    [SwaggerResponse(401, "Unauthorized", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "User not found", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = GetUserId();
            var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("User not found", 404));
            }

            var profileDto = new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                CreatedAt = user.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                ProfileImageUrl = "",
                PreferredLanguage = "en",
                TimeZone = "UTC"
            };

            return Ok(ApiResponse<UserProfileDto>.SuccessResult(profileDto, "Profile retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile");
            return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving profile", 500));
        }
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    /// <param name="request">Updated profile information</param>
    /// <returns>Updated profile</returns>
    /// <response code="200">Profile updated successfully</response>
    /// <response code="400">Invalid profile data</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPut("profile")]
    [SwaggerOperation(
        Summary = "Update user profile",
        Description = "Updates the profile information for the currently authenticated user.",
        OperationId = "UpdateUserProfile",
        Tags = new[] { "Profile" })]
    [SwaggerResponse(200, "Profile updated successfully", typeof(ApiResponse<UserProfileDto>))]
    [SwaggerResponse(400, "Invalid profile data", typeof(ApiResponse<object>))]
    [SwaggerResponse(401, "Unauthorized", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "User not found", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> UpdateProfile([FromBody, SwaggerRequestBody("Updated profile data", Required = true)] UpdateUserProfileDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("Invalid profile data", 400));
            }

            var userId = GetUserId();
            var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("User not found", 404));
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.Phone = request.Phone;
            user.DateOfBirth = request.DateOfBirth;

            await _context.SaveChangesAsync();

            var profileDto = new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                CreatedAt = user.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                ProfileImageUrl = request.ProfileImageUrl,
                PreferredLanguage = request.PreferredLanguage,
                TimeZone = request.TimeZone
            };

            return Ok(ApiResponse<UserProfileDto>.SuccessResult(profileDto, "Profile updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            return StatusCode(500, ApiResponse<object>.ErrorResult("Error updating profile", 500));
        }
    }

    /// <summary>
    /// Get user addresses
    /// </summary>
    /// <returns>List of user addresses</returns>
    /// <response code="200">Addresses retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("addresses")]
    [SwaggerOperation(
        Summary = "Get user addresses",
        Description = "Retrieves all addresses for the currently authenticated user.",
        OperationId = "GetUserAddresses",
        Tags = new[] { "Addresses" })]
    [SwaggerResponse(200, "Addresses retrieved successfully", typeof(ApiResponse<List<UserAddressDto>>))]
    [SwaggerResponse(401, "Unauthorized", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetAddresses()
    {
        try
        {
            var userId = GetUserId();
            var addresses = await _context.UserAddresses
                .Where(a => a.UserId == userId && a.IsActive)
                .OrderByDescending(a => a.IsDefault)
                .ThenBy(a => a.Title)
                .ToListAsync();

            var addressDtos = addresses.Select(a => new UserAddressDto
            {
                Id = a.Id,
                UserId = a.UserId,
                Title = a.Title,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Company = "",
                AddressLine1 = a.Address,
                AddressLine2 = "",
                City = a.City,
                State = a.State,
                ZipCode = a.ZipCode,
                Country = a.Country,
                Phone = a.Phone,
                IsDefault = a.IsDefault,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            return Ok(ApiResponse<List<UserAddressDto>>.SuccessResult(addressDtos, "Addresses retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving addresses");
            return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving addresses", 500));
        }
    }

    /// <summary>
    /// Create new address
    /// </summary>
    /// <param name="request">Address creation data</param>
    /// <returns>Created address</returns>
    /// <response code="201">Address created successfully</response>
    /// <response code="400">Invalid address data</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("addresses")]
    [SwaggerOperation(
        Summary = "Create new address",
        Description = "Creates a new address for the currently authenticated user.",
        OperationId = "CreateUserAddress",
        Tags = new[] { "Addresses" })]
    [SwaggerResponse(201, "Address created successfully", typeof(ApiResponse<UserAddressDto>))]
    [SwaggerResponse(400, "Invalid address data", typeof(ApiResponse<object>))]
    [SwaggerResponse(401, "Unauthorized", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> CreateAddress([FromBody, SwaggerRequestBody("Address creation data", Required = true)] CreateUserAddressDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("Invalid address data", 400));
            }

            var userId = GetUserId();

            // If this is set as default, update existing default addresses
            if (request.IsDefault)
            {
                var existingDefaults = await _context.UserAddresses
                    .Where(a => a.UserId == userId && a.IsDefault)
                    .ToListAsync();

                foreach (var addr in existingDefaults)
                {
                    addr.IsDefault = false;
                }
            }

            var address = new UserAddress
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Title = request.Title,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address = request.AddressLine1,
                City = request.City,
                State = request.State,
                ZipCode = request.ZipCode,
                Country = request.Country,
                Phone = request.Phone,
                IsDefault = request.IsDefault,
                IsActive = true
            };

            _context.UserAddresses.Add(address);
            await _context.SaveChangesAsync();

            var addressDto = new UserAddressDto
            {
                Id = address.Id,
                UserId = address.UserId,
                Title = address.Title,
                FirstName = address.FirstName,
                LastName = address.LastName,
                Company = "",
                AddressLine1 = address.Address,
                AddressLine2 = "",
                City = address.City,
                State = address.State,
                ZipCode = address.ZipCode,
                Country = address.Country,
                Phone = address.Phone,
                IsDefault = address.IsDefault,
                IsActive = address.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return CreatedAtAction(nameof(GetAddresses), new { }, ApiResponse<UserAddressDto>.SuccessResult(addressDto, "Address created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating address");
            return StatusCode(500, ApiResponse<object>.ErrorResult("Error creating address", 500));
        }
    }

    /// <summary>
    /// Update address
    /// </summary>
    /// <param name="id">Address ID</param>
    /// <param name="request">Updated address data</param>
    /// <returns>Updated address</returns>
    /// <response code="200">Address updated successfully</response>
    /// <response code="400">Invalid address data</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Address not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPut("addresses/{id}")]
    [SwaggerOperation(
        Summary = "Update address",
        Description = "Updates an existing address for the currently authenticated user.",
        OperationId = "UpdateUserAddress",
        Tags = new[] { "Addresses" })]
    [SwaggerResponse(200, "Address updated successfully", typeof(ApiResponse<UserAddressDto>))]
    [SwaggerResponse(400, "Invalid address data", typeof(ApiResponse<object>))]
    [SwaggerResponse(401, "Unauthorized", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "Address not found", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> UpdateAddress(
        [SwaggerParameter("Address ID", Required = true)] string id,
        [FromBody, SwaggerRequestBody("Updated address data", Required = true)] UpdateUserAddressDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("Invalid address data", 400));
            }

            var userId = GetUserId();
            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (address == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("Address not found", 404));
            }

            // If this is set as default, update existing default addresses
            if (request.IsDefault && !address.IsDefault)
            {
                var existingDefaults = await _context.UserAddresses
                    .Where(a => a.UserId == userId && a.IsDefault && a.Id != id)
                    .ToListAsync();

                foreach (var addr in existingDefaults)
                {
                    addr.IsDefault = false;
                }
            }

            address.Title = request.Title;
            address.FirstName = request.FirstName;
            address.LastName = request.LastName;
            address.Address = request.AddressLine1;
            address.City = request.City;
            address.State = request.State;
            address.ZipCode = request.ZipCode;
            address.Country = request.Country;
            address.Phone = request.Phone;
            address.IsDefault = request.IsDefault;
            address.IsActive = request.IsActive;

            await _context.SaveChangesAsync();

            var addressDto = new UserAddressDto
            {
                Id = address.Id,
                UserId = address.UserId,
                Title = address.Title,
                FirstName = address.FirstName,
                LastName = address.LastName,
                Company = "",
                AddressLine1 = address.Address,
                AddressLine2 = "",
                City = address.City,
                State = address.State,
                ZipCode = address.ZipCode,
                Country = address.Country,
                Phone = address.Phone,
                IsDefault = address.IsDefault,
                IsActive = address.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return Ok(ApiResponse<UserAddressDto>.SuccessResult(addressDto, "Address updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating address");
            return StatusCode(500, ApiResponse<object>.ErrorResult("Error updating address", 500));
        }
    }

    /// <summary>
    /// Delete address
    /// </summary>
    /// <param name="id">Address ID</param>
    /// <returns>Deletion result</returns>
    /// <response code="204">Address deleted successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Address not found</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete("addresses/{id}")]
    [SwaggerOperation(
        Summary = "Delete address",
        Description = "Deletes an address for the currently authenticated user.",
        OperationId = "DeleteUserAddress",
        Tags = new[] { "Addresses" })]
    [SwaggerResponse(204, "Address deleted successfully")]
    [SwaggerResponse(401, "Unauthorized", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "Address not found", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> DeleteAddress([SwaggerParameter("Address ID", Required = true)] string id)
    {
        try
        {
            var userId = GetUserId();
            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (address == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("Address not found", 404));
            }

            address.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting address");
            return StatusCode(500, ApiResponse<object>.ErrorResult("Error deleting address", 500));
        }
    }

    /// <summary>
    /// Get user statistics
    /// </summary>
    /// <returns>User statistics and metrics</returns>
    /// <response code="200">Statistics retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("stats")]
    [SwaggerOperation(
        Summary = "Get user statistics",
        Description = "Retrieves statistics and metrics for the currently authenticated user.",
        OperationId = "GetUserStats",
        Tags = new[] { "Statistics" })]
    [SwaggerResponse(200, "Statistics retrieved successfully", typeof(ApiResponse<UserStatsDto>))]
    [SwaggerResponse(401, "Unauthorized", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var userId = GetUserId();
            var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("User not found", 404));
            }

            // Mock statistics - in real implementation, you'd query order service, etc.
            var stats = new UserStatsDto
            {
                UserId = userId,
                TotalOrders = 5,
                TotalSpent = 1250.75m,
                WishlistItems = 3,
                ReviewsWritten = 8,
                LastOrderDate = DateTime.UtcNow.AddDays(-7),
                JoinDate = user.CreatedAt,
                MembershipLevel = "Silver",
                LoyaltyPoints = 125
            };

            return Ok(ApiResponse<UserStatsDto>.SuccessResult(stats, "Statistics retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user statistics");
            return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving statistics", 500));
        }
    }

    private string GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
    }
}

// Extended UserAddress model to match the existing one in Program.cs
public class UserAddress
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
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
    public bool IsActive { get; set; } = true;
}
