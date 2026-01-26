using BillingSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BillingSystem.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetAllQueryable();
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        void SoftDelete(T entity);
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly HospitalDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(HospitalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public IQueryable<T> GetAllQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void SoftDelete(T entity)
        {
            // Check if entity has IsActive property
            var property = typeof(T).GetProperty("IsActive");
            if (property != null && property.PropertyType == typeof(bool))
            {
                property.SetValue(entity, false);
                _dbSet.Update(entity);
            }
            else
            {
                // Fallback to hard delete if not supported, or throw exception? 
                // Given "Soft delete flags" requirement, better to just Remove if strictly required, 
                // but usually soft delete implies "try soft, if not, do nothing or hard". 
                // For safety in this prompt context, let's Remove if IsActive doesn't exist, 
                // effectively treating it as a delete operation.
                _dbSet.Remove(entity);
            }
        }
    }
}
