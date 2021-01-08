using System.Threading.Tasks;
using NetREST.Common.Response;
using NetREST.DTO.Auth;

namespace NetREST.BLL.Services.Tokens
{
    public interface ITokensService
    {
        Task<Response<AuthInfoDTO>> Authenticate(AuthDTO dto);
        Task<Response<TokenDTO>> RefreshToken(string refreshToken);
    }
}