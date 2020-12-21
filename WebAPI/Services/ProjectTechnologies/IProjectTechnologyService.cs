using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.RequestModel;
using WebAPI.ViewModels;

namespace WebAPI.Services.ProjectTechnologies
{
    public interface IProjectTechnologyService
    {
        /// <summary>
        /// insert list id technology into projectTechnology table
        /// </summary>
        /// <param name="enitities"></param>
        /// <returns></returns>
        public Task<ProjectTechnologyViewModel> InsertListTechnologyAsync(List<ProjectTechnology> enitities);

        public Task<ProjectTechnologyViewModel> UpdateListTechnologyAsync(int id, List<ProjectTechnology> entities);

        public Task<ProjectTechnologyViewModel> DeleteProjectTechnology(int id);

    }
}
