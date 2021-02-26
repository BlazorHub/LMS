using System.ComponentModel.DataAnnotations;

namespace LMS.Shared
{
    public class Register
    {
        /// <summary>用户名</summary>
        [Display(Name = "用户名")]
        [DataType(DataType.Text)]
        [Required]
        [StringLength(Config.User.UserName.MaximumLength, MinimumLength = Config.User.UserName.MinimumLength)]
        public string UserName { get; set; }

        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        /// <summary>邮箱</summary>
        [Display(Name = "邮箱")]
        [DataType(DataType.EmailAddress)]
        [Required]
        [StringLength(Config.User.Email.MaximumLength, MinimumLength = Config.User.Email.MinimumLength)]
        public string Email { get; set; }

        /// <summary>手机号</summary>
        [Display(Name = "手机号")]
        [DataType(DataType.PhoneNumber)]
        [Required]
        [StringLength(Config.User.PhoneNumber.Length, MinimumLength = Config.User.PhoneNumber.Length)]
        public string PhoneNumber { get; set; }

        /// <summary>姓名</summary>
        [Display(Name = "姓名")]
        [DataType(DataType.Text)]
        [Required]
        [StringLength(Config.User.Name.MaximumLength, MinimumLength = Config.User.Name.MinimumLength)]
        public string Name { get; set; }
    }
}
