using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetREST.Common.Models.Pagination;
using NetREST.Domain.User;

namespace NetREST.DAL.Repository.Users.Implementation
{
    public class UsersRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;

        public UsersRepository(ApplicationDbContext context)
        {
            _db = context;
            // if (!_db.Users.Any())
            // {
            //     _db.Users.Add(new UserModel { Name = "Alina", Age = 30 });
            //     _db.Users.Add(new UserModel { Name = "Oleg", Age = 28 });
            //     _db.SaveChanges();
            // }
        }
        
        public async Task<PaginationModel<UserModel>> GetList(PaginationModel model)
        {
            var total = await _db.Users.CountAsync();
            var items = await _db.Users
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .OrderBy(x => x.Id)
                .ToListAsync();
            var result = new PaginationModel<UserModel>
            {
                Page = model.Page,
                PageSize = model.PageSize,
                Total = total,
                Items = items,
            };
            return result;
        }

        public async Task<UserModel> GetUserByEmail(string email)
        {
            if (email == null) return null;

            UserModel user = await _db.Users.FirstOrDefaultAsync(
                x => x.Email.ToLower() == email.ToLower());

            return user;
        }

        public async Task<UserModel> GetById(int id)
        {
            UserModel user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            return user;
        }

        public async Task<UserModel> Create(UserModel entity)
        {
            var user = await _db.Users.AddAsync(entity);
            await _db.SaveChangesAsync();
            UserModel result = user.Entity;
            return result;
        }

        public async Task<UserModel> Update(UserModel entity)
        {
            _db.Users.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<UserModel> Delete(int id)
        {
            UserModel user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return user;
        }
    }
}