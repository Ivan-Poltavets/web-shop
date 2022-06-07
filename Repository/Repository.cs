using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.Repository
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        private readonly ApplicationContext _context;
        private readonly DbSet<T> _dbEntity;

        public Repository(ApplicationContext context)
        {
            _context = context;
            _dbEntity = _context.Set<T>();
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _dbEntity.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbEntity.FindAsync(id);
            _dbEntity.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbEntity.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbEntity.FindAsync(id);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbEntity.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
