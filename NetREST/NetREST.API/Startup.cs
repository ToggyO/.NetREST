using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NetREST.API.Extensions;
using NetREST.API.Filters;
using NetREST.API.Middleware;
using NetREST.BLL.Mappings;
using NetREST.BLL.Services.WebSocket;
using NetREST.Common.Settings;
using NetREST.DAL;
using NetREST.DTO;

namespace NetREST.API
{
    public class Startup
    {
        private const string MyAllowOrigins = "_myAllowOrigins";

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
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowOrigins,
                    builder => builder
                        // .WithOrigins("http://localhost:63342")
                        .AllowAnyOrigin()
                        // .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                    );
            });
            
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
            services.Configure<ApiSettings>(Configuration.GetSection("AppSettings:APISettings"));

            var dbSettings = new DbSettings
            {
                DB_HOST = Configuration.GetSection("DB_HOST").Value,
                DB_PORT = Configuration.GetSection("DB_PORT").Value,
                DB_USER = Configuration.GetSection("DB_USER").Value,
                DB_PASSWORD = Configuration.GetSection("DB_PASSWORD").Value,
                DB_NAME = Configuration.GetSection("DB_NAME").Value
            };

            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseNpgsql(dbSettings.DbConnectionString,
                    optionsBuilder =>
                        optionsBuilder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            
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
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!String.IsNullOrEmpty(path)
                                && path.StartsWithSegments("/api/ws"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
            
            services.AddAuthorization();
            
            services.AddSignalR();

            services.AddControllers(options =>
                {
                    options.Filters.Add(typeof(ValidateModelAttribute));
                    options.Filters.Add(typeof(StatusCodeFilter));
                })
                .AddFluentValidation(fv =>
                    {
                        ValidatorConfigurationOverload.Override();
                        fv.RegisterValidatorsFromAssemblyContaining<DTO.DependencyInjectionModule>();
                    });

            services.AddDirectoryBrowser();

            services.AddSingleton(MappingConfig.GetMapper());
			
            DependencyInjectionModule.Load(services);
        }

        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            ILogger<Startup> logger)
        {
            logger.LogInformation("Enter Configure");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            logger.LogInformation("Static files");
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.WebRootPath, "chat")),
                RequestPath = "/chat"
            });
            
            logger.LogInformation("Routing");
            app.UseRouting();
            
            logger.LogInformation("Cors");
            app.UseCors(MyAllowOrigins);

            logger.LogInformation("Auth middleware");
            app.UseAuthentication();
            app.UseAuthorization();

            logger.LogInformation("EnsureMigrationOfContext");
            app.EnsureMigrationOfContext<ApplicationDbContext>();

            logger.LogInformation("Custom middleware");
            app.UseMiddleware<ExceptionMiddleware>();
            
            logger.LogInformation("Endpoints");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHubService>("api/ws/chat");
            });
            logger.LogInformation("Exit Configure");
        }
    }
}
