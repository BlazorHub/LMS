using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.Shared;
using LMS.API.Models;

namespace LMS.API.Controllers
{
    [Route("accounts")]
    [ApiController]
    public class AccountController : BaseController
    {
        public AccountController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(string id)
        {
            User user;

            if (CurrentUser.Id == id)
            {
                user = CurrentUser;
            }
            else if (CurrentUser.Type == UserType.Administrator)
            {
                user = await UserManager.FindByIdAsync(id);
                if (user == null)
                    return NotFound();
            }
            else
            {
                return Forbid();
            }
            
            return Ok(user.ToSafe());
        }
    }
}
