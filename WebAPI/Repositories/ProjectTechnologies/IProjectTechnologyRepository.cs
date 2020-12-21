using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Repositories.Interfaces;

namespace WebAPI.Repositories.ProjectTechnologies
{
    public interface IProjectTechnologyRepository : IRepositoryBase<ProjectTechnology>
    {
        /// <summary>
        /// insert list technology
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<int> InsertListTechnologyAsync(List<ProjectTechnology> entities);

        Task<int> UpdateListTechnologyAsync(int id, List<ProjectTechnology> entities);

        Task<IEnumerable<Technology>> GetListTechnology(int id);

        Task<int> CountProject(int id);

        Task<int> CountTechnology(int id);

        Task<int> CheckTechnologyInPerson(int PersonId, int TechnologyId);

        Task<int> GetPersonId(int id);
    }
}
