

using Manager.Domain.Entities;

namespace Manager.Infra.Interfaces
{
    public interface IBaseRepository<T> where T : Base
    {
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task RemoveAsync(long id);
        Task<T?> GetAsync(long id);
        Task<List<T>> GetAllAsync();
    }
}
