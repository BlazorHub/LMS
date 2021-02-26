using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LMS.Web.Data
{
    public class BaseLayoutComponent : LayoutComponentBase
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Inject]
        private IHttpClientFactory HttpClientFactory { get; set; }

        protected HttpClient AnonymousHttpClient { get; set; }

        protected HttpClient AuthorizeHttpClient { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            AnonymousHttpClient = HttpClientFactory.CreateAnonymousClient();
            AuthorizeHttpClient = HttpClientFactory.CreateAuthorizeClient();
        }
    }
}
