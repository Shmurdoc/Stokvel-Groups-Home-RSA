using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.Infrastructure;
using System.Linq.Expressions;

namespace Stokvel_Groups_Home_RSA.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();

        }


        public async Task Add(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task<T?> Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {

            IQueryable<T> query;
            if (tracked)
            {
                query = dbSet;
            }
            else
            {
                query = dbSet.AsNoTracking();
            }

            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T?>> GetAllAsync(
     Expression<Func<T, bool>>? filter = null,
     string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToListAsync();
        }


        public async Task<T?> GetByIdAsync(string? id)
        {
            return await _db.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetByIdAsync(int? id)
        {
            return await _db.Set<T>().FindAsync(id);
        }

        public IQueryable<T?> GetList()
        {
            return _db.Set<T>();
        }

        public async Task RemoveAsync(T entity)
        {
            _db.Set<T>().Remove(entity);
            await _db.SaveChangesAsync();
        }
        public async Task RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
            await _db.SaveChangesAsync();
        }
    }
}
