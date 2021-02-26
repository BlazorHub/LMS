using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using AntDesign.Pro.Layout;
using LMS.Shared;
using LMS.Web.Data;

namespace LMS.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.AddScoped<APIAuthorizationMessageHandler>();

            Action<HttpClient> configureClient = client =>
            {
                client.BaseAddress = new Uri(Config.API.Url);
                client.DefaultRequestHeaders.AcceptLanguage.Clear();
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.CurrentCulture.Name);
            };

            builder.Services.AddHttpClient($"{Config.API.Name}.Anonymous", configureClient);

            builder.Services.AddHttpClient($"{Config.API.Name}.Authorize", configureClient)
                .AddHttpMessageHandler<APIAuthorizationMessageHandler>();
            
            builder.Services.AddOidcAuthentication(options =>
            {
                options.ProviderOptions.Authority = Config.IdP.Url;
                options.ProviderOptions.ClientId = Config.Web.Name;

                //options.AuthenticationPaths.LogOutSucceededPath = "";

                // Issue: https://github.com/dotnet/aspnetcore/issues/25153
                //options.AuthenticationPaths.RemoteRegisterPath = $"{Config.IdP.Url}/Account/Register";
                //options.AuthenticationPaths.RemoteProfilePath = $"{Config.IdP.Url}/Account/Edit";

                options.ProviderOptions.DefaultScopes.Clear();
                options.ProviderOptions.DefaultScopes.Add("openid");
                options.ProviderOptions.DefaultScopes.Add(Config.API.Name);

                options.ProviderOptions.ResponseType = "id_token token"; //OpenIdConnectResponseType
            });

            builder.Services.AddAntDesign();
            builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));
            
            var host = builder.Build();
            var result = await host.Services.GetRequiredService<IJSRuntime>().InvokeAsync<string>("blazorCulture.get");
            var culture = result != null 
                ? CultureInfo.GetCultureInfo(result) 
                : CultureInfo.CurrentCulture.ConvertChineseCulture(CultureConvertDirection.ToInternal);

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            
            await host.RunAsync();
        }
    }
}
