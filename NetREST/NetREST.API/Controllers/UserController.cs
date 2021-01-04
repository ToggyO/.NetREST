using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetREST.API.Handlers.Users;
using NetREST.Common.Pagination;
using NetREST.Common.Response;
using NetREST.DTO.User;

namespace NetREST.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
	    private readonly IUserHandler _handler;
	    
	    public UserController(IUserHandler handler)
	    {
		    _handler = handler;
	    }

		[HttpGet]
		[AllowAnonymous]
		public async Task<Response<PaginationModel<UserDTO>>> GetList([FromQuery] PaginationModel model)
		{
			return await _handler.GetList(model);
		}

		[HttpGet("{id}")]
		public async Task<Response<UserDTO>> Get([Required, FromRoute] int id)
		{
			return await _handler.GetUserById(id);
		}

		[HttpPut("{id}")]
		public async Task<Response<UserDTO>> Put([Required, FromRoute] int id,
			[FromBody] UserDTO user)
		{
			return await _handler.UpdateUser(id, user);
		}

		[HttpDelete("{id}")]
		public async Task<Response> Delete(int id)
		{
			return await _handler.DeleteUser(id);
		}
    }
}