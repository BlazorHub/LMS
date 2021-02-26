using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace LMS.Shared
{
    public static class Seeder
    {
        public static async Task SeedAsync(Context context, UserManager<User> userManager)
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            
            await userManager.CreateAsync(new User
            {
                UserName = "defaultadmin",
                Email = "defaultadmin@LMS.com",
                PhoneNumber = "10000000001",
                Name = "默认管理员",
                Type = UserType.Administrator,
                IsActive = true,
                RegisterDateTime = DateTime.UtcNow
            }, "defaultadmin");
            
            await userManager.CreateAsync(new User
            {
                UserName = "defaultuser",
                Email = "defaultuser@LMS.com",
                PhoneNumber = "00000000001",
                Name = "默认用户",
                Type = UserType.User,
                IsActive = true,
                RegisterDateTime = DateTime.UtcNow
            }, "defaultuser");

            await userManager.CreateAsync(new User
            {
                UserName = "longname",
                Email = "longname@LMS.com",
                PhoneNumber = "00000000002",
                Name = "一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二",
                Type = UserType.User,
                IsActive = true,
                RegisterDateTime = DateTime.UtcNow
            }, "longname");

            await userManager.CreateAsync(new User
            {
                UserName = "shortname",
                Email = "shortname@LMS.com",
                PhoneNumber = "00000000003",
                Name = "..",
                Type = UserType.User,
                IsActive = true,
                RegisterDateTime = DateTime.UtcNow
            }, "shortname");
        }
    }
}
