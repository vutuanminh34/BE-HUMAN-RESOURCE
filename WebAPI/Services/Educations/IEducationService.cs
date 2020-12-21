using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Models.Resource.Education;
using WebAPI.RequestModel;
using WebAPI.ViewModels;

namespace WebAPI.Services.Educations
{
    public interface IEducationService
    {
        Task<IEnumerable<SaveEducationResource>> GetEducationByPersonId(int personId);

        Task<EducationViewModel<SaveEducationResource>> CreateEducation(CreateEducationResource educationObj);

        Task<EducationViewModel<SaveEducationResource>> UpdateOrderIndexEducation(SwapOrderIndexRequestModel swapId);

        Task<EducationViewModel<SaveEducationResource>> UpdateInfomationEducation(int educationId, UpdateEducationResource educationObj);

        Task<EducationViewModel<SaveEducationResource>> DeleteEducation(int educationId);
    }
}
