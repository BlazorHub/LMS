using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using LMS.Shared;

namespace LMS.API.Models
{
    public class BaseController : ControllerBase, IAsyncActionFilter
    {
        protected UserManager<User> UserManager { get; set; }

        protected User CurrentUser { get; set; }
        
        protected Context Context { get; set; }

        public BaseController(IServiceProvider serviceProvider)
        {
            UserManager = serviceProvider.GetRequiredService<UserManager<User>>();
            Context = serviceProvider.GetRequiredService<Context>();
        }

        [NonAction]
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var hasAuthorizeAttribute = context.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>().Any();

            if (hasAuthorizeAttribute)
            {
                CurrentUser = await UserManager.GetUserAsync(User);

                // Not sure if it is needed, but it is there.
                if (CurrentUser == null)
                {
                    context.Result = Unauthorized();
                    return;
                }
            }

            await next();
        }
    }
}
