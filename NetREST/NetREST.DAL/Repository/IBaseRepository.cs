using System.Threading.Tasks;

namespace NetREST.DAL.Repository
{
    public interface IBaseRepository<T>
    {
        Task<T> GetById(int id);
        Task<T> Create(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(int id);
    }
}