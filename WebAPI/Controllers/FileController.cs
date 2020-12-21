using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAPI.Constants;
using WebAPI.Models;
using WebAPI.Services.ExportFiles;
using WebAPI.Services.ImportFiles;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : BaseApiController<AppResult>
    {
        private readonly IExportFileService _exportFileService;
        private readonly IImportService _importService;

        public FileController(IExportFileService exportFileService, IImportService importService)
        {
            this._exportFileService = exportFileService;
            this._importService = importService;
        }
        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Export(int id, string type)
        {
            var result = await _exportFileService.ExportFile(id, type);

            if (result == null)
            {
                apiResult.Message = Constant.PERSONID_ERROR;
                apiResult.DataResult = null;
                return BadRequest(apiResult);
            }
            apiResult.DataResult = result;
            apiResult.Message = Constant.GET_SUCCESS;
            return Ok(apiResult);
        }

        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPost]
        public async Task<IActionResult> ImportFile([FromForm] FileUpload file)
        {
            var app = await _importService.ImportFile(file, null);
            apiResult = app;
            if (app.Result)
                return Ok(apiResult);
            else
                return BadRequest(apiResult);
        }
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPut("{id}")]
        public async Task<IActionResult> ImportFileToUpdate([FromForm] FileUpload file, int? id)
        {
            var app = await _importService.ImportFile(file, id);
            apiResult = app;
            if (app.Result)
                return Ok(apiResult);
            else
                return BadRequest(apiResult);
        }
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpGet]
        public async Task<IActionResult> DownloadTemplateCV()
        {
            var result = await _exportFileService.DownloadTemplateCV();
            apiResult.DataResult = result;
            apiResult.Message = Constant.GET_SUCCESS;
            return Ok(apiResult);
        }
    }
}
