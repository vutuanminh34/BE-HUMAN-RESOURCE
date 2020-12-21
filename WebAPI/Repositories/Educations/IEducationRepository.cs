using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Repositories.Interfaces;

namespace WebAPI.Repositories.Educations
{
    public interface IEducationRepository : IRepositoryBase<EducationInfo>
    {
        Task<IEnumerable<EducationInfo>> GetEducationByPersonIdAsync(int personId, bool flag = true);

        Task<int> MaximumOrderIndexAsync(int personId, bool flag = true);

        Task<int> ValidatePersonIdAsync(int personId, bool flag = true);

        Task<bool> SwapOrderIndexAsync(EducationInfo currentObj, EducationInfo turnedObj);
    }
}
