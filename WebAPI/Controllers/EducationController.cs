#nullable enable
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAPI.Constants;
using WebAPI.Models;
using WebAPI.Models.Resource.Education;
using WebAPI.RequestModel;
using WebAPI.Services.Educations;
using WebAPI.ViewModels;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EducationController : BaseApiController<EducationViewModel<SaveEducationResource>>
    {

        private readonly IEducationService _educationService;
        public EducationController(IEducationService educationService)
        {
            this._educationService = educationService;
        }

        /// <summary>
        /// Get all field from table Education with personId
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEducationByPersonId(int id)
        {
            var tempEducation = await _educationService.GetEducationByPersonId(id);
            
            apiResult.AppResult.Message = Constant.GET_SUCCESS;
            apiResult.AppResult.DataResult = tempEducation;

            return Ok(apiResult.AppResult);
        }

        /// <summary>
        /// Create a field into Education
        /// </summary>
        /// <param name="educationObj"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPost]
        public async Task<IActionResult> CreateEducation([FromBody] CreateEducationResource educationObj)
        {
            var tempEducationModel = await _educationService.CreateEducation(educationObj);
            if (!tempEducationModel.AppResult.Result) return BadRequest(tempEducationModel.AppResult);
            return Ok(tempEducationModel.AppResult);
        }

        /// <summary>
        /// Swap order index of Object
        /// </summary>
        /// <param name="swapId"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPut("swap")]
        public async Task<IActionResult> SwapOrderIndex([FromBody] SwapOrderIndexRequestModel swapId) 
        {
            var tempEducationModel = await _educationService.UpdateOrderIndexEducation(swapId);
            if (!tempEducationModel.AppResult.Result) return BadRequest(tempEducationModel.AppResult);
            return Ok(tempEducationModel.AppResult);
        }

        /// <summary>
        /// Update a field from table Education
        /// </summary>
        /// <param name="educationObj"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatetEducation(int id, [FromBody] UpdateEducationResource educationObj)
        {
            var tempEducationModel = await _educationService.UpdateInfomationEducation(id, educationObj);
            if (!tempEducationModel.AppResult.Result) return BadRequest(tempEducationModel.AppResult);
            return Ok(tempEducationModel.AppResult);
        }

        /// <summary>
        /// Delete Education
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEducation(int id)
        {
            var tempEducationModel = await _educationService.DeleteEducation(id);
            if (!tempEducationModel.AppResult.Result) return BadRequest(tempEducationModel.AppResult);
            return Ok(tempEducationModel.AppResult);
        }
    }
}
