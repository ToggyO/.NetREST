using Microsoft.Extensions.DependencyInjection;
using NetREST.BLL.Services.Auth;
using NetREST.BLL.Services.Auth.Implementation;
using NetREST.Common.Extensions;

namespace NetREST.BLL
{
    public static class DependencyInjectionModule
    {
        public static void Load(IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            DAL.DependencyInjectionModule.Load(services);

            services.Add<IAuthService, AuthService>(serviceLifetime);
        }
    }
}