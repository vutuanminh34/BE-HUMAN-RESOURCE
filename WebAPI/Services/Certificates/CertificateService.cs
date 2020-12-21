#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebAPI.Common;
using WebAPI.Constants;
using WebAPI.Models;
using WebAPI.Models.Resource.Certificate;
using WebAPI.Repositories.Certificates;
using WebAPI.RequestModel;
using WebAPI.ViewModels;

namespace WebAPI.Services.Certificates
{
    public class CertificateService : BaseService<CertificateViewModel<SaveCertificateResource>>, ICertificateService
    {
        private readonly ICertificateRepository _certificateRepository;
        public CertificateService(ICertificateRepository certificateRepository)
        {
            this._certificateRepository = certificateRepository;
        }

        /// <summary>
        /// Get all Certificate with personId
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SaveCertificateResource>> GetCertificateByPersonId(int personId)
        {
            var tempCertificateInfo = await _certificateRepository.GetCertificateByPersonIdAsync(personId);
            
            List<SaveCertificateResource> tempViewCertificateResource = new List<SaveCertificateResource>();
            foreach (var item in tempCertificateInfo)
            {
                var tempStartDate = string.Format(item.StartDate.Year.ToString());
                var tempEndDate = (item.EndDate is null) ? string.Empty : string.Format(item.EndDate?.Year.ToString());
                var tempSaveCertificateResource = new SaveCertificateResource()
                {
                    Id = item.Id,
                    Name = item.Name,
                    OrderIndex = item.OrderIndex,
                    Provider = item.Provider,
                    StartDate = tempStartDate,
                    EndDate = tempEndDate
                };
                tempViewCertificateResource.Add(tempSaveCertificateResource);
            }
            // Sort list
            var sortedList = tempViewCertificateResource?.OrderBy(x => x.OrderIndex);

            return sortedList;
        }

        /// <summary>
        /// Create a new Certificate
        /// </summary>
        /// <param name="certificateObj"></param>
        /// <returns></returns>
        public async Task<CertificateViewModel<SaveCertificateResource>> CreateCertificate(CreateCertificateResource certificateObj)
        {
            model.AppResult.Result = false;
            // Validate CollegeName, Major
            if (string.IsNullOrEmpty(certificateObj.Name) ||
                string.IsNullOrEmpty(certificateObj.Provider) ||
                certificateObj.Name.Length > 255 ||
                certificateObj.Provider.Length > 255)
            {
                model.AppResult.Message = "CollegeName/Major invalid";
                return model;
            }
            // Validate Start/End Date
            if (!Functions.ValidateInputTime(certificateObj.StartDate, certificateObj.EndDate))
            {
                model.AppResult.Message = Constant.DATETIME_ERROR;
                return model;
            }
            // Validate PersonId
            var taskValidPeronId = await _certificateRepository.ValidatePersonIdAsync(certificateObj.PersonId);
            if (taskValidPeronId < 1)
            {
                model.AppResult.Message = Constant.PERSONID_ERROR;
                return model;
            }

            // Define DateTime.Year = "1111" is null
            var valueStartDate = Functions.ConvertDateTime(certificateObj.StartDate);
            var valueEndDate = Functions.ConvertDateTime(certificateObj.EndDate);
            // Find maximum value of OrderIndex
            int maximumOrderIndex = await _certificateRepository.MaximumOrderIndexAsync(certificateObj.PersonId);
            maximumOrderIndex = (maximumOrderIndex <= 0) ? 1 : maximumOrderIndex + 1;
            // Insert infomation into temporary model
            var tempCertificateInfo = new CertificateInfo()
            {
                Name = Regex.Replace(certificateObj.Name.Trim(), @"\s{2,}", " "),
                Provider = Regex.Replace(certificateObj.Provider.Trim(), @"\s{2,}", " "),
                StartDate = valueStartDate,
                EndDate = (valueEndDate.Year == 1111) ? (DateTime?)null : valueEndDate, // Define DateTime.Year = "1111" is null
                PersonId = certificateObj.PersonId,
                UpdatedBy = null,
                UpdatedAt = null,
                CreatedBy = Helpers.HttpContext.CurrentUser,
                CreatedAt = DateTime.Now,
                Status = true,
                OrderIndex = maximumOrderIndex
            };
            var tempCertificateInfoId = await _certificateRepository.InsertAsync(tempCertificateInfo);
            // Set response to FE
            model.ObjModel.Id = tempCertificateInfoId;
            model.ObjModel.OrderIndex = tempCertificateInfo.OrderIndex;
            model.ObjModel.Name = tempCertificateInfo.Name;
            model.ObjModel.Provider = tempCertificateInfo.Provider;
            model.ObjModel.StartDate = tempCertificateInfo.StartDate.Year.ToString();
            model.ObjModel.EndDate = (tempCertificateInfo.EndDate is null) ? string.Empty : string.Format(tempCertificateInfo.EndDate?.Year.ToString());

            model.AppResult.Result = true;
            model.AppResult.Message = Constant.INSERT_SUCCESS;
            model.AppResult.DataResult = model.ObjModel;

            return model;
        }

