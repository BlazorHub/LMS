using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Localization;
using AntDesign;

namespace LMS.Web.Components.User
{
    public partial class UserDeleteModal
    {
        [Parameter]
        public string UserId { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string ButtonText { get; set; }

        [Parameter]
        public EventCallback AfterDelete { get; set; }
        
        [Inject]
        private ModalService ModalService { get; set; }

        [Inject]
        public IStringLocalizer<UserDeleteModal> Localizer { get; set; }

        private bool _visible;

        private bool _confirmLoading;

        private void ShowModal()
        {
            _visible = true;
        }

        private async Task HandleOk(MouseEventArgs e)
        {
            _confirmLoading = true;

            string errorMessage = null;
            try
            {
                var response = await AuthorizeHttpClient.DeleteAsync($"/accounts/{UserId}");
                if (response.IsSuccessStatusCode)
                {
                    _visible = false;
                    ModalService.Success(new ConfirmOptions
                    {
                        Centered = true,
                        Content = Localizer["This account has been successfully deleted!"].Value
                    });
                    await AfterDelete.InvokeAsync();
                }
                else
                {
                    errorMessage = await response.Content.ReadAsStringAsync();
                }
            }
            catch (AccessTokenNotAvailableException accessTokenNotAvailableException)
            {
                accessTokenNotAvailableException.Redirect();
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
            }

            if (errorMessage != null)
            {
                _visible = false;
                ModalService.Error(new ConfirmOptions
                {
                    Centered = true,
                    Content = Localizer["Unknown error occurred: '{0}', please check your input and try again.", errorMessage].Value
                });
            }

            _confirmLoading = false;
            StateHasChanged();
        }

        private void HandleCancel(MouseEventArgs e)
        {
            _visible = false;
        }
    }
}
