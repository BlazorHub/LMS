using System;
using Microsoft.AspNetCore.Identity;

namespace LMS.Shared
{
    public static class Config
    {
        public static class Site
        {
            public const string Name = "LMS";
            public const string DisplayName = "学习管理系统";
        }

        public static class IdP
        {
            public const string Url = "https://localhost:5001";
            public const string DisplayName = Site.DisplayName + " 用户中心";
        }
        
        public static class API
        {
            public const string Url = "https://localhost:5000";
            public const string Name = "API";
            public const string DisplayName = Site.DisplayName + " API";
            public const string Secret = "api secret";
        }

        public static class Web
        {
            public const string Url = "https://localhost:4000";
            public const string Name = "Web";
            public const string DisplayName = Site.DisplayName + " Web";
            public const string Secret = "web secret";
        }
        
        public static class Message
        {
            public static class Page
            {
                public static class Account
                {
                    public const string Register = "";
                    public const string Login = "The default users are defaultadmin/defaultuser/longname/shortname, passwords are the same.";
                    public const string Edit = "You need to login again after making changes.";
                    public const string Delete = "Caution! Your account cannot be restored after completion!";
                }
            }
        }

        // Caution! All existing databases will be dropped if this value is true.
        public const bool ReCreateDatabaseOnStart = false;

        public static readonly Database Database = new ()
        {
            Type = DatabaseType.PostgreSQL,
            Version = "13.2",
            Server = "localhost",
            Port = "5432",
            UserName = "postgres",
            Password = "postgres",
            Name = $"{Config.Site.Name}_Dev"
        };

        public static readonly Action<IdentityOptions> IdentityOptions = options =>
        {
            options.ClaimsIdentity.RoleClaimType = "role";
            // This is UserName, not User.Name
            options.ClaimsIdentity.UserNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
            options.ClaimsIdentity.UserIdClaimType = "sub";
            options.ClaimsIdentity.EmailClaimType = "email";

            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._+";
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 1;
            options.Password.RequiredUniqueChars = 1;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireDigit = false;

            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.SignIn.RequireConfirmedAccount = false;

            options.Stores.MaxLengthForKeys = -1;
            options.Stores.ProtectPersonalData = false;
        };

        public static class User
        {
            public static class UserName
            {
                public const int MinimumLength = 5;
                public const int MaximumLength = 32;
            }
            
            public static class Email
            {
                public const int MinimumLength = 6;
                public const int MaximumLength = 256;
            }

            public static class PhoneNumber
            {
                public const int Length = 11;
            }

            public static class Name
            {
                public const int MinimumLength = 2;
                public const int MaximumLength = 32;
            }
        }
    }
}
