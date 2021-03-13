using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Routing;
using AntDesign;
using LMS.Shared;

namespace LMS.Web.Pages.User
{
    public partial class Index : IDisposable
    {
        private PagedData<Shared.User> _pagedData;

        private IEnumerable<Shared.User> _data;
        
        private bool _locationChanged;
        
        private int _total;

        private int _page;

        private int _pageSize;

        private string _keyword;
        
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            
            NavigationManager.LocationChanged += HandleLocationChanged;

            await GetPagedDataAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_locationChanged)
            {
                _locationChanged = false;

                await GetPagedDataAsync();

                StateHasChanged();
            }
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= HandleLocationChanged;
        }

        private void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            _locationChanged = true;
        }

        private void OnPageIndexChange(PaginationEventArgs args)
        {
            _page = args.PageIndex;
            _pageSize = args.PageSize;
            NavigationManager.NavigateTo(GenerateQueryString("/user"));
        }

        private async Task GetPagedDataAsync()
        {
            ParseQueryString();

            _pagedData = await AuthorizeHttpClient.GetFromJsonAsync<PagedData<Shared.User>>(GenerateQueryString("/accounts"));
            _data = _pagedData?.Data;
            _total = _pagedData?.Total ?? default;
        }

        private async Task AfterDelete()
        {
            await GetPagedDataAsync();

            StateHasChanged();
        }
        
        // Set QueryString to internal field from URL
        private void ParseQueryString()
        {
            var query = GetQueryString(NavigationManager);

            _page = 1;
            if (query.TryGetValue("page", out var pageString))
            {
                if (int.TryParse(pageString, out var page) && page > 0)
                    _page = page;
            }

            _pageSize = 10;
            if (query.TryGetValue("pageSize", out var pageSizeString))
            {
                if (int.TryParse(pageSizeString, out var pageSize) && pageSize > 0)
                    _pageSize = pageSize;
            }
            
            if (query.TryGetValue("keyword", out var keyword))
            {
                _keyword = keyword;
            }
        }

        // Generate QueryString from internal field
        private string GenerateQueryString(string baseUrl)
        {
            var query = new Dictionary<string, string>
            {
                { "page", _page.ToString() },
                { "pageSize", _pageSize.ToString() }
            };

            if (!string.IsNullOrWhiteSpace(_keyword))
            {
                query.Add("keyword", _keyword);
            }

            return AddQueryString(baseUrl, query);
        }
    }
}
