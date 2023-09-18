using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace todolistasp.Repository.BaseRepository
{
    public interface IBaseRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync(GetAllQueryOptions options);
        Task<T?> GetByIdAsync(int id);
        Task<T> AddOneAsync(T data);
        Task<T> UpdateOneAsync(T data);
        Task<bool> DeleteOneAsync(int id);
    }
}