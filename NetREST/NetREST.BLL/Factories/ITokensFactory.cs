using System.Security.Claims;
using System.Threading.Tasks;
using NetREST.Common.Enums;
using NetREST.Domain.User;
using NetREST.DTO.Auth;

namespace NetREST.BLL.Factories
{
    public interface ITokensFactory
    {
        Task<TokenDTO> CreateToken(UserModel user);
        TokenStatus ValidateToken(string token, out ClaimsPrincipal principal);
    }
}