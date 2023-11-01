using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
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

        public async Task CreateAsync(T entity)
        {
            await _db.AddAsync(entity);
            await SaveAsync();

        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if(includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries)) {

                    query = query.Include(includeProp);
                }
                
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, int pageSize = 2, int pageNumber = 1)
        {
            IQueryable<T> query = dbSet;
            if(includeProperties != null)
            {
                foreach(var includeProp in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if(pageSize > 0)
            {
                if(pageSize > 100)
                {
                    pageSize = 100;
                }

                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }

            return await query.ToListAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            _db.Remove(entity);
            await SaveAsync();
        }

       

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
