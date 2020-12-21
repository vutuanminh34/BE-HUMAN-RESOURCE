using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Models.Resource.Skill;
using WebAPI.Models.Resource.Technology;
using WebAPI.RequestModel;
using WebAPI.ViewModels;

namespace WebAPI.Services.Skills
{
    public interface ISkillService
    {
        /// <summary>
        /// Get Skill By PersonId
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        Task<IEnumerable<SkillResource>> GetSkillByPerson(int personId);

        /// <summary>
        /// Inser Skill
        /// </summary>
        /// <param name="skillRequestModel"></param>
        /// <returns></returns>
        Task<SkillViewModel> InserSkill(SkillRequestModel skillRequestModel);


        /// <summary>
        /// Update Skill
        /// </summary>
        /// <param name="skillRequestModel"></param>
        /// <returns></returns>
        Task<SkillViewModel> UpdateSkill(int id, SkillRequestModel skillRequestModel);

        /// <summary>
        /// Delete Skill 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SkillViewModel> DeleteSkill(int id);

        /// <summary>
        /// Get Skill By Category
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<IEnumerable<TechnologyResource>> GetSkillByCategory(int personId, int categoryId);

        /// <summary>
        /// Swap OrderIndex
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<SkillViewModel> SwapOrderIndex(SwapOrderIndexRequestModel model);

        
    }
}
