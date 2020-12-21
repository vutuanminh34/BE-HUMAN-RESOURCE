using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Common;
using WebAPI.Constants;
using WebAPI.Models;
using WebAPI.Models.Resource.WorkHistory;
using WebAPI.Repositories.WorkHistories;
using WebAPI.RequestModel;
using WebAPI.ViewModels;

namespace WebAPI.Services.WorkHistories
{
    public class WorkHistoryService : BaseService<WorkHistoryViewModel>, IWorkHistoryService
    {
        IWorkHistoryRepository _workHistoryRepository;
        public WorkHistoryService(IWorkHistoryRepository workHistoryRepository)
        {
            this._workHistoryRepository = workHistoryRepository;
        }

        /// <summary>
        /// Get WorkHistory by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SaveWorkHistoryResource> GetWorkHistoryById(int id)
        {
            var tempWorkHistoryInfo = await _workHistoryRepository.FindAsync(id);
            var tempStartDate = string.Format(tempWorkHistoryInfo.StartDate.ToString("MM/yyyy"));
            var tempEndDate = (tempWorkHistoryInfo.EndDate is null) ? string.Empty : string.Format(tempWorkHistoryInfo.EndDate?.ToString("MM/yyyy"));
            var tempWorkHistory = new SaveWorkHistoryResource()
            {
                Id = tempWorkHistoryInfo.Id,
                Position = tempWorkHistoryInfo.Position,
                OrderIndex = tempWorkHistoryInfo.OrderIndex,
                CompanyName = tempWorkHistoryInfo.CompanyName,
                StartDate = tempStartDate,
                EndDate = tempEndDate,
                PersonId = tempWorkHistoryInfo.PersonId
            };
            return tempWorkHistory;
        }

        /// <summary>
        /// Get WorkHistory by PersonId
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SaveWorkHistoryResource>> GetWorkHistoryByPersonId(int id)
        {
            var tempWorkHistoryInfo = await _workHistoryRepository.FindAsyncByPersonId(id);
            // List is null
            List<SaveWorkHistoryResource> lists = new List<SaveWorkHistoryResource>();
            foreach (var item in tempWorkHistoryInfo)
            {
                var tempStartDate = string.Format(item.StartDate.ToString("MM/yyyy"));
                var tempEndDate = (item.EndDate is null) ? string.Empty : string.Format(item.EndDate?.ToString("MM/yyyy"));
                var tempWorkHistory = new SaveWorkHistoryResource()
                {
                    Id = item.Id,
                    Position = item.Position,
                    OrderIndex = item.OrderIndex,
                    CompanyName = item.CompanyName,
                    StartDate = tempStartDate,
                    EndDate = tempEndDate,
                    PersonId = item.PersonId
                };
                lists.Add(tempWorkHistory);
            }
            // Sort list
            var result = lists?.OrderBy(x => x.OrderIndex);
            return result;

        }

        /// <summary>
        /// Create WorkHistory 
        /// </summary>
        /// <param name="workHistoryObj"></param>
        /// <returns></returns>
        public async Task<WorkHistoryViewModel> CreateWorkHistory(CreateWorkHistoryResource workHistoryObj)
        {
            model.AppResult.Result = false;
            // Validate Start/End Date
            if (!Functions.ValidateInputTime(workHistoryObj.StartDate, workHistoryObj.EndDate))
            {
                model.AppResult.Message = Constant.DATETIME_ERROR;
                return model;
            }
            //Validate CompanyName
            if (String.IsNullOrEmpty(workHistoryObj.CompanyName) || workHistoryObj.CompanyName.Length > 255)
            {
                model.AppResult.Message = Constant.COMPANY_ERROR;
                return model;
            }
            //Validate Position
            if (String.IsNullOrEmpty(workHistoryObj.Position) || workHistoryObj.Position.Length > 255)
            {
                model.AppResult.Message = Constant.POSITION_ERROR;
                return model;
            }
            // Validate PersonId
            var taskValidPeronId = await _workHistoryRepository.ValidatePersonId(workHistoryObj.PersonId);
            if (taskValidPeronId < 1)
            {
                model.AppResult.Message = Constant.PERSONID_ERROR;
                return model;
            }
            // Define DateTime.Year = "1111" is null
            var valueStartDate = Functions.ConvertDateTime(workHistoryObj.StartDate);
            var valueEndDate = Functions.ConvertDateTime(workHistoryObj.EndDate);
            var tempWorkHistoryInfo = new WorkHistoryInfo()
            {
                Position = workHistoryObj.Position,
                CompanyName = workHistoryObj.CompanyName,
                StartDate = valueStartDate,
                EndDate = (valueEndDate.Year == 1111) ? (DateTime?)null : valueEndDate, // Define DateTime.Year = "1111" is null
                PersonId = workHistoryObj.PersonId,
                UpdatedBy = null,
                UpdatedAt = null,
                CreatedBy = Helpers.HttpContext.CurrentUser,
                CreatedAt = DateTime.Now
            };
            var id = await _workHistoryRepository.InsertAsync(tempWorkHistoryInfo);
            var result = await GetWorkHistoryById(id);
            model.AppResult.DataResult = result;
            model.AppResult.Result = true;
            model.AppResult.Message = Constant.INSERT_SUCCESS;
            return model;
        }

