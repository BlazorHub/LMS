using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using LMS.Shared;
using LMS.API.Models;

namespace LMS.API.Controllers
{
    [Route("accounts")]
    [ApiController]
    public class AccountController : BaseController
    {
        private IStringLocalizer<AccountController> Localizer { get; }

        public AccountController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Localizer = serviceProvider.GetRequiredService<IStringLocalizer<AccountController>>();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
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
        
        [HttpGet("")]
        [Authorize]
        public IActionResult Get
        (
            [FromQuery] [Range(1, int.MaxValue)] int page,
            [FromQuery] [Range(1, 100)] int pageSize,
            [FromQuery] string keyword
        )
        {
            if (CurrentUser.Type < UserType.Administrator)
                return Forbid();

            var users = UserManager.Users;

            if (!string.IsNullOrEmpty(keyword))
            {
                Func<string, string, bool> like = Context.Database.IsNpgsql() ? EF.Functions.ILike : EF.Functions.Like;
                
                users = users.Where(x =>
                    like(x.UserName, $"%{keyword}%") ||
                    like(x.Email, $"%{keyword}%") ||
                    like(x.PhoneNumber, $"%{keyword}%"));
            }

            return Ok(new PagedData<User>(users.OrderBy(x => x.RegisterDateTime), page, pageSize));
        }

        // For Administrator only
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(string id, User user)
        {
            if (CurrentUser.Type < UserType.Administrator)
                return Forbid();

            if (id != user.Id)
            {
                ModelState.AddModelError("IdNotMatch", Localizer["IdNotMatch"]);
                return BadRequest(ModelState);
            }

            var userFromDatabase = await UserManager.FindByIdAsync(id);
            if (userFromDatabase == null)
                return NotFound();

            if (await UserManager.Users.AnyAsync(x => x.Id != id && x.PhoneNumber == user.PhoneNumber))
            {
                ModelState.AddModelError("DuplicatePhoneNumber", Localizer["DuplicatePhoneNumber", user.PhoneNumber]);
                return BadRequest(ModelState);
            }

            userFromDatabase.Type = user.Type;
            userFromDatabase.UserName = user.UserName;
            userFromDatabase.Email = user.Email;
            userFromDatabase.PhoneNumber = user.PhoneNumber;
            userFromDatabase.Name = user.Name;
            userFromDatabase.IsActive = user.IsActive;

            var result = await UserManager.UpdateAsync(userFromDatabase);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        // For Administrator only
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            if (CurrentUser.Type < UserType.Administrator)
                return Forbid();

            // Could not delete yourself
            if (CurrentUser.Id == id)
                return BadRequest();

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await UserManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(error.Code, error.Description);

                return BadRequest(ModelState);
            }

            return NoContent();
        }
    }
}
