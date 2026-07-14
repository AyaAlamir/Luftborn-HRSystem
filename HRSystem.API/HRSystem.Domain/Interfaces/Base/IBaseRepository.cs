using System.Collections.Generic;
using System.Threading.Tasks;
using HRSystem.Domain.Entities.Base;

namespace HRSystem.Domain.Interfaces.Base
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}
