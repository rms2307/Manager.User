using Manager.Domain.Entities;
using Manager.Infra.Context;
using Manager.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Manager.Infra.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : Base
    {
        private readonly ManagerContext _context;

        public BaseRepository(ManagerContext context)
        {
            _context = context;
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async virtual Task<List<T>> GetAllAsync()
            => await _context.Set<T>().AsNoTracking().ToListAsync();

        public virtual async Task<T?> GetAsync(long id)
            => await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public virtual async Task RemoveAsync(long id)
        {
            T? entity = await GetAsync(id);
            if (entity is not null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async virtual Task<T> UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
