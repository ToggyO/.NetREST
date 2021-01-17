using AutoMapper;
using NetREST.Common.Models.Chat;
using NetREST.Domain.User;
using NetREST.DTO.User;

namespace NetREST.BLL.Mappings.Profiles
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<UserModel, UserDTO>().ReverseMap();
            CreateMap<UserModel, UserCreateDTO>().ReverseMap();
            CreateMap<UserModel, ChatUser>().ReverseMap();
        }
    }
}