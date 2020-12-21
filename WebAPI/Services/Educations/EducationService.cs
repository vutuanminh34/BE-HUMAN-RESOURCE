#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebAPI.Common;
using WebAPI.Constants;
using WebAPI.Models;
using WebAPI.Models.Resource.Education;
using WebAPI.Repositories.Educations;
using WebAPI.RequestModel;
using WebAPI.ViewModels;

namespace WebAPI.Services.Educations
{
    public class EducationService : BaseService<EducationViewModel<SaveEducationResource>>, IEducationService
    {
        private readonly IEducationRepository _educationRepository;
        public EducationService(IEducationRepository educationRepository)
        {
            this._educationRepository = educationRepository;
        }

        /// <summary>
        /// Get all Education with personId
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SaveEducationResource>> GetEducationByPersonId(int personId)
        {
            var tempEducationInfo = await _educationRepository.GetEducationByPersonIdAsync(personId);

            List<SaveEducationResource> tempViewEducationResource = new List<SaveEducationResource>();
            foreach (var item in tempEducationInfo)
            {
                var tempStartDate = string.Format(item.StartDate.Year.ToString());
                var tempEndDate = (item.EndDate is null) ? string.Empty : string.Format(item.EndDate?.Year.ToString());
                var tempSaveEducationResource = new SaveEducationResource()
                {
                    Id = item.Id,
                    CollegeName = item.CollegeName,
                    OrderIndex = item.OrderIndex,
                    Major = item.Major,
                    StartDate = tempStartDate,
                    EndDate = tempEndDate
                };
                tempViewEducationResource.Add(tempSaveEducationResource);
            }
            // Sort list
            var sortedList = tempViewEducationResource?.OrderBy(x => x.OrderIndex);
            
            return sortedList;
        }

        /// <summary>
        /// Create a new Education
        /// </summary>
        /// <param name="educationObj"></param>
        /// <returns></returns>
        public async Task<EducationViewModel<SaveEducationResource>> CreateEducation(CreateEducationResource educationObj)
        {
            model.AppResult.Result = false;
            // Validate CollegeName, Major
            if (string.IsNullOrEmpty(educationObj.CollegeName) ||
                string.IsNullOrEmpty(educationObj.Major) ||
                educationObj.Major.Length > 255 ||
                educationObj.CollegeName.Length > 255)
            {
                model.AppResult.Message = "CollegeName/Major invalid";
                return model;
            }
            // Validate Start/End Date
            if (!Functions.ValidateInputTime(educationObj.StartDate, educationObj.EndDate))
            {
                model.AppResult.Message = Constant.DATETIME_ERROR;
                return model;
            }
            // Validate PersonId
            var taskValidPeronId = await _educationRepository.ValidatePersonIdAsync(educationObj.PersonId);
            if (taskValidPeronId < 1)
            {
                model.AppResult.Message = Constant.PERSONID_ERROR;
                return model;
            }

            // Define DateTime.Year = "1111" is null
            var valueStartDate = Functions.ConvertDateTime(educationObj.StartDate);
            var valueEndDate = Functions.ConvertDateTime(educationObj.EndDate);
            // Find maximum value of OrderIndex
            int maximumOrderIndex = await _educationRepository.MaximumOrderIndexAsync(educationObj.PersonId);
            maximumOrderIndex = (maximumOrderIndex <= 0) ? 1 : maximumOrderIndex + 1;
            // Insert info into temporary model
            var tempEducationInfo = new EducationInfo()
            {
                CollegeName = Regex.Replace(educationObj.CollegeName.Trim(), @"\s{2,}", " "),
                Major = Regex.Replace(educationObj.Major.Trim(), @"\s{2,}", " "),
                StartDate = valueStartDate,
                EndDate = (valueEndDate.Year == 1111) ? (DateTime?)null : valueEndDate, // Define DateTime.Year = "1111" is null
                PersonId = educationObj.PersonId,
                UpdatedBy = null,
                UpdatedAt = null,
                CreatedBy = Helpers.HttpContext.CurrentUser,
                CreatedAt = DateTime.Now,
                Status = true,
                OrderIndex = maximumOrderIndex
            };
            var tempEducationInfoId = await _educationRepository.InsertAsync(tempEducationInfo);
            // Set response to FE
            model.ObjModel.Id = tempEducationInfoId;
            model.ObjModel.OrderIndex = tempEducationInfo.OrderIndex;
            model.ObjModel.CollegeName = tempEducationInfo.CollegeName;
            model.ObjModel.Major = tempEducationInfo.Major;
            model.ObjModel.StartDate = tempEducationInfo.StartDate.Year.ToString();
            model.ObjModel.EndDate = (tempEducationInfo.EndDate is null) ? string.Empty : string.Format(tempEducationInfo.EndDate?.Year.ToString());

            model.AppResult.Result = true;
            model.AppResult.Message = Constant.INSERT_SUCCESS;
            model.AppResult.DataResult = model.ObjModel;

            return model;
        }

