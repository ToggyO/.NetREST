using Microsoft.Extensions.DependencyInjection;
using NetREST.Common.Extensions;
using NetREST.DAL.Repository.Users;
using NetREST.DAL.Repository.Users.Implementation;

namespace NetREST.DAL
{
    public static class DependencyInjectionModule
    {
        public static void Load(IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            services.Add<IUserRepository, UsersRepository>(serviceLifetime);
        }
    }
}