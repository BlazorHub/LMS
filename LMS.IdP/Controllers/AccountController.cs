using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using LMS.Shared;
using LMS.IdP.Models;
using LMS.IdP.Models.Account;
using LMS.IdP.Models.External;

namespace LMS.IdP.Controllers
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        
        public AccountController
        (
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
        }

        /// <summary>
        /// Entry point into the register workflow
        /// </summary>
        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            var vm = new RegisterViewModel
            {
                ReturnUrl = returnUrl
            };
            
            return View(vm);
        }

        /// <summary>
        /// Handle postback from register
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterInputModel model, string button)
        {
            //model.ReturnUrl = Url.Action("Login", !string.IsNullOrEmpty(model.ReturnUrl) ? new { returnUrl = model.ReturnUrl } : null);
            
            // the user clicked the "cancel" button
            if (button != "register")
                return this.LoadingPage("Redirect", !string.IsNullOrEmpty(model.ReturnUrl) ? model.ReturnUrl : Url.Action("Login"));

            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Name = model.Name,
                    Type = UserType.User,
                    IsActive = true,
                    RegisterDateTime = DateTime.UtcNow
                }, model.Password);

                if (result.Succeeded)
                {
                    // request for a local page
                    //return this.LoadingPage("Redirect",
                    //    Url.IsLocalUrl(model.ReturnUrl) ? model.ReturnUrl : Url.Action("Login"));

                    // TODO: Redirect to login page with returnUrl
                    //var returnUrl = Url.Action("Login", !string.IsNullOrEmpty(model.ReturnUrl) ? new { returnUrl = model.ReturnUrl } : null);
                    var returnUrl = !string.IsNullOrEmpty(model.ReturnUrl) ? model.ReturnUrl : Url.Action("Login");

                    return View("Message", new MessageViewModel
                    {
                        Type = MessageType.Success,
                        Title = "注册成功",
                        Message = string.Empty,
                        ReturnUrl = returnUrl,
                        RedirectSeconds = 3
                    });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }

            // something went wrong, show form with error
            return View(new RegisterViewModel
            {
                UserName = model.UserName,
                Password = model.Password,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Name = model.Name,
                ReturnUrl = model.ReturnUrl
            });
        }

        /// <summary>
        /// Entry point into the edit workflow
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(string returnUrl)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsActive)
            {
                return RedirectToAction("Logout");
            }

            var vm = new EditViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Name = user.Name,
                RegisterDateTime = user.RegisterDateTime,
                ReturnUrl = returnUrl
            };

            return View(vm);
        }
        
        /// <summary>
        /// Handle postback from edit
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(EditInputModel model, string button)
        {
            if (string.IsNullOrEmpty(model.ReturnUrl))
                model.ReturnUrl = Config.Web.Url;

            // the user clicked the "cancel" button
            if (button != "edit")
                return this.LoadingPage("Redirect", model.ReturnUrl);

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    ModelState.AddModelError("UserNotFound", "User not found.");
                }
                else if (!user.IsActive)
                {
                    ModelState.AddModelError("Banned", $"User {user.UserName} is banned.");
                }
                else if (user.Id != model.Id)
                {
                    ModelState.AddModelError("IdNotMatch", "Id does not match.");
                }
                else if (!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    ModelState.AddModelError("PasswordNotMatch", "Password does not match.");
                }
                else if (await _userManager.Users.AnyAsync(x => x.Id != model.Id && x.PhoneNumber == model.PhoneNumber))
                {
                    ModelState.AddModelError("DuplicatePhoneNumber", $"Phone number {model.PhoneNumber} is already taken.");
                }
                else
                {
                    IdentityResult result = null;

                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        result = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
                    }

                    if (result == null || result.Succeeded)
                    {
                        user.UserName = model.UserName;
                        user.Email = model.Email;
                        user.PhoneNumber = model.PhoneNumber;
                        user.Name = model.Name;

                        result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            if (User?.Identity.IsAuthenticated == true)
                            {
                                // delete local authentication cookie
                                await _signInManager.SignOutAsync();

                                // raise the logout event
                                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
                            }

                            return View("Message", new MessageViewModel
                            {
                                Type = MessageType.Success,
                                Title = "编辑成功",
                                Message = "你需要重新登录网页",
                                ReturnUrl = model.ReturnUrl,
                                RedirectSeconds = 3
                            });
                        }
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }
            }

            // something went wrong, show form with error
            return View(new EditViewModel
            {
                Id = model.Id,
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Name = model.Name,
                RegisterDateTime = model.RegisterDateTime
            });
        }

        /// <summary>
        /// Entry point into the delete workflow
        /// </summary>
        [HttpGet]
        public IActionResult Delete(string returnUrl)
        {
            var vm = new DeleteViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(vm);
        }

        /// <summary>
        /// Handle postback from delete
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(DeleteInputModel model, string button)
        {
            // the user clicked the "cancel" button
            if (button != "delete")
                return this.LoadingPage("Redirect", model.ReturnUrl);

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null || !user.IsActive)
                {
                    return Redirect(Url.Action("Logout", values: await _interaction.CreateLogoutContextAsync()));
                }

                if (await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        return View("Message", new MessageViewModel
                        {
                            Type = MessageType.Success,
                            Title = "删除账号成功",
                            Message = "该账号已被永久删除，如需继续使用网站请重新注册",
                            ReturnUrl = Url.Action("Login"),
                            RedirectSeconds = 3
                        });
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError("PasswordNotMatch", "Password does not match.");
                }
            }

            // something went wrong, show form with error
            return View(new DeleteViewModel
            {
                ReturnUrl = model.ReturnUrl
            });
        }

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "External", new { scheme = vm.ExternalLoginScheme, returnUrl });
            }

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            // the user clicked the "cancel" button
            if (button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }

                    return Redirect(model.ReturnUrl);
                }

                // since we don't have a valid context, then we just go back to the home page
                return Redirect("~/");
            }

            if (ModelState.IsValid)
            {
                var userName = model.UserName;

                // maybe login by email
                if (model.UserName.Contains('@'))
                {
                    var userByEmail = await _userManager.FindByEmailAsync(model.UserName);
                    userName = userByEmail?.UserName ?? userName;
                }

                var result = await _signInManager.PasswordSignInAsync(userName, model.Password, model.RememberLogin, lockoutOnFailure: true);
                string error;
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(userName);

                    if (!user.IsActive)
                    {
                        error = $"User {user.UserName} is banned.";
                    }
                    else
                    {
                        await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.Name, clientId: context?.Client.ClientId));

                        if (context != null)
                        {
                            if (context.IsNativeClient())
                            {
                                // The client is native, so this change in how to
                                // return the response is for better UX for the end user.
                                return this.LoadingPage("Redirect", model.ReturnUrl);
                            }

                            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                            return Redirect(model.ReturnUrl);
                        }

                        return Redirect("~/");
                    }
                }
                else
                {
                    error = result.ToString();
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(model.UserName, error, clientId: context?.Client.ClientId));
                ModelState.AddModelError(string.Empty, error);
            }

            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }
        
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServer4.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    UserName = context.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                UserName = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.UserName = model.UserName;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }
    }
}