        /// <summary>
        /// Swap OrderIndex value of Object
        /// </summary>
        /// <param name="swapId"></param>
        /// <returns></returns>
        public async Task<EducationViewModel<SaveEducationResource>> UpdateOrderIndexEducation(SwapOrderIndexRequestModel swapId)
        {
            model.AppResult.Result = false;
            var tempCurrentEducationInfo = await _educationRepository.FindAsync(swapId.CurrentId);
            var tempTurnedEducationInfo = await _educationRepository.FindAsync(swapId.TurnedId);
            if (tempCurrentEducationInfo is null || tempTurnedEducationInfo is null)
            {
                model.AppResult.Message = Constant.SWAP_ERROR;
                return model;
            }

            // Set who edited?
            tempCurrentEducationInfo.UpdatedBy = Helpers.HttpContext.CurrentUser;
            tempCurrentEducationInfo.UpdatedAt = DateTime.Now;

            tempTurnedEducationInfo.UpdatedBy = Helpers.HttpContext.CurrentUser;
            tempTurnedEducationInfo.UpdatedAt = DateTime.Now;

            // Swap orderIndex
            int tempOrderIndex = 0;
            tempOrderIndex = tempCurrentEducationInfo.OrderIndex;
            tempCurrentEducationInfo.OrderIndex = tempTurnedEducationInfo.OrderIndex;
            tempTurnedEducationInfo.OrderIndex = tempOrderIndex;
            // Execute swap
            var isSuccess = await _educationRepository.SwapOrderIndexAsync(tempCurrentEducationInfo, tempTurnedEducationInfo);

            if (isSuccess)
            {
                model.AppResult.Result = true;
                model.AppResult.Message = Constant.SWAP_SUCCESS;
                return model;
            }
            model.AppResult.Message = "Taylor Swift";
            return model;
        }

        /// <summary>
        /// Update infomation of Education
        /// </summary>
        /// <param name="educationObj"></param>
        /// <returns></returns>
        public async Task<EducationViewModel<SaveEducationResource>> UpdateInfomationEducation(int educationId, UpdateEducationResource educationObj)
        {
            model.AppResult.Result = false;
            // Validate Start/End Date
            if (!Functions.ValidateInputTime(educationObj.StartDate, educationObj.EndDate))
            {
                model.AppResult.Message = Constant.DATETIME_ERROR;
                return model;
            }
            // Validate Id of EducationInfo
            var tempEducationInfo = await _educationRepository.FindAsync(educationId);
            if (tempEducationInfo is null)
            {
                model.AppResult.Message = Constant.PERSONID_ERROR;
                return model;
            }

            // Define DateTime.Year = "1111" is null
            var valueStartDate = Functions.ConvertDateTime(educationObj.StartDate);
            var valueEndDate = Functions.ConvertDateTime(educationObj.EndDate);

            // Set value into EducationInfo
            tempEducationInfo.CollegeName = Regex.Replace(educationObj.CollegeName.Trim(), @"\s{2,}", " ");
            tempEducationInfo.Major = Regex.Replace(educationObj.Major.Trim(), @"\s{2,}", " ");
            tempEducationInfo.StartDate = valueStartDate;
            tempEducationInfo.EndDate = (valueEndDate.Year == 1111) ? (DateTime?)null : valueEndDate; // Define DateTime.Year = "1111" is null
            tempEducationInfo.UpdatedBy = Helpers.HttpContext.CurrentUser;
            tempEducationInfo.UpdatedAt = DateTime.Now;

            var isSuccess = await _educationRepository.UpdateAsync(tempEducationInfo);
            if (isSuccess > 0)
            {
                model.AppResult.Result = true;
                model.AppResult.Message = Constant.UPDATE_SUCCESS;
                return model;
            }
            model.AppResult.Message = "Bad Request";
            return model;
        }

        /// <summary>
        /// Delete Education
        /// </summary>
        /// <param name="educationId"></param>
        /// <returns></returns>
        public async Task<EducationViewModel<SaveEducationResource>> DeleteEducation(int educationId)
        {
            model.AppResult.Result = false;
            // Validate Id of EducationInfo
            var tempEducationInfo = await _educationRepository.FindAsync(educationId);
            if (tempEducationInfo is null)
            {
                model.AppResult.Message = Constant.PERSONID_ERROR;
                return model;
            }

            // Set value into EducationInfo
            tempEducationInfo.Status = false;
            tempEducationInfo.UpdatedAt = DateTime.Now;
            tempEducationInfo.UpdatedBy = Helpers.HttpContext.CurrentUser;

            var isSuccess = await _educationRepository.UpdateAsync(tempEducationInfo);
            if (isSuccess > 0)
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
