using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.RequestModel;
using WebAPI.Services.Categories;
using WebAPI.Services.Skills;
using WebAPI.ViewModels;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiController]
    public class SkillController : BaseApiController<SkillViewModel>
    {
        private ISkillService _skillService;
        public SkillController(ISkillService skillService)
        {
            this._skillService = skillService;
        }

        /// <summary>
        /// Get Skill By Person
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpGet("person/{personId}/skill")]
        public async Task<IActionResult> GetSkill(int personId)
        {
            var skills = await _skillService.GetSkillByPerson(personId);
            apiResult.AppResult.DataResult = skills;
            return Ok(apiResult.AppResult);
        }

        /// <summary>
        /// Get Skill By Category
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpGet("person/{id}/category/{categoryId}/skill")]
        public async Task<IActionResult> GetSkillByCategory(int id, int categoryId)
        {
            var skills = await _skillService.GetSkillByCategory(id, categoryId);
            apiResult.AppResult.DataResult = skills;
            return Ok(apiResult.AppResult);
        }

        /// <summary>
        /// Insert Skill
        /// </summary>
        /// <param name="skillRequestModel"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPost("skill")]
        public async Task<IActionResult> InsertSkill([FromBody]SkillRequestModel skillRequestModel)
        {
            var app = await _skillService.InserSkill(skillRequestModel);
            if (app.AppResult.Result == false)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }

        /// <summary>
        /// Update Skill
        /// </summary>
        /// <param name="id"></param>
        /// <param name="skillRequestModel"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPut("skill/{id}")]
        public async Task<IActionResult> UpdateSkill(int id, [FromBody]SkillRequestModel skillRequestModel)
        {
            var app = await _skillService.UpdateSkill(id, skillRequestModel);
            if (app.AppResult.Result == false)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }

        /// <summary>
        /// Delete Skill By Technology
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpDelete("skill/{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var app = await _skillService.DeleteSkill(id);
            if (app.AppResult.Result == false)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }


        /// <summary>
        /// Swap OrderIndex Skill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPut("skill/swap")]
        public async Task<IActionResult> SwapOrderIndex([FromBody]SwapOrderIndexRequestModel model) 
        {
            var app = await _skillService.SwapOrderIndex(model);
            return Ok(app.AppResult);
        }

    }
}
