using LMS.Shared;

namespace LMS.IdP.Models.Account
{
    public class LoginInputModel : Login
    {
        public bool RememberLogin { get; set; }

        public string ReturnUrl { get; set; }
    }
}
