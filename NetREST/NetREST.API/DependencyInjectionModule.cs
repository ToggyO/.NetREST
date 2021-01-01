using Microsoft.Extensions.DependencyInjection;
using NetREST.API.Handlers.Users;
using NetREST.API.Handlers.Users.Implemetation;
using NetREST.Common.Extensions;
using NetREST.DAL.Repository.Users;
using NetREST.DAL.Repository.Users.Implementation;

namespace NetREST.API
{
    public static class DependencyInjectionModule
    {
        public static void Load(IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            services.Add<IUserHandler, UserHander>(serviceLifetime);
            services.Add<IUserRepository, UsersRepository>(serviceLifetime);
        }
    }
}