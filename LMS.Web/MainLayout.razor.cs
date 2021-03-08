using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using AntDesign.Pro.Layout;
using LMS.Shared;

namespace LMS.Web
{
    public partial class MainLayout
    {
        [Inject]
        private IStringLocalizer<MainLayout> Localizer { get; set; }

        // https://gw.alipayobjects.com/zos/rmsportal/KDpgvguMpGfqaHPjicRK.svg
        private readonly OneOf.OneOf<string, RenderFragment> _logo = "/lms_icon_b.svg";

        public bool ShouldUpdateMenuData { get; set; } = true;

        private MenuDataItem[] _initialMenuData;

        private MenuDataItem[] _menuData;
        
        protected override void OnInitialized()
        {
            base.OnInitialized();
            
            _initialMenuData = new MenuDataItem[]
            {
                new ()
                {
                    Path = "/",
                    Name = Localizer["Home"],
                    Key = "/",
                    Icon = "home"
                }
            };
            _menuData = _initialMenuData;
        }
        
        public void UpdateMenuData(User user)
        {
            ShouldUpdateMenuData = false;
            _menuData = CreateMenuData(user);
            StateHasChanged();
        }

        private MenuDataItem[] CreateMenuData(User user)
        {
            var menuDataList = _initialMenuData.ToList();

            menuDataList.Add(new MenuDataItem
            {
                Path = "/userinfo",
                Name = "查看用户信息（调试用）",
                Key = "/userinfo",
                Icon = "user"
            });

            if (user != null && user.Type == UserType.Administrator)
            {
                menuDataList.Add(new MenuDataItem
                {
                    Name = "用户",
                    Icon = "user",
                    Children = new MenuDataItem[]
                    {
                        new ()
                        {
                            Path = "/user",
                            Name = "用户列表",
                            Key = "/user"
                        }
                    }
                });
            }

            return menuDataList.ToArray();
        }
    }
}
