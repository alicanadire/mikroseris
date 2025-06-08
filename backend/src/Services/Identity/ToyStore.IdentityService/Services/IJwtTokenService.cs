using System.Security.Claims;
using ToyStore.IdentityService.Models;

namespace ToyStore.IdentityService.Services;

public interface IJwtTokenService
{
    Task<string> GenerateTokenAsync(ApplicationUser user, IList<string> roles);
    ClaimsPrincipal? ValidateToken(string token);
}
