using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using ToyStore.IdentityService.Models;
using ToyStore.Shared.Models;

namespace ToyStore.IdentityService.Controllers;

/// <summary>
/// Role management controller for admin operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin")]
[SwaggerTag("Role management for user access control")]
[Produces("application/json")]
public class RolesController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RolesController> _logger;

    public RolesController(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger<RolesController> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Get all available roles
    /// </summary>
    /// <returns>List of roles</returns>
    /// <response code="200">Roles retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Forbidden</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all roles",
        Description = "Retrieves all available roles in the system.",
        OperationId = "GetRoles",
        Tags = new[] { "Roles" })]
    [SwaggerResponse(200, "Roles retrieved successfully", typeof(ApiResponse<List<RoleDto>>))]
    [SwaggerResponse(401, "Unauthorized", typeof(ApiResponse<object>))]
    [SwaggerResponse(403, "Forbidden", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetRoles()
    {
        try
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var roleDtos = roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name!,
                NormalizedName = r.NormalizedName!
            }).ToList();

            return Ok(ApiResponse<List<RoleDto>>.SuccessResult(roleDtos, "Roles retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles");
            return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred retrieving roles", 500));
        }
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    /// <param name="request">Role creation data</param>
    /// <returns>Created role</returns>
    /// <response code="201">Role created successfully</response>
    /// <response code="400">Invalid role data</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Forbidden</response>
    /// <response code="409">Role already exists</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create new role",
        Description = "Creates a new role in the system.",
        OperationId = "CreateRole",
        Tags = new[] { "Roles" })]
    [SwaggerResponse(201, "Role created successfully", typeof(ApiResponse<RoleDto>))]
    [SwaggerResponse(400, "Invalid role data", typeof(ApiResponse<object>))]
    [SwaggerResponse(401, "Unauthorized", typeof(ApiResponse<object>))]
    [SwaggerResponse(403, "Forbidden", typeof(ApiResponse<object>))]
    [SwaggerResponse(409, "Role already exists", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> CreateRole([FromBody, SwaggerRequestBody("Role creation data", Required = true)] CreateRoleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("Invalid role data", 400));
            }

            var existingRole = await _roleManager.FindByNameAsync(request.Name);
            if (existingRole != null)
            {
                return Conflict(ApiResponse<object>.ErrorResult("Role already exists", 409));
            }

            var role = new IdentityRole(request.Name);
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(ApiResponse<object>.ErrorResult($"Role creation failed: {errors}", 400));
            }

            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name!,
                NormalizedName = role.NormalizedName!
            };

            _logger.LogInformation("Role created successfully: {RoleName}", role.Name);

            return CreatedAtAction(nameof(GetRoles), new { }, ApiResponse<RoleDto>.SuccessResult(roleDto, "Role created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role: {RoleName}", request.Name);
            return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred creating role", 500));
        }
    }

    /// <summary>
    /// Assign role to user
    /// </summary>
    /// <param name="request">Role assignment data</param>
    /// <returns>Assignment result</returns>
    /// <response code="200">Role assigned successfully</response>
    /// <response code="400">Invalid assignment data</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Forbidden</response>
    /// <response code="404">User or role not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("assign")]
    [SwaggerOperation(
        Summary = "Assign role to user",
        Description = "Assigns a role to a specific user.",
        OperationId = "AssignRole",
        Tags = new[] { "Roles" })]
    [SwaggerResponse(200, "Role assigned successfully", typeof(ApiResponse<bool>))]
    [SwaggerResponse(400, "Invalid assignment data", typeof(ApiResponse<object>))]
    [SwaggerResponse(401, "Unauthorized", typeof(ApiResponse<object>))]
    [SwaggerResponse(403, "Forbidden", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "User or role not found", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> AssignRole([FromBody, SwaggerRequestBody("Role assignment data", Required = true)] AssignRoleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("Invalid assignment data", 400));
            }

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("User not found", 404));
            }

            var role = await _roleManager.FindByNameAsync(request.RoleName);
            if (role == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("Role not found", 404));
            }

            var result = await _userManager.AddToRoleAsync(user, request.RoleName);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(ApiResponse<object>.ErrorResult($"Role assignment failed: {errors}", 400));
            }

            _logger.LogInformation("Role {RoleName} assigned to user {Email}", request.RoleName, user.Email);

            return Ok(ApiResponse<bool>.SuccessResult(true, "Role assigned successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleName} to user {UserId}", request.RoleName, request.UserId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred assigning role", 500));
        }
    }

    /// <summary>
    /// Remove role from user
    /// </summary>
    /// <param name="request">Role removal data</param>
    /// <returns>Removal result</returns>
    /// <response code="200">Role removed successfully</response>
    /// <response code="400">Invalid removal data</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Forbidden</response>
    /// <response code="404">User or role not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("remove")]
    [SwaggerOperation(
        Summary = "Remove role from user",
        Description = "Removes a role from a specific user.",
        OperationId = "RemoveRole",
        Tags = new[] { "Roles" })]
    [SwaggerResponse(200, "Role removed successfully", typeof(ApiResponse<bool>))]
    [SwaggerResponse(400, "Invalid removal data", typeof(ApiResponse<object>))]
    [SwaggerResponse(401, "Unauthorized", typeof(ApiResponse<object>))]
    [SwaggerResponse(403, "Forbidden", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "User or role not found", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> RemoveRole([FromBody, SwaggerRequestBody("Role removal data", Required = true)] RemoveRoleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("Invalid removal data", 400));
            }

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("User not found", 404));
            }

            var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(ApiResponse<object>.ErrorResult($"Role removal failed: {errors}", 400));
            }

            _logger.LogInformation("Role {RoleName} removed from user {Email}", request.RoleName, user.Email);

            return Ok(ApiResponse<bool>.SuccessResult(true, "Role removed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleName} from user {UserId}", request.RoleName, request.UserId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred removing role", 500));
        }
    }
}

// DTOs
public class RoleDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
}

public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
}

public class AssignRoleRequest
{
    public string UserId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}

public class RemoveRoleRequest
{
    public string UserId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}
