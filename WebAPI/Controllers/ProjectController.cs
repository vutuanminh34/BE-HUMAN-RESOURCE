using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.RequestModel;
using WebAPI.Services.Projects;
using WebAPI.ViewModels;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : BaseApiController<ProjectViewModel>
    {
        private IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            this._projectService = projectService;
        }

        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpGet]
        public async Task<IActionResult> GetAllProject()
        {
            var projects = await _projectService.GetAllProject();
            apiResult.AppResult.DataResult = projects.AsEnumerable<Project>();
            return Ok(apiResult.AppResult);
        }

        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var app = await _projectService.GetProjectById(id);
            if (app == null)
            {
                return BadRequest();
            }
            apiResult.AppResult.DataResult = app;
            return Ok(apiResult.AppResult);
        }

        /// <summary>
        /// get all project by personid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpGet("person/{id}")]
        public async Task<IActionResult> GetProjectByPersonId(int id)
        {
            var app = await _projectService.GetProjectByPersonId(id);
            apiResult.AppResult.DataResult = app;
            return Ok(apiResult.AppResult);
        }

        /// <summary>
        /// insert infor project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPost]
        public async Task<IActionResult> InsertProject([FromBody] Project project)
        {
            var app = await _projectService.InsertProject(project);
            if (app.AppResult.DataResult == null)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }

        /// <summary>
        /// using for updating orderindex only
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPut]
        public async Task<IActionResult> UpdateProject([FromBody] Project project)
        {
            var app = await _projectService.UpdateProject(project);
            return Ok(app.AppResult);
        }

        /// <summary>
        /// update infor project
        /// </summary>
        /// <param name="id"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProjectWithId(int id, [FromBody] Project project)
        {
            project.Id = id;
            var app = await _projectService.UpdateProjectWithId(project);
            if (app.AppResult.DataResult == null)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }

        /// <summary>
        /// swap orderindex in project by currentid and turnedid
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPut("swap")]
        public async Task<IActionResult> SwapOrderIndexProject([FromBody] SwapOrderIndexRequestModel project)
        {
            
            var app = await _projectService.SwapOderIndexProject(project);
            if (!app.AppResult.Result)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }

        /// <summary>
        /// delete project by projectid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var app = await _projectService.DeleteProject(id);
            if (app.AppResult.DataResult == null)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }
    }
}
