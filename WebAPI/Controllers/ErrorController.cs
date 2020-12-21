using WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System;
using System.Linq;
using WebAPI.ViewModels;
using Serilog;
using System.Net;
using Newtonsoft.Json;

namespace WebAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ErrorController : BaseApiController<ErrorViewModel>
    {
        /// <summary>
        /// Create Log
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult CreateLog()
        {
            var excetionDetails = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
            StackTrace trace = new StackTrace(excetionDetails.Error, true);

            var thisasm = Assembly.GetExecutingAssembly();
            var actionName = excetionDetails.Path.Substring(1);
            var time = DateTime.Now.ToString("yyyy/mm/dd  hh:mm:ss");
            var version = GetType().Assembly.GetName().Version.ToString();
            var os = System.Runtime.InteropServices.RuntimeInformation.OSDescription;

            var module = trace.GetFrames().Select(f => f.GetMethod()).FirstOrDefault(m => m.Module.Assembly == thisasm)?.ReflectedType.Name;
            var procedure = trace.GetFrames().Select(f => f.GetMethod()).FirstOrDefault(m => m.Module.Assembly == thisasm)?.Name;
            var errorNumber = HttpContext.Response.StatusCode.ToString();
            var errorDesc = excetionDetails.Error.Message;

            apiResult.ActionName = actionName;
            apiResult.Time = time;
            apiResult.Description = errorDesc;
            apiResult.OS = os;
            apiResult.ErrorNumber = errorNumber;
            apiResult.AppVersion = version;
            apiResult.ClassName = module;
            apiResult.MethodName = procedure;
            apiResult.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            apiResult.AppResult = new AppResult
            {
                StatusCd = errorNumber,
                Message = ((HttpStatusCode)HttpContext.Response.StatusCode).ToString(),
                Result = false
            };

            var lineBreak = "\n";
            var outputTemplate = "{Message:lj}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.File("Logs\\error_.log", rollingInterval: RollingInterval.Day, outputTemplate: outputTemplate)
                .CreateLogger();
            var strLog = "----------------------------------------------------------------------" + lineBreak;
            strLog += "Error in         : " + actionName + lineBreak;
            strLog += "Time             : " + time + lineBreak;
            strLog += "Version          : " + version + lineBreak;
            strLog += "OS               : " + os + lineBreak;
            strLog += "Module           : " + module + lineBreak;
            strLog += "Procedure        : " + procedure + lineBreak;
            strLog += "Error Number     : " + errorNumber + lineBreak;
            strLog += "Error Description: " + errorDesc + lineBreak;
            Log.Error(strLog);
            Log.CloseAndFlush();

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(apiResult.AppResult),
                ContentType = "application/json",
                StatusCode = HttpContext.Response.StatusCode
            };
        }
    }
}
