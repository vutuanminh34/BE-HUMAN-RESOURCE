using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Repositories.Interfaces
{
    public interface IRepositoryBase<T>
    {
        /// <summary>
        /// Get all Entity
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Find Entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> FindAsync(int id);

        /// <summary>
        /// Insert Entity
        /// </summary>
        /// <param name="entity"></param>
        Task<int> InsertAsync(T entity);

        /// <summary>
        /// Update Entity
        /// </summary>
        /// <param name="entityToUpdate"></param>
        Task<int> UpdateAsync(T entity);

        /// <summary>
        /// Delete Entity
        /// </summary>
        /// <param name="id"></param>
        Task<int> DeleteAsync(int id);


    }
}
