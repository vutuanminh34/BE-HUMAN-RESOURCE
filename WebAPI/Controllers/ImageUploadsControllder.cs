using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Services.Uploads;
using WebAPI.ViewModels;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadsController : BaseApiController<PersonViewModel>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUploadService _uploadService;
        public ImageUploadsController(IWebHostEnvironment webHostEnvironment, IUploadService uploadService)
        {
            _webHostEnvironment = webHostEnvironment;
            _uploadService = uploadService;
        }
        /// <summary>
        /// API post image
        /// </summary>
        /// <param name="objectFile"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPost]
        public async Task<IActionResult> PostImage([FromForm] FileUpload objectFile, [FromForm] int id)
        {
            string path = _webHostEnvironment.WebRootPath + "\\Avatar\\";
            var HandlingStr = await _uploadService.UploadImage(path, objectFile, id);
            if (HandlingStr == null)
            {
                apiResult.AppResult.Result = false;
                apiResult.AppResult.StatusCd = "400";
                apiResult.AppResult.Message = "This type file extensions not  format images or id person invalid";
                apiResult.AppResult.DataResult = null;
                return BadRequest(apiResult.AppResult);
            }
            else
            {
                apiResult.AppResult.StatusCd = "200";
                apiResult.AppResult.Message = "Avatar had inserted";
                apiResult.AppResult.DataResult = HandlingStr;
                return Ok(apiResult.AppResult);
            }
        }
    }
}
