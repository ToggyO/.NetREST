using System.Threading.Tasks;
using NetREST.Common.Pagination;
using NetREST.Common.Response;
using NetREST.DTO.User;

namespace NetREST.API.Handlers.Users
{
    public interface IUserHandler
    {
        Task<Response<PaginationModel<UserDTO>>> GetList(PaginationModel model);
        Task<Response<UserDTO>> GetUserById(int id);
        Task<Response<UserDTO>> UpdateUser(int id, UserDTO dto);
        Task<Response<UserDTO>> DeleteUser(int id);
    }
}