using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Models.Resource.Education;
using WebAPI.Models.Resource.WorkHistory;
using WebAPI.RequestModel;
using WebAPI.ViewModels;

namespace WebAPI.Services.WorkHistories
{
    public interface IWorkHistoryService 
    {
        /// <summary>
        /// Get WorkHistory by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SaveWorkHistoryResource> GetWorkHistoryById(int id);

        /// <summary>
        /// Get WorkHistory by PersonId
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        Task<IEnumerable<SaveWorkHistoryResource>> GetWorkHistoryByPersonId(int id);

        /// <summary>
        /// Create WorkHistory
        /// </summary>
        /// <param name="workHistoryObj"></param>
        /// <returns></returns>
        Task<WorkHistoryViewModel> CreateWorkHistory(CreateWorkHistoryResource workHistoryObj);

        /// <summary>
        /// Update Info WorkHistory
        /// </summary>
        /// <param name="workHistoryObj"></param>
        /// <returns></returns>
        Task<WorkHistoryViewModel> UpdateInfomationWorkHistory(int id , UpdateWorkHistoryResource workHistoryObj);

        /// <summary>
        /// Swap order index of Object
        /// </summary>
        /// <param name="swapId"></param>
        /// <returns></returns>
        Task<WorkHistoryViewModel> UpdateOrderIndexWorkHistory(SwapOrderIndexRequestModel swapId);

        /// <summary>
        /// Delete WorkHistory
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<WorkHistoryViewModel> DeleteWorkHistory(int id);
    }
}
