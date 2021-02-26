using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using LMS.Shared;

namespace LMS.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(Config.Web.Name, builder => builder
                    .WithOrigins(Config.Web.Url)
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddControllers()
                .AddDataAnnotationsLocalization();

            services.AddDbContext<Context>(options => options.UseDatabase(Config.Database));

            services.AddAuthorization();

            services.AddAuthentication("Bearer")
                .AddOAuth2Introspection("Bearer", options =>
                {
                    options.Authority = Config.IdP.Url;
                    options.ClientId = Config.API.Name;
                    options.ClientSecret = Config.API.Secret;
                });
            
            services.AddIdentityCore<User>(Config.IdentityOptions)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<Context>()
                .AddDefaultTokenProviders();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = Config.API.DisplayName, Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Context context, UserManager<User> userManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Config.API.DisplayName} v1"));
            }

            app.Use((context, next) =>
            {
                context.Request.Headers["Accept-Language"] = context.Request.Headers["Accept-Language"]
                    .Select(lang => lang
                        .ToLowerInvariant()
                        .Replace("zh-cn", "zh-hans-cn")
                        .Replace("zh-tw", "zh-hant-tw"))
                    .ToArray();
                return next();
            });

            var supportedCultures = new []
            {
                "en",
                "zh-Hans"
            };

            app.UseRequestLocalization(new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures));
            
            app.UseRouting();

            app.UseCors(Config.Web.Name);

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (Config.ReCreateDatabaseOnStart)
            {
                Seeder.SeedAsync(context, userManager).Wait();
            }
        }
    }
}
