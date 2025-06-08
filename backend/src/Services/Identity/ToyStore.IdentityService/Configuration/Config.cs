using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace ToyStore.IdentityService.Configuration;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource
            {
                Name = "roles",
                UserClaims = new List<string> {"role"}
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("toystore.products", "ToyStore Products API"),
            new ApiScope("toystore.orders", "ToyStore Orders API"),
            new ApiScope("toystore.users", "ToyStore Users API"),
            new ApiScope("toystore.inventory", "ToyStore Inventory API"),
            new ApiScope("toystore.notifications", "ToyStore Notifications API"),
            new ApiScope("toystore.admin", "ToyStore Admin API")
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("toystore.api", "ToyStore API")
            {
                Scopes = new List<string>
                {
                    "toystore.products",
                    "toystore.orders",
                    "toystore.users",
                    "toystore.inventory",
                    "toystore.notifications",
                    "toystore.admin"
                },
                UserClaims = new List<string> {"role"}
            }
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // React SPA Client
            new Client
            {
                ClientId = "toystore-spa",
                ClientName = "ToyStore SPA",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RequirePkce = true,
                AllowOfflineAccess = true,

                RedirectUris =
                {
                    "http://localhost:3000/auth/callback",
                    "http://localhost:5173/auth/callback"
                },

                PostLogoutRedirectUris =
                {
                    "http://localhost:3000",
                    "http://localhost:5173"
                },

                AllowedCorsOrigins =
                {
                    "http://localhost:3000",
                    "http://localhost:5173"
                },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    "toystore.products",
                    "toystore.orders",
                    "toystore.users",
                    "toystore.inventory",
                    "toystore.notifications"
                },

                AccessTokenLifetime = 3600,
                RefreshTokenUsage = TokenUsage.ReUse
            },

            // Admin Dashboard Client
            new Client
            {
                ClientId = "toystore-admin",
                ClientName = "ToyStore Admin Dashboard",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RequirePkce = true,
                AllowOfflineAccess = true,

                RedirectUris =
                {
                    "http://localhost:3001/auth/callback"
                },

                PostLogoutRedirectUris =
                {
                    "http://localhost:3001"
                },

                AllowedCorsOrigins =
                {
                    "http://localhost:3001"
                },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    "toystore.admin",
                    "toystore.products",
                    "toystore.orders",
                    "toystore.users",
                    "toystore.inventory",
                    "toystore.notifications"
                },

                AccessTokenLifetime = 3600,
                RefreshTokenUsage = TokenUsage.ReUse
            },

            // Machine to Machine Client for microservices
            new Client
            {
                ClientId = "toystore-m2m",
                ClientName = "ToyStore Machine to Machine",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("toystore-secret".Sha256()) },

                AllowedScopes =
                {
                    "toystore.products",
                    "toystore.orders",
                    "toystore.users",
                    "toystore.inventory",
                    "toystore.notifications",
                    "toystore.admin"
                }
            }
        };
}
