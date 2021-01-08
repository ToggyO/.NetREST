using System;
using System.Text;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NetREST.API.Filters;
using NetREST.BLL.Mappings;
using NetREST.Common.Settings;
using NetREST.DAL;

namespace NetREST.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
		
        public Startup(IWebHostEnvironment env)
        {
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var apiSettings = Configuration.GetSection("AppSettings:APISettings").Get<ApiSettings>();
            Console.WriteLine(apiSettings.PublicKey);
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
            services.Configure<ApiSettings>(Configuration.GetSection("AppSettings:APISettings"));

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

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var apiSettings = Configuration.GetSection("AppSettings:APISettings").Get<ApiSettings>();
                    // options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.Zero,
                        
                        ValidateAudience = true,
                        ValidAudience = apiSettings.AUDIENCE,
                        
                        ValidateIssuer = true,
                        ValidIssuer = apiSettings.ISSUER,
                        
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiSettings.PublicKey)),
                        
                        // To allow return custom response for expired token
                        ValidateLifetime = false,
                    };
                });

            services.AddAuthorization();
            
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
