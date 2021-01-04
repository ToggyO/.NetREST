using System.Security.Claims;
using System.Threading.Tasks;
using NetREST.DTO.Auth;

namespace NetREST.BLL.Services.Auth
{
    public interface IAuthService
    {
        Task<ClaimsPrincipal> Authenticate(AuthDTO dto);
    }
}