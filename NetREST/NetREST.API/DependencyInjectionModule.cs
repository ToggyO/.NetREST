using Microsoft.Extensions.DependencyInjection;
using NetREST.API.Handlers.Auth;
using NetREST.API.Handlers.Auth.Implementation;
using NetREST.API.Handlers.Users;
using NetREST.API.Handlers.Users.Implemetation;
using NetREST.Common.Extensions;

namespace NetREST.API
{
    public static class DependencyInjectionModule
    {
        public static void Load(IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            BLL.DependencyInjectionModule.Load(services);

            services.Add<IUserHandler, UserHander>(serviceLifetime);
            services.Add<IAuthHandler, AuthHandler>(serviceLifetime);
        }
    }
}