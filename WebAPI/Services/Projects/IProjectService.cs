using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.RequestModel;
using WebAPI.ViewModels;

namespace WebAPI.Services.Projects
{
    public interface IProjectService
    {
        /// <summary>
        /// get all project
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Project>> GetAllProject();

        /// <summary>
        /// get project by personid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<Project>> GetProjectByPersonId(int id);

        /// <summary>
        /// get project by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Project> GetProjectById(int id);

        /// <summary>
        /// insert into project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public Task<ProjectViewModel> InsertProject(Project project);

        /// <summary>
        /// update project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public Task<ProjectViewModel> UpdateProject(Project project);
        /// <summary>
        /// delete project
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        public Task<ProjectViewModel> DeleteProject(int id);

        /// <summary>
        /// Swap orderIndex of project by projectId
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public Task<ProjectViewModel> SwapOderIndexProject(SwapOrderIndexRequestModel project);
        /// <summary>
        /// update with id in url
        /// </summary>
        /// <param name="id"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public Task<ProjectViewModel> UpdateProjectWithId(Project project);
    }
}
