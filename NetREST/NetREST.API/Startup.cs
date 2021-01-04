using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetREST.API.Filters;
using NetREST.BLL.Mappings;
using NetREST.Common.Settings;
using NetREST.DAL;

namespace NetREST.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
		
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

            var dbSettings = new DbSettings
            {
                DB_HOST = Configuration.GetSection("DB_HOST").Value,
                DB_PORT = Configuration.GetSection("DB_PORT").Value,
                DB_USER = Configuration.GetSection("DB_USER").Value,
                DB_PASSWORD = Configuration.GetSection("DB_PASSWORD").Value,
                DB_NAME = Configuration.GetSection("DB_NAME").Value,
            };

            services.AddDbContext<ApplicationContext>(options => 
                options.UseNpgsql(dbSettings.DbConnectionString));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ValidateModelAttribute));
                options.Filters.Add(typeof(StatusCodeFilter));
            }).AddFluentValidation(
                fv =>
                    fv.RegisterValidatorsFromAssemblyContaining<DTO.DependencyInjectionModule>());

            services.AddSingleton(MappingConfig.GetMapper());
			
            DependencyInjectionModule.Load(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
