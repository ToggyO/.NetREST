using System.Threading.Tasks;
using NetREST.Common.Response;
using NetREST.DTO.Auth;
using NetREST.DTO.SignUp;
using NetREST.DTO.User;

namespace NetREST.API.Handlers.Auth
{
    public interface IAuthHandler
    {
        Task<Response<AuthInfoDTO>> Login(AuthDTO dto);
        Task<Response<TokenDTO>> RefreshToken(string token);
        Task<Response<UserDTO>> SignUp(SignUpDTO dto);
    }
}