using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Repositories.ProjectTechnologies;
using WebAPI.RequestModel;
using WebAPI.ViewModels;
using WebAPI.Constants;


namespace WebAPI.Services.ProjectTechnologies
{
    public class ProjectTechnologyService : BaseService<ProjectTechnologyViewModel>, IProjectTechnologyService
    {
        IProjectTechnologyRepository _projectTechnologyRepository;

        public ProjectTechnologyService(IProjectTechnologyRepository projectTechnologyRepository)
        {
            this._projectTechnologyRepository = projectTechnologyRepository;
        }

        public async Task<ProjectTechnologyViewModel> DeleteProjectTechnology(int id)
        {
            await _projectTechnologyRepository.DeleteAsync(id);
            return model;
        }

        public async Task<ProjectTechnologyViewModel> InsertListTechnologyAsync(List<ProjectTechnology> entities)
        {
            foreach (var item in entities)
            {
                int ProjectId = await _projectTechnologyRepository.CountProject(item.ProjectId);
                int TechnologyId = await _projectTechnologyRepository.CountTechnology(item.TechnologyId);
                int PersonId = await _projectTechnologyRepository.GetPersonId(item.ProjectId);
                int CheckIndex = await _projectTechnologyRepository.CheckTechnologyInPerson(PersonId, item.TechnologyId);
                if (ProjectId <= 0)
                {
                    model.AppResult.Message = Constant.PROJECT_ERROR;
                    model.AppResult.DataResult = null;
                    return model;
                }
                if (TechnologyId <= 0)
                {
                    model.AppResult.Message = Constant.TECHNOLOGY_ERROR; 
                    model.AppResult.DataResult = null;
                    return model;
                }
                if (CheckIndex <= 0)
                {
                    model.AppResult.Message = Constant.CHECKTECHNOLOGY_ERROR; 
                    model.AppResult.DataResult = null;
                    return model;
                }
            }
            await _projectTechnologyRepository.InsertListTechnologyAsync(entities);
            var result = await _projectTechnologyRepository.GetListTechnology(entities.FirstOrDefault().ProjectId);
            model.AppResult.Message = Constant.INSERT_SUCCESS;
            model.AppResult.DataResult = result;
            return model;
        }


        public async Task<ProjectTechnologyViewModel> UpdateListTechnologyAsync(int id, List<ProjectTechnology> entities)
        {
            foreach (var item in entities)
            {
                int ProjectId = await _projectTechnologyRepository.CountProject(id);
                int TechnologyId = await _projectTechnologyRepository.CountTechnology(item.TechnologyId);
                int PersonId = await _projectTechnologyRepository.GetPersonId(id);
                int CheckIndex = await _projectTechnologyRepository.CheckTechnologyInPerson(PersonId, item.TechnologyId);
                if (ProjectId <= 0)
                {
                    model.AppResult.Message = Constant.PROJECT_ERROR;
                    model.AppResult.DataResult = null;
                    return model;
                }
                if (TechnologyId <= 0)
                {
                    model.AppResult.Message = Constant.TECHNOLOGY_ERROR;
                    model.AppResult.DataResult = null;
                    return model;
                }
                if (CheckIndex <= 0)
                {
                    model.AppResult.Message = Constant.CHECKTECHNOLOGY_ERROR;
                    model.AppResult.DataResult = null;
                    return model;
                }
            }
            await _projectTechnologyRepository.UpdateListTechnologyAsync(id, entities);
            var result = await _projectTechnologyRepository.GetListTechnology(id);
            model.AppResult.Message = Constant.UPDATE_SUCCESS;
            model.AppResult.DataResult = result;
            return model;
        }
    }
}