        /// <summary>
        /// Swap orderIndex of Object
        /// </summary>
        /// <param name="swapId"></param>
        /// <returns></returns>
        public async Task<CertificateViewModel<SaveCertificateResource>> UpdateOrderIndexCertificate(SwapOrderIndexRequestModel swapId)
        {
            model.AppResult.Result = false;
            var tempCurrentCertificateInfo = await _certificateRepository.FindAsync(swapId.CurrentId);
            var tempTurnedCertificateInfo = await _certificateRepository.FindAsync(swapId.TurnedId);
            if (tempCurrentCertificateInfo is null || tempTurnedCertificateInfo is null)
            {
                model.AppResult.Message = Constant.SWAP_ERROR;
                return model;
            }

            // Set who edited?
            tempCurrentCertificateInfo.UpdatedBy = Helpers.HttpContext.CurrentUser;
            tempCurrentCertificateInfo.UpdatedAt = DateTime.Now;

            tempTurnedCertificateInfo.UpdatedBy = Helpers.HttpContext.CurrentUser;
            tempTurnedCertificateInfo.UpdatedAt = DateTime.Now;

            // Swap OrderIndex
            int tempOrderIndex = 0;
            tempOrderIndex = tempCurrentCertificateInfo.OrderIndex;
            tempCurrentCertificateInfo.OrderIndex = tempTurnedCertificateInfo.OrderIndex;
            tempTurnedCertificateInfo.OrderIndex = tempOrderIndex;
            // Execute swap
            var isSuccess = await _certificateRepository.SwapOrderIndexAsync(tempCurrentCertificateInfo, tempTurnedCertificateInfo);

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
        /// Update infomation of Certificate
        /// </summary>
        /// <param name="certificateObj"></param>
        /// <returns></returns>
        public async Task<CertificateViewModel<SaveCertificateResource>> UpdateInfomationCertificate(int certificateId, UpdateCertificateResource certificateObj)
        {
            model.AppResult.Result = false;
            // Validate Start/End Date
            if (!Functions.ValidateInputTime(certificateObj.StartDate, certificateObj.EndDate))
            {
                model.AppResult.Message = Constant.DATETIME_ERROR;
                return model;
            }
            // Validate Id of CertificateInfo
            var tempCertificateInfo = await _certificateRepository.FindAsync(certificateId);
            if (tempCertificateInfo is null)
            {
                model.AppResult.Message = Constant.PERSONID_ERROR;
                return model;
            }

            // Define DateTime.Year = "1111" is null
            var valueStartDate = Functions.ConvertDateTime(certificateObj.StartDate);
            var valueEndDate = Functions.ConvertDateTime(certificateObj.EndDate);

            // Set value into CertificateInfo
            tempCertificateInfo.Name = Regex.Replace(certificateObj.Name.Trim(), @"\s{2,}", " ");
            tempCertificateInfo.Provider = Regex.Replace(certificateObj.Provider.Trim(), @"\s{2,}", " ");
            tempCertificateInfo.StartDate = valueStartDate;
            tempCertificateInfo.EndDate = (valueEndDate.Year == 1111) ? (DateTime?)null : valueEndDate; // Define DateTime.Year = "1111" is null
            tempCertificateInfo.UpdatedBy = Helpers.HttpContext.CurrentUser;
            tempCertificateInfo.UpdatedAt = DateTime.Now;

            var isSuccess = await _certificateRepository.UpdateAsync(tempCertificateInfo);
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
        /// Delete Certificate
        /// </summary>
        /// <param name="certificateId"></param>
        /// <returns></returns>
        public async Task<CertificateViewModel<SaveCertificateResource>> DeleteCertificate(int certificateId)
        {
            model.AppResult.Result = false;
            // Validate Id of CertificateInfo
            var tempCertificateInfo = await _certificateRepository.FindAsync(certificateId);
            if (tempCertificateInfo is null)
            {
                model.AppResult.Message = Constant.PERSONID_ERROR;
                return model;
            }

            // Set value into CertificateInfo
            tempCertificateInfo.Status = false;
            tempCertificateInfo.UpdatedBy = Helpers.HttpContext.CurrentUser;
            tempCertificateInfo.UpdatedAt = DateTime.Now;

            var isSuccess = await _certificateRepository.UpdateAsync(tempCertificateInfo);
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
