using System;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Localization;
using AntDesign;
using LMS.Shared;

namespace LMS.Web.Data
{
    public class PageComponent : BaseComponent
    {
        [CascadingParameter]
        private MainLayout MainLayout { get; set; }

        [Inject]
        private IStringLocalizer<PageComponent> Localizer { get; set; }

        [Inject]
        private MessageService Message { get; set; }
        
        protected User CurrentUser { get; private set; }
        
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
                    await Message.Error(Localizer["Unknown error occurred: '{0}', please refresh the page or try again later.", e.Message].Value);
                    return null;
                }

                if (user == null)
                {
                    await Message.Error(Localizer["An error occurred: 'Unable to obtain user information', please refresh the page or try again later."].Value);
                    return null;
                }
            }
            
            return user;
        }
    }
}
