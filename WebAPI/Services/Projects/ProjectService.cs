#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Repositories.Projects;
using WebAPI.RequestModel;
using WebAPI.ViewModels;
using WebAPI.Common;
using WebAPI.Constants;

namespace WebAPI.Services.Projects
{
    public class ProjectService : BaseService<ProjectViewModel>, IProjectService
    {
        IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            this._projectRepository = projectRepository;
        }
        public async Task<ProjectViewModel> DeleteProject(int id)
        {
            var project = await GetProjectById(id);
            if (project is null)
            {
                model.AppResult.Message = Constant.ID_ERROR;
                return model;
            }
            else if (project.Status == false)
            {
                model.AppResult.Message = Constant.PROJECT_ERROR;
                return model;
            }
            else
            {
                await _projectRepository.DeleteAsync(id);
                model.AppResult.Message = Constant.DELETE_SUCCESS; 
                model.AppResult.DataResult = "OK";
                return model;
            }
        }

        public async Task<IEnumerable<Project>> GetAllProject()
        {
            return await _projectRepository.GetAllAsync();
        }

        public async Task<Project> GetProjectById(int id)
        {
            return await _projectRepository.FindAsync(id);
        }

        public async Task<IEnumerable<Project>> GetProjectByPersonId(int id)
        {
            return await _projectRepository.GetProjectByPersonIdAsync(id);
        }

        public async Task<ProjectViewModel> InsertProject(Project project)
        {
            model.Project = project;
            int count = await _projectRepository.CountPerson(project.PersonId);
            if (count <= 0)
            {
                model.AppResult.Message = Constant.PERSONID_ERROR;
                return model;
            }
            else if (project.TeamSize <= 0)
            {
                model.AppResult.Message = Constant.TEAMSIZE_ERROR;
                return model;
            }
            else if (String.IsNullOrEmpty(project.Name) || project.Name.Length > 255)
            {
                model.AppResult.Message = Constant.NAME_ERROR;
                return model;
            }
            else if (String.IsNullOrEmpty(project.Description))
            {
                model.AppResult.Message = Constant.DESCRIPTION_ERROR;
                return model;
            }
            else if (String.IsNullOrEmpty(project.Position) || project.Position.Length > 255)
            {
                model.AppResult.Message = Constant.POSITION_ERROR;
                return model;
            }
            else if (String.IsNullOrEmpty(project.Responsibilities) || project.Responsibilities.Length > 255)
            {
                model.AppResult.Message = Constant.RESPONSIBILITIES_ERROR;
                return model;
            }
            else if (!project.CheckDateTime())
            {
                model.AppResult.Message = Constant.DATETIME_ERROR;
                return model;
            }
            else
            {
                model.Project.CreatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
                model.Project.CreatedAt = DateTime.Now;
                var id = await _projectRepository.InsertAsync(model.Project);
                var data = await GetProjectById(id);
                model.Project.Id = id;
                model.AppResult.Message = Constant.INSERT_SUCCESS; 
                model.AppResult.DataResult = data;
                return model;
            }
        }

        public async Task<ProjectViewModel> SwapOderIndexProject(SwapOrderIndexRequestModel project)
        {
            var CurrentProject = await GetProjectById(project.CurrentId);
            var TurnedProject = await GetProjectById(project.TurnedId);

            if (CurrentProject is null || TurnedProject is null) 
            {
                model.AppResult.Result = false; 
                model.AppResult.Message = Constant.SWAP_ERROR;
                return model;
            }

            CurrentProject.UpdatedBy = HttpContext.CurrentUser;
            CurrentProject.UpdatedAt = DateTime.Now;
            TurnedProject.UpdatedBy = HttpContext.CurrentUser;
            TurnedProject.UpdatedAt = DateTime.Now;
            
            int OrderIndex = -1;
            OrderIndex = CurrentProject.OrderIndex;
            CurrentProject.OrderIndex = TurnedProject.OrderIndex;
            TurnedProject.OrderIndex = OrderIndex;

            var Current = await UpdateProject(CurrentProject);
            var Turned = await UpdateProject(TurnedProject);

            
            if (Current.AppResult.Result && Turned.AppResult.Result) 
            {
                model.AppResult.Result = true;
                model.AppResult.Message = Constant.SWAP_SUCCESS;
                return model;
            }
            return model;
        }

        public async Task<ProjectViewModel> UpdateProject(Project project)
        {
            model.Project = project;
            model.Project.UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser;
            model.Project.UpdatedAt = DateTime.Now;
            var tempInt = await _projectRepository.UpdateAsync(model.Project);
            if (tempInt > 0)
            {
                model.AppResult.Result = true;
            }
            else
            {
                model.AppResult.Result = false;
            } 
            return model;

        }

        public async Task<ProjectViewModel> UpdateProjectWithId(Project project)
        {
            model.Project = project;
            model.Project.UpdatedBy = HttpContext.CurrentUser;
            model.Project.UpdatedAt = DateTime.Now;
            var index = await GetProjectById(project.Id);
            if (index is null)
            {
                model.AppResult.Message = Constant.ID_ERROR;
                return model;
            }
            else if (project.TeamSize <= 0)
            {
                model.AppResult.Message = Constant.TEAMSIZE_ERROR;
                return model;
            }
            else if (String.IsNullOrEmpty(project.Name) || project.Name.Length > 255)
            {
                model.AppResult.Message = Constant.NAME_ERROR;
                return model;
            }
            else if (String.IsNullOrEmpty(project.Description))
            {
                model.AppResult.Message = Constant.DESCRIPTION_ERROR;
                return model;
            }
            else if (String.IsNullOrEmpty(project.Position) || project.Position.Length > 255)
            {
                model.AppResult.Message = Constant.POSITION_ERROR;
                return model;
            }
            else if (String.IsNullOrEmpty(project.Responsibilities) || project.Responsibilities.Length > 255)
            {
                model.AppResult.Message = Constant.RESPONSIBILITIES_ERROR;
                return model;
            }else if(!project.CheckDateTime())
            {
                model.AppResult.Message = Constant.DATETIME_ERROR;
                return model;
            }
            else
            {
                var result = await _projectRepository.UpdateProjectWithIdAsync(model.Project);
                model.AppResult.Message = Constant.UPDATE_SUCCESS; 
                model.AppResult.DataResult = result;
                return model;
            }
        }
    }
}
