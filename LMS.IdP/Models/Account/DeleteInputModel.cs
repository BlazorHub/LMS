using System.ComponentModel.DataAnnotations;

namespace LMS.IdP.Models.Account
{
    public class DeleteInputModel
    {
        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
