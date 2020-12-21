using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Repositories.Interfaces;

namespace WebAPI.Repositories.Projects
{
    public interface IProjectRepository : IRepositoryBase<Project>
    {
        Task<IEnumerable<Project>> GetProjectByPersonIdAsync(int id);

        Task<int> CountProject(int Id);

        Task<int> CountPerson(int Id);

        Task<int> UpdateProjectWithIdAsync(Project project);
    }
}
