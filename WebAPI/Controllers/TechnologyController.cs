using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.Resource.Technology;
using WebAPI.RequestModel;
using WebAPI.Services.Technologies;
using WebAPI.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TechnologyController : BaseApiController<TechnologyViewModel>
    {
        private ITechnologyService _technologyService;
        public TechnologyController(ITechnologyService technologyService)
        {
            this._technologyService = technologyService;
        }

        /// <summary>
        /// Get Technology By Person
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpGet("{id}/person")]
        public async Task<IActionResult> GetTechnologyByPerson(int id)
        {
            var skills = await _technologyService.GetTechnologyByPerson(id);
            apiResult.AppResult.DataResult = skills;
            return Ok(apiResult.AppResult);
        }

        /// <summary>
        /// Get Technology By Category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpGet("{id}/category")]
        public async Task<IActionResult> GetTechnologyByCateogry(int id)
        {
            var skills = await _technologyService.GetTechnologyByCategory(id);
            apiResult.AppResult.DataResult = skills;
            return Ok(apiResult.AppResult);
        }

        /// <summary>
        /// Get all Technology
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="search"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpGet]
        public async Task<IActionResult> GetAllTechnology(int? page, int? limit, string search, int? category)
        {
            var technologies = await _technologyService.GetAllTechnology(page, limit, search, category);
            return Ok(technologies);
        }

        /// <summary>
        /// Create Technology
        /// </summary>
        /// <param name="technology"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPost]
        public async Task<IActionResult> InsertTechnology([FromBody]Technology technology)
        {
            var app = await _technologyService.InsertTechnology(technology);
            if (app.AppResult.Result == false)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }

        /// <summary>
        /// Update Technology
        /// </summary>
        /// <param name="technology"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTechnology([FromBody] Technology technology, int id)
        {
            var app = await _technologyService.UpdateTechnology(technology, id);
            if (app.AppResult.Result == false)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }

        /// <summary>
        /// Delete Technology
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTechnology(int id)
        {
            var app = await _technologyService.DeleteTechnology(id);
            if (app.AppResult.Result == false)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }
    }
}
