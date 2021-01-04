using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetREST.API.Handlers.Auth;
using NetREST.BLL.Services.Auth;
using NetREST.Common.Errors;
using NetREST.Common.Response;
using NetREST.DTO.Auth;
using NetREST.DTO.SignUp;
using NetREST.DTO.User;

namespace NetREST.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthHandler _authHandler;
        private readonly IAuthService _authService;
        
        public AuthController(IAuthHandler authHandler,
            IAuthService authService)
        {
            _authHandler = authHandler;
            _authService = authService;
        }
        
        [HttpPost("login")]
        public async Task<Response<AuthInfoDTO>> Login([Required, FromBody] AuthDTO dto)
        {
            var principal = await _authService.Authenticate(dto);

            if (principal == null)
                return new ErrorResponse<AuthInfoDTO>
                {
                    HttpStatusCode = HttpStatusCode.Conflict,
                    ErrorCode = ErrorCodes.Security.AuthDataInvalid,
                    ErrorMessage = ErrorMessages.Security.AuthDataInvalid,
                    Errors = new Error[0],
                };
            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return await _authHandler.Login(dto);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<Response> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return _authHandler.Logout();
        }

        [HttpPost("signup")]
        public async Task<Response<UserDTO>> SignUp([Required, FromBody] SignUpDTO dto)
        {
            return await _authHandler.SignUp(dto);
        }
    }
}