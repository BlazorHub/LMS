using System.Net.Http;
using Microsoft.AspNetCore.Components;

namespace LMS.Web.Data
{
    public class BaseComponent : ComponentBase
    {
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
