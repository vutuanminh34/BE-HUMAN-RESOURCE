using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Repositories.Interfaces;

namespace WebAPI.Repositories.WorkHistories
{
    public interface IWorkHistoryRepository : IRepositoryBase<WorkHistoryInfo>
    {
        /// <summary>
        /// Find Entity
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        Task<IEnumerable<WorkHistoryInfo>> FindAsyncByPersonId(int id);

        /// <summary>
        /// Find maximum value of OrderIndex in table WorkHistory
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        Task<int> MaximumOrderIndexAsync(int personId);

        /// <summary>
        /// Validate PersonId
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        Task<int> ValidatePersonId(int personId);

    }
}
