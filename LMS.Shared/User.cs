using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LMS.Shared
{
    /// <summary>用户类型</summary>
    public enum UserType
    {
        /// <summary>用户</summary>
        [Display(Name = "用户")]
        User = 1,

        /// <summary>管理员</summary>
        [Display(Name = "管理员")]
        Administrator = 255
    }

    /// <summary>用户</summary>
    public class User : IdentityUser
    {
        /// <summary>编号</summary>
        [Display(Name = "编号")]
        public override string Id { get; set; }

        /// <summary>类型</summary>
        [Display(Name = "类型")]
        [EnumDataType(typeof(UserType))]
        public UserType Type { get; set; }

        /// <summary>用户名</summary>
        [Display(Name = "用户名")]
        [DataType(DataType.Text)]
        [Required]
        [StringLength(Config.User.UserName.MaximumLength, MinimumLength = Config.User.UserName.MinimumLength)]
        public override string UserName { get; set; }
        
        /// <summary>邮箱</summary>
        [Display(Name = "邮箱")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Required]
        [StringLength(Config.User.Email.MaximumLength, MinimumLength = Config.User.Email.MinimumLength)]
        public override string Email { get; set; }

        /// <summary>手机号</summary>
        [Display(Name = "手机号")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"\d+")]
        [Required]
        [StringLength(Config.User.PhoneNumber.Length, MinimumLength = Config.User.PhoneNumber.Length)]
        public override string PhoneNumber { get; set; }

        /// <summary>姓名</summary>
        [Display(Name = "姓名")]
        [DataType(DataType.Text)]
        [Required]
        [StringLength(Config.User.Name.MaximumLength, MinimumLength = Config.User.Name.MinimumLength)]
        public string Name { get; set; }

        /// <summary>账号激活状态</summary>
        [Display(Name = "账号激活状态")]
        [Required]
        public bool IsActive { get; set; }

        /// <summary>注册时间</summary>
        [Display(Name = "注册时间")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime RegisterDateTime { get; set; }

        public object ToSafe()
        {
            return new
            {
                Id,
                Type,
                UserName,
                Email,
                PhoneNumber,
                Name,
                IsActive,
                RegisterDateTime,

                EmailConfirmed,
                PhoneNumberConfirmed,
                TwoFactorEnabled,
                LockoutEnd,
                LockoutEnabled,
                AccessFailedCount
            };
        }
    }
}
