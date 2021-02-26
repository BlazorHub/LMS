using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using LMS.Shared;

namespace LMS.IdP
{
    public static class IdentityServerConfig
    {
        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
        {
            new IdentityResources.OpenId()
        };

        public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
        {
            new (Config.API.Name, Config.API.DisplayName)
        };

        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new (Config.API.Name, Config.API.DisplayName)
            {
                Enabled = true,
                ApiSecrets = { new Secret(Config.API.Secret.Sha256()) },
                Scopes = { Config.API.Name }
            }
        };

        public static IEnumerable<Client> Clients => new Client[]
        {
            // Web
            new ()
            {
                Enabled = true,

                ClientId = Config.Web.Name,

                ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,

                //ClientSecrets = {},

                //RequireClientSecret = true,

                ClientName = Config.Web.DisplayName,

                //Description = ,

                ClientUri = Config.Web.Url,

                //LogoUri = ,

                RequireConsent = false,

                AllowRememberConsent = true,

                AllowedGrantTypes = GrantTypes.Implicit,

                //RequirePkce = true,

                //AllowPlainTextPkce = false,

                //RequireRequestObject = false,

                AllowAccessTokensViaBrowser = true,

                RedirectUris =
                {
                    $"{Config.Web.Url}/authentication/login-callback"
                },

                PostLogoutRedirectUris =
                {
                    $"{Config.Web.Url}/authentication/logout-callback"
                },

                //FrontChannelLogoutUri = ,

                //FrontChannelLogoutSessionRequired = true,

                //BackChannelLogoutUri = ,

                //BackChannelLogoutSessionRequired = true,

                AllowOfflineAccess = false,

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    Config.API.Name
                },

                AlwaysIncludeUserClaimsInIdToken = false,

                IdentityTokenLifetime = 300,

                //AllowedIdentityTokenSigningAlgorithms = {},

                AccessTokenLifetime = 300,

                //AuthorizationCodeLifetime = 300,

                AbsoluteRefreshTokenLifetime = 2592000,

                SlidingRefreshTokenLifetime = 1296000,

                ConsentLifetime = null,

                RefreshTokenUsage = TokenUsage.OneTimeOnly,

                UpdateAccessTokenClaimsOnRefresh = true,

                RefreshTokenExpiration = TokenExpiration.Absolute,

                AccessTokenType = AccessTokenType.Reference,

                EnableLocalLogin = true,

                //IdentityProviderRestrictions = {},

                IncludeJwtId = true,

                //Claims = {},

                AlwaysSendClientClaims = false,

                ClientClaimsPrefix = "client_",

                //PairWiseSubjectSalt = ,

                //UserSsoLifetime = ,

                //UserCodeType = ,

                //DeviceCodeLifetime = 300,

                AllowedCorsOrigins =
                {
                    Config.Web.Url
                },

                //Properties = new Dictionary<string, string>(),
            }
        };
    }
}
