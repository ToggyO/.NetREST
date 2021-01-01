using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetREST.API.Handlers.Auth;
using NetREST.Common.Response;
using NetREST.DTO.Auth;

namespace NetREST.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthHandler _handler;
        
        public AuthController(IAuthHandler handler)
        {
            _handler = handler;
        }
        
        [HttpPost]
        public async Task<Response<AuthInfoDTO>> Login(AuthDTO dto)
        {
            return await _handler.Login(dto);
        }
    }
}