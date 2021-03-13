using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Localization;
using AntDesign;
using LMS.Web.Data;

namespace LMS.Web.Components.User
{
    public partial class UserEditModal
    {
        [Parameter]
        public Shared.User User { get; set; }

        [Parameter]
        public EventCallback<Shared.User> UserChanged { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string ButtonText { get; set; }

        [Inject]
        private ModalService ModalService { get; set; }

        [Inject]
        private IStringLocalizer<UserEditModal> Localizer { get; set; }

        private ModalRef _modalRef;
        
        private bool _confirmLoading;

        private async Task OpenTemplate()
        {
            var user = User.JsonClone();

            var modalConfig = new ModalOptions
            {
                Centered = true,

                Title = this.Title,

                OkText = Localizer["Ok"].Value,

                CancelText = Localizer["Cancel"].Value,
                
                ConfirmLoading = _confirmLoading,

                OnOk = async (e) =>
                {
                    _confirmLoading = true;
                    string errorMessage = null;
                    try
                    {
                        var response = await AuthorizeHttpClient.PutAsJsonAsync($"/accounts/{user.Id}", user);
                        if (response.IsSuccessStatusCode)
                        {
                            await _modalRef.CloseAsync();
                            ModalService.Success(new ConfirmOptions
                            {
                                Centered = true,
                                Content = Localizer["This account has been successfully edited!"].Value
                            });
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
                        User = user;
                        await _modalRef.CloseAsync();
                        ModalService.Error(new ConfirmOptions
                        {
                            Centered = true,
                            Content = Localizer["Unknown error occurred: '{0}', please check your input and try again.", errorMessage].Value
                        });
                    }
                    else
                    {
                        await UserChanged.InvokeAsync(user);
                    }

                    _confirmLoading = false;
                    StateHasChanged();
                },

                OnCancel = async (e) =>
                {
                    User = user;
                    await _modalRef.CloseAsync();
                }
            };

            _modalRef = await ModalService.CreateModalAsync<UserEditModalForm, Shared.User>(modalConfig, User);
        }
    }
}
