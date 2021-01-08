using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetREST.API.Handlers.Auth;
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
        
        public AuthController(IAuthHandler authHandler)
        {
            _authHandler = authHandler;
        }
        
        [HttpPost("token")]
        public async Task<Response<AuthInfoDTO>> Login([Required, FromBody] AuthDTO dto)
        {
            return await _authHandler.Login(dto);
        }

        [HttpPut("token")]
        public async Task<Response<TokenDTO>> RefreshToken([Required, FromBody] RefreshTokenDTO dto)
        {
            return await _authHandler.RefreshToken(dto.RefreshToken);
        }

        [HttpPost("signup")]
        public async Task<Response<UserDTO>> SignUp([Required, FromBody] SignUpDTO dto)
        {
            return await _authHandler.SignUp(dto);
        }
    }
}