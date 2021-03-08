using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Routing;
using AntDesign;
using LMS.Shared;

namespace LMS.Web.Pages.User
{
    public partial class Index : IDisposable
    {
        private readonly IDictionary<string, (bool edit, LMS.Shared.User data)> _editCache = new Dictionary<string, (bool edit, LMS.Shared.User data)>();
        
        private PagedData<LMS.Shared.User> _pagedData;

        private IEnumerable<LMS.Shared.User> _data;

        private bool _locationChanged;
        
        private int _total;

        private int _page;

        private int _pageSize;

        private string _keyword;
        
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            ParseQueryString();

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
            ParseQueryString();
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
            _pagedData = await AuthorizeHttpClient.GetFromJsonAsync<PagedData<LMS.Shared.User>>(GenerateQueryString("/accounts"));
            _data = _pagedData?.Data;
            _total = _pagedData?.Total ?? default;
            UpdateEditCache();
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

        private void StartEdit(string id)
        {
            var data = _editCache[id];
            data.edit = true;
            _editCache[id] = data;
        }

        private void CancelEdit(string id)
        {
            var data = _data.FirstOrDefault(item => item.Id == id);
            _editCache[id] = new (false, data);
        }

        private void SaveEdit(string id)
        {
            var data = _data.FirstOrDefault(item => item.Id == id);
            _editCache[id] = new (false, data);
        }

        private void UpdateEditCache()
        {
            _data.ForEach(item =>
            {
                _editCache[item.Id] = new (false, item);
            });
        }
    }
}
