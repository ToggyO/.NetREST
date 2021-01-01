using AutoMapper;
using NetREST.BLL.Mappings.Profiles;

namespace NetREST.BLL.Mappings
{
    public class MappingConfig
    {
        private static readonly MapperConfiguration Config = new MapperConfiguration(config =>
        {
            config.AddProfile<UserMapperProfile>();
        });

        public static IMapper GetMapper()
        {
            Config.AssertConfigurationIsValid();
            return Config.CreateMapper();
        }
    }
}