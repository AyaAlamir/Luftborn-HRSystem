using System.Collections.Generic;
using System.Threading.Tasks;
using HRSystem.Domain.Entities.Base;
using HRSystem.Domain.Interfaces.Base;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Infrastructure.Persistence
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public Task<T> GetByIdAsync(int id)
        {
            return _dbSet.FirstOrDefaultAsync(item => item.Id == id);
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                return;
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
