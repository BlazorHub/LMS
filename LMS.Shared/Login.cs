using System.ComponentModel.DataAnnotations;

namespace LMS.Shared
{
    public class Login
    {
        /// <summary>用户名</summary>
        [Display(Name = "用户名")]
        [DataType(DataType.Text)]
        [Required]
        [StringLength(Config.User.UserName.MaximumLength, MinimumLength = Config.User.UserName.MinimumLength)]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
