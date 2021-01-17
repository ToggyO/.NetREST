using System.Threading.Tasks;
using NetREST.Common.Models.Pagination;
using NetREST.Domain.User;

namespace NetREST.DAL.Repository.Users
{
    public interface IUserRepository : IBaseRepository<UserModel>
    {
        Task<PaginationModel<UserModel>> GetList(PaginationModel entity);
        Task<UserModel> GetUserByEmail(string email);
    }
}