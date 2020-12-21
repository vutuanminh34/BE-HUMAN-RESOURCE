using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController<T> : ControllerBase, IActionFilter where T : new()
    {
        public T apiResult = new T();
        [NonAction]
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is Microsoft.AspNetCore.Mvc.ObjectResult)
            {
                var result = (Microsoft.AspNetCore.Mvc.ObjectResult)context.Result;
                if (result != null && result.Value is AppResult)
                {
                    var res = (AppResult)result.Value;
                    res.StatusCd = result.StatusCode.ToString();
                    res.Result = result.StatusCode != 200 ? false : true;
                }
            }

        }
        [NonAction]
        public void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }
}