        /// <summary>
        /// Update WorkHistory For Controller
        /// </summary>
        /// <param name="workHistoryObj"></param>
        /// <returns></returns>
        public async Task<WorkHistoryViewModel> UpdateInfomationWorkHistory(int id, UpdateWorkHistoryResource workHistoryObj)
        {
            model.AppResult.Result = false;
            //Validate Status
            var workHistoryById = await GetWorkHistoryById(id);
            if (workHistoryById == null)
            {
                model.AppResult.Message = Constant.WORKHISTORY_ERROR;
                return model;
            }
            // Validate Start/End Date
            if (!Functions.ValidateInputTime(workHistoryObj.StartDate, workHistoryObj.EndDate))
            {
                model.AppResult.Message = Constant.DATETIME_ERROR;
                return model;
            }
            //Validate CompanyName
            if (String.IsNullOrEmpty(workHistoryObj.CompanyName) || workHistoryObj.CompanyName.Length > 255)
            {
                model.AppResult.Message = Constant.COMPANY_ERROR;
                return model;
            }
            //Validate Position
            if (String.IsNullOrEmpty(workHistoryObj.Position) || workHistoryObj.Position.Length > 255)
            {
                model.AppResult.Message = Constant.POSITION_ERROR;
                return model;
            }
            // Validate Id of WorkHistoryInfo
            var tempWorkHistoryInfo = await _workHistoryRepository.FindAsync(id);
            if (tempWorkHistoryInfo is null)
            {
                model.AppResult.Message = Constant.WORKHISTORY_ERROR;
                return model;
            }
            // Define DateTime.Year = "1111" is null
            var valueStartDate = Functions.ConvertDateTime(workHistoryObj.StartDate);
            var valueEndDate = Functions.ConvertDateTime(workHistoryObj.EndDate);
            // Set value into WorkHistoryInfo
            tempWorkHistoryInfo.Id = id;
            tempWorkHistoryInfo.Position = workHistoryObj.Position;
            tempWorkHistoryInfo.CompanyName = workHistoryObj.CompanyName;
            tempWorkHistoryInfo.StartDate = valueStartDate;
            tempWorkHistoryInfo.EndDate = (valueEndDate.Year == 1111) ? (DateTime?)null : valueEndDate; // Define DateTime.Year = "1111" is null
            tempWorkHistoryInfo.UpdatedAt = DateTime.Now;
            tempWorkHistoryInfo.UpdatedBy = Helpers.HttpContext.CurrentUser;
            model.AppResult.Message = Constant.UPDATE_SUCCESS;
            var result = await _workHistoryRepository.UpdateAsync(tempWorkHistoryInfo);
            model.AppResult.DataResult = result;
            model.AppResult.Result = true;
            return model;
        }
        /// <summary>
        /// Swap order index of Object
        /// </summary>
        /// <param name="swapId"></param>
        /// <returns></returns>
        public async Task<WorkHistoryViewModel> UpdateOrderIndexWorkHistory(SwapOrderIndexRequestModel swapId)
        {
            model.AppResult.Result = false;
            var tempCurrentWorkHistoryInfo = await _workHistoryRepository.FindAsync(swapId.CurrentId);
            var tempTurnedWorkHistoryInfo = await _workHistoryRepository.FindAsync(swapId.TurnedId);
            if (tempCurrentWorkHistoryInfo is null || tempTurnedWorkHistoryInfo is null)
            {
                model.AppResult.Message = Constant.WORKHISTORY_ERROR;
                return model;
            }

            int tempOrderIndex = -1;
            tempOrderIndex = tempCurrentWorkHistoryInfo.OrderIndex;
            tempCurrentWorkHistoryInfo.OrderIndex = tempTurnedWorkHistoryInfo.OrderIndex;
            tempTurnedWorkHistoryInfo.OrderIndex = tempOrderIndex;

            tempTurnedWorkHistoryInfo.UpdatedAt = DateTime.Now;
            tempCurrentWorkHistoryInfo.UpdatedAt = DateTime.Now;

            var tempCurrent = await _workHistoryRepository.UpdateAsync(tempCurrentWorkHistoryInfo);
            var tempTurned = await _workHistoryRepository.UpdateAsync(tempTurnedWorkHistoryInfo);
            
            if (tempCurrent > 0 && tempTurned > 0)
            {
                model.AppResult.Message = Constant.SWAP_SUCCESS;
                model.AppResult.Result = true;
                return model;
            }
            model.AppResult.Message = Constant.SWAP_ERROR;
            return model;
        }

        /// <summary>
        /// Delete WorkHistory
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<WorkHistoryViewModel> DeleteWorkHistory(int id)
        {
            model.AppResult.Result = false;
            //Validate Status
            var workHistoryById = await GetWorkHistoryById(id);
            if (workHistoryById == null)
            {
                model.AppResult.Message = Constant.WORKHISTORY_ERROR;
                return model;
            }
            var result = await _workHistoryRepository.DeleteAsync(id);
            if (result > 0)
            {
                model.AppResult.Result = true;
                model.AppResult.Message = Constant.DELETE_SUCCESS;
                return model;
                
            }
            model.AppResult.Message = Constant.DELETE_FAIL;
            return model;
        }
    }
}
