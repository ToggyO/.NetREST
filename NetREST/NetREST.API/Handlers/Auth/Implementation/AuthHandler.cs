using System.Threading.Tasks;
using NetREST.BLL.Services.Auth;
using NetREST.Common.Response;
using NetREST.DTO.Auth;

namespace NetREST.API.Handlers.Auth.Implementation
{
    public class AuthHandler : IAuthHandler
    {
        private readonly IAuthService _authService;
        
        public AuthHandler(IAuthService authService)
        {
            _authService = authService;
        }
        
        public async Task<Response<AuthInfoDTO>> Login(AuthDTO dto)
        {
            
        }
    }
}