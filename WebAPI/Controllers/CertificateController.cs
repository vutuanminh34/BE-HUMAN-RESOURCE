#nullable enable
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAPI.Constants;
using WebAPI.Models;
using WebAPI.Models.Resource.Certificate;
using WebAPI.RequestModel;
using WebAPI.Services.Certificates;
using WebAPI.ViewModels;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : BaseApiController<CertificateViewModel<SaveCertificateResource>>
    {

        private readonly ICertificateService _certificateService;
        public CertificateController(ICertificateService certificateService)
        {
            this._certificateService = certificateService;
        }

        /// <summary>
        /// Get all field from table Certificate with personId
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCertificateByPersonId(int id)
        {
            var tempCertificate = await _certificateService.GetCertificateByPersonId(id);

            apiResult.AppResult.Message = Constant.GET_SUCCESS;
            apiResult.AppResult.DataResult = tempCertificate;

            return Ok(apiResult.AppResult);
        }

        /// <summary>
        /// Create a field into Certificate
        /// </summary>
        /// <param name="certificateObj"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPost]
        public async Task<IActionResult> CreateCertificate([FromBody] CreateCertificateResource certificateObj)
        {
            var tempCertificateModel = await _certificateService.CreateCertificate(certificateObj);
            if (!tempCertificateModel.AppResult.Result) return BadRequest(tempCertificateModel.AppResult);

            return Ok(tempCertificateModel.AppResult);
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
            var tempCertificateModel = await _certificateService.UpdateOrderIndexCertificate(swapId);
            if (!tempCertificateModel.AppResult.Result) return BadRequest(tempCertificateModel.AppResult);

            return Ok(tempCertificateModel.AppResult);
        }

        /// <summary>
        /// Update a field from table Certificate
        /// </summary>
        /// <param name="certificateObj"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatetCertificate(int id, [FromBody] UpdateCertificateResource certificateObj)
        {
            var tempCertificateModel = await _certificateService.UpdateInfomationCertificate(id, certificateObj);
            if (!tempCertificateModel.AppResult.Result) return BadRequest(tempCertificateModel.AppResult);
            
            return Ok(tempCertificateModel.AppResult);
        }

        /// <summary>
        /// Delete Certificate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificate(int id)
        {
            var tempCertificateModel = await _certificateService.DeleteCertificate(id);
            if (!tempCertificateModel.AppResult.Result) return BadRequest(tempCertificateModel.AppResult);

            return Ok(tempCertificateModel.AppResult);
        }
    }
}
