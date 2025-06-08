using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using ToyStore.IdentityService.Models;
using ToyStore.IdentityService.Services;
using ToyStore.Shared.Models;

namespace ToyStore.IdentityService.Controllers;

/// <summary>
/// Account management controller for user authentication and registration
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[SwaggerTag("User account management including registration, login, and profile operations")]
[Produces("application/json")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="request">User registration information</param>
    /// <returns>Registration result</returns>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">Invalid registration data</response>
    /// <response code="409">Email already exists</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("register")]
    [SwaggerOperation(
        Summary = "Register new user",
        Description = "Creates a new user account with email and password validation.",
        OperationId = "RegisterUser",
        Tags = new[] { "Account" })]
    [SwaggerResponse(200, "User registered successfully", typeof(ApiResponse<UserRegistrationResponse>))]
    [SwaggerResponse(400, "Invalid registration data", typeof(ApiResponse<object>))]
    [SwaggerResponse(409, "Email already exists", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> Register([FromBody, SwaggerRequestBody("User registration data", Required = true)] UserRegistrationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("Invalid registration data", 400));
            }

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return Conflict(ApiResponse<object>.ErrorResult("Email already registered", 409));
            }

            // Create new user
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = true, // For demo purposes
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(ApiResponse<object>.ErrorResult($"Registration failed: {errors}", 400));
            }

            // Add default role
            await _userManager.AddToRoleAsync(user, "Customer");

            // Add claims
            var claims = new List<Claim>
            {
                new Claim("given_name", user.FirstName),
                new Claim("family_name", user.LastName),
                new Claim("email", user.Email!),
                new Claim("role", "Customer")
            };
            await _userManager.AddClaimsAsync(user, claims);

            var response = new UserRegistrationResponse
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsEmailConfirmed = user.EmailConfirmed
            };

            _logger.LogInformation("User registered successfully: {Email}", user.Email);

            return Ok(ApiResponse<UserRegistrationResponse>.SuccessResult(response, "User registered successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for email: {Email}", request.Email);
            return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred during registration", 500));
        }
    }

    /// <summary>
    /// User login
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Login result</returns>
    /// <response code="200">Login successful</response>
    /// <response code="400">Invalid credentials</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "User login",
        Description = "Authenticates user credentials and returns login status.",
        OperationId = "LoginUser",
        Tags = new[] { "Account" })]
    [SwaggerResponse(200, "Login successful", typeof(ApiResponse<UserLoginResponse>))]
    [SwaggerResponse(400, "Invalid credentials", typeof(ApiResponse<object>))]
    [SwaggerResponse(401, "Unauthorized", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> Login([FromBody, SwaggerRequestBody("User login credentials", Required = true)] UserLoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("Invalid login data", 400));
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.IsActive)
            {
                return Unauthorized(ApiResponse<object>.ErrorResult("Invalid credentials", 401));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return Unauthorized(ApiResponse<object>.ErrorResult("Account is locked out", 401));
                }
                return Unauthorized(ApiResponse<object>.ErrorResult("Invalid credentials", 401));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = await _jwtTokenService.GenerateTokenAsync(user, roles);

            var response = new UserLoginResponse
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList(),
                Token = token,
                TokenExpires = DateTime.UtcNow.AddHours(1),
                LastLoginAt = DateTime.UtcNow
            };

            // Update last login
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("User logged in successfully: {Email}", user.Email);

            return Ok(ApiResponse<UserLoginResponse>.SuccessResult(response, "Login successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login for email: {Email}", request.Email);
            return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred during login", 500));
        }
    }

    /// <summary>
    /// User logout
    /// </summary>
    /// <returns>Logout result</returns>
    /// <response code="200">Logout successful</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("logout")]
    [Authorize]
    [SwaggerOperation(
        Summary = "User logout",
        Description = "Logs out the current user and invalidates the session.",
        OperationId = "LogoutUser",
        Tags = new[] { "Account" })]
    [SwaggerResponse(200, "Logout successful", typeof(ApiResponse<bool>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _signInManager.SignOutAsync();

            _logger.LogInformation("User logged out successfully");

            return Ok(ApiResponse<bool>.SuccessResult(true, "Logout successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user logout");
            return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred during logout", 500));
        }
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    /// <returns>Current user details</returns>
    /// <response code="200">User information retrieved</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("me")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Get current user",
        Description = "Retrieves information about the currently authenticated user.",
        OperationId = "GetCurrentUser",
        Tags = new[] { "Account" })]
    [SwaggerResponse(200, "User information retrieved", typeof(ApiResponse<CurrentUserResponse>))]
    [SwaggerResponse(401, "Unauthorized", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "User not found", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<object>.ErrorResult("User not authenticated", 401));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("User not found", 404));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            var response = new CurrentUserResponse
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList(),
                IsEmailConfirmed = user.EmailConfirmed,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive
            };

            return Ok(ApiResponse<CurrentUserResponse>.SuccessResult(response, "User information retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current user information");
            return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred retrieving user information", 500));
        }
    }

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="request">Password change request</param>
    /// <returns>Password change result</returns>
    /// <response code="200">Password changed successfully</response>
    /// <response code="400">Invalid password data</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("change-password")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Change password",
        Description = "Changes the password for the currently authenticated user.",
        OperationId = "ChangePassword",
        Tags = new[] { "Account" })]
    [SwaggerResponse(200, "Password changed successfully", typeof(ApiResponse<bool>))]
    [SwaggerResponse(400, "Invalid password data", typeof(ApiResponse<object>))]
    [SwaggerResponse(401, "Unauthorized", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> ChangePassword([FromBody, SwaggerRequestBody("Password change data", Required = true)] ChangePasswordRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("Invalid password data", 400));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<object>.ErrorResult("User not authenticated", 401));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(ApiResponse<object>.ErrorResult("User not found", 401));
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(ApiResponse<object>.ErrorResult($"Password change failed: {errors}", 400));
            }

            _logger.LogInformation("Password changed successfully for user: {Email}", user.Email);

            return Ok(ApiResponse<bool>.SuccessResult(true, "Password changed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred changing password", 500));
        }
    }
}

// DTOs
public class UserRegistrationRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class UserRegistrationResponse
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsEmailConfirmed { get; set; }
}

public class UserLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}

public class UserLoginResponse
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public string Token { get; set; } = string.Empty;
    public DateTime TokenExpires { get; set; }
    public DateTime LastLoginAt { get; set; }
}

public class CurrentUserResponse
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool IsEmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
