using System.Linq.Expressions;

namespace Stokvel_Groups_Home_RSA.Interface.Infrastructure
{
    public interface IRepository<T> where T : class
    {

        // T - Category
        Task<IEnumerable<T?>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        Task<T?> Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
        IQueryable<T?> GetList();
        Task<T?> GetByIdAsync(string? id);
        Task<T?> GetByIdAsync(int? id);
        Task Add(T entity);
        Task RemoveAsync(T entity);
        Task RemoveRange(IEnumerable<T> entities);
    }
}
