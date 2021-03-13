using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using LMS.API.Models;

namespace LMS.API.Controllers
{
    [Route("courses")]
    [ApiController]
    public class CourseController : BaseController
    {
        private IStringLocalizer<CourseController> Localizer { get; }

        public CourseController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Localizer = serviceProvider.GetRequiredService<IStringLocalizer<CourseController>>();
        }
    }
}
