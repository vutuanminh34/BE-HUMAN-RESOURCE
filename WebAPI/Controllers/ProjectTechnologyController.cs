using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Services.ProjectTechnologies;
using WebAPI.ViewModels;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectTechnologyController : BaseApiController<ProjectTechnologyViewModel>
    {
        public IProjectTechnologyService _projectTechnologyService;

        public ProjectTechnologyController(IProjectTechnologyService projectTechnologyService)
        {
            this._projectTechnologyService = projectTechnologyService;
        }

        /// <summary>
        /// insert technology in project
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPost]
        public async Task<IActionResult> InsertListTechnology(List<ProjectTechnology> entities)
        {
            var tech = await _projectTechnologyService.InsertListTechnologyAsync(entities);
            if(tech.AppResult.DataResult == null)
            {
                return BadRequest(tech.AppResult);
            }
            return Ok(tech.AppResult);
        }

        /// <summary>
        /// update technology in project
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateListTechnology(int id, List<ProjectTechnology> entities)
        {
            var tech = await _projectTechnologyService.UpdateListTechnologyAsync(id, entities);
            if (tech.AppResult.DataResult == null)
            {
                return BadRequest(tech.AppResult);
            }
            return Ok(tech.AppResult);
        }
    }
}
