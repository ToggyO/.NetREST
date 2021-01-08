using Microsoft.Extensions.DependencyInjection;
using NetREST.BLL.Factories;
using NetREST.BLL.Factories.Implementation;
using NetREST.BLL.Services.Tokens;
using NetREST.BLL.Services.Tokens.Implementation;
using NetREST.Common.Extensions;

namespace NetREST.BLL
{
    public static class DependencyInjectionModule
    {
        public static void Load(IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            DAL.DependencyInjectionModule.Load(services);

            services.Add<ITokensFactory, TokensFactory>(serviceLifetime);
            services.Add<ITokensService, TokensService>(serviceLifetime);
        }
    }
}