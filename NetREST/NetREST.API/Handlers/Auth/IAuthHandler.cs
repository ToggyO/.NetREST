using System.Threading.Tasks;
using NetREST.Common.Response;
using NetREST.DTO.Auth;

namespace NetREST.API.Handlers.Auth
{
    public interface IAuthHandler
    {
        Task<Response<AuthInfoDTO>> Login(AuthDTO dto);
    }
}