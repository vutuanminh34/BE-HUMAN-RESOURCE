using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Constants;
using WebAPI.Models;
using WebAPI.Models.Resource.WorkHistory;
using WebAPI.RequestModel;
using WebAPI.Services.WorkHistories;
using WebAPI.ViewModels;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkHistoryController : BaseApiController<WorkHistoryViewModel>
    {
        private IWorkHistoryService _workHistoryService;
        public WorkHistoryController(IWorkHistoryService workHistoryService)
        {
            this._workHistoryService = workHistoryService;
        }

        /// <summary>
        /// Get WorkHistory by PersonId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpGet("person/{id}")]
        public async Task<IActionResult> GetWorkHistoryByPersonId(int id)
        {
            var app = await _workHistoryService.GetWorkHistoryByPersonId(id);
            apiResult.AppResult.DataResult = app;
            apiResult.AppResult.Message = Constant.GET_SUCCESS;
            return Ok(apiResult.AppResult);
        }

        /// <summary>
        /// Create WorkHistory
        /// </summary>
        /// <param name="workHistory"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPost]
        public async Task<IActionResult> InsertworkHistory([FromBody] CreateWorkHistoryResource workHistory)
        {
            var app = await _workHistoryService.CreateWorkHistory(workHistory);
            if (!app.AppResult.Result) return BadRequest(app.AppResult);
            return Ok(app.AppResult);
        }

        /// <summary>
        /// Update WorkHistory
        /// </summary>
        /// <param name="workHistory"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkHistory(int id, [FromBody] UpdateWorkHistoryResource workHistory)
        {
            var app = await _workHistoryService.UpdateInfomationWorkHistory(id, workHistory);
            if (!app.AppResult.Result) return BadRequest(app.AppResult);
            return Ok(app.AppResult);
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
            var result = await _workHistoryService.UpdateOrderIndexWorkHistory(swapId);
            if (!result.AppResult.Result) return BadRequest(result.AppResult);
            return Ok(result.AppResult);
        }

        /// <summary>
        /// Delete WorkHistory
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteworkHistory(int id)
        {
            var app = await _workHistoryService.DeleteWorkHistory(id);
            if (!app.AppResult.Result) return BadRequest(app.AppResult);
            return Ok(app.AppResult);
        }
    }
}
