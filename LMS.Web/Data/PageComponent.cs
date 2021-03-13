using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using AntDesign;
using LMS.Shared;

namespace LMS.Web.Data
{
    public class PageComponent : BaseComponent
    {
        [CascadingParameter]
        private MainLayout MainLayout { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected MessageService Message { get; set; }

        [Inject]
        private IStringLocalizer<PageComponent> Localizer { get; set; }

        protected User CurrentUser { get; private set; }

        protected static string AddQueryString(string url, IDictionary<string, string> query) =>
            QueryHelpers.AddQueryString(url, query);

        protected static IDictionary<string, string> GetQueryString(string queryString) => 
            QueryHelpers.ParseQuery(queryString)
                .ToDictionary(k => k.Key, v => v.Value.ToString());

        protected static IDictionary<string, string> GetQueryString(NavigationManager navigationManager) =>
            GetQueryString(navigationManager.ToAbsoluteUri(navigationManager.Uri).Query);

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            CurrentUser = await GetCurrentUser();

            if (MainLayout.ShouldUpdateMenuData)
            {
                MainLayout.UpdateMenuData(CurrentUser);
            }
        }

        private async Task<User> GetCurrentUser()
        {
            User user = null;

            var identity = (await AuthenticationStateTask).User.Identity;
            if (identity?.IsAuthenticated ?? false)
            {
                var userId = (identity as ClaimsIdentity)?.FindFirst("sub")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return null;

                try
                {
                    user = await AuthorizeHttpClient.GetFromJsonAsync<User>($"/accounts/{userId}");
                    StateHasChanged();
                }
                catch (AccessTokenNotAvailableException e)
                {
                    e.Redirect();
                }
                catch (Exception e)
                {
                    Message.Error(Localizer["Unknown error occurred: '{0}', please refresh the page or try again later.", e.Message].Value);
                    return null;
                }

                if (user == null)
                {
                    Message.Error(Localizer["An error occurred: 'Unable to obtain user information', please refresh the page or try again later."].Value);
                    return null;
                }
            }
            
            return user;
        }
    }
}
