using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using LMS.Shared;

namespace LMS.Web.Data
{
    public class APIAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public APIAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigationManager) : base(provider, navigationManager)
        {
            ConfigureHandler(
                new[] { Config.API.Url }, 
                new[] { "openid", Config.API.Name });
        }
    }
}
