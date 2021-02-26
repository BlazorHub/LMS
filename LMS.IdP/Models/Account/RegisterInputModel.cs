using LMS.Shared;

namespace LMS.IdP.Models.Account
{
    public class RegisterInputModel : Register
    {
        public string ReturnUrl { get; set; }
    }
}
