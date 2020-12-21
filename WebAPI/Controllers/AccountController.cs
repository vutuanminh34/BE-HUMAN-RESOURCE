using WebAPI.Services.Accounts;
using WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAPI.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using WebAPI.Models.Token;
using Newtonsoft.Json.Linq;
using WebAPI.RequestModel;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseApiController<AccountViewModel>
    {
        private IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            this._accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest model)
        {
            var response = await _accountService.Authenticate(model, IpAddress());

            if (response == null)
                return BadRequest(new { Message = "Username or password is incorrect" });

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("captcha")]
        public IActionResult CheckRecaptcha(string encodedResponse)
        {
            bool isCaptchaValid = (ReCaptchaRequestModel.Validate(encodedResponse) == "true" ? true : false);
            if (!isCaptchaValid)
            {
                return BadRequest(new { Message = "Invalid Recaptcha!"});
            }
            return Ok(isCaptchaValid);
        }

        [AllowAnonymous]
        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken(JObject jObject)
        {
            string refreshToken = jObject["refreshToken"]?.ToString();
            var response = await _accountService.RefreshToken(refreshToken, IpAddress());

            if (response == null)
                return BadRequest(new { Message = "Invalid token" });

            return Ok(response);
        }

        /// <summary>
        /// Get all Account
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllAccount()
        {
            var employees = await _accountService.GetAllAccount();
            apiResult.AppResult.DataResult = employees.AsEnumerable<AccountInfo>();
            return Ok(apiResult.AppResult);
        }

        /// <summary>
        /// Get Account by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            var app = await _accountService.GetAccountById(id);
            apiResult.AppResult.DataResult = app;
            return Ok(apiResult.AppResult);
        }

        /// <summary>
        /// Get all Account by Name with pagging and Get all account only
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAccountByName(int iPageNum, int iPageSize, string fullName, string email, string phone, string address)
        {
            Paggings paggings = new Paggings
            {
                PageNum = iPageNum == 0 ? 1 : iPageNum,
                PageSize = iPageSize == 0 ? int.MaxValue : iPageSize
            };

            if(iPageNum <= 0 || iPageSize <= 0)
            {
                return BadRequest(new { Message = "Invalid iPageNum or iPageSize!" });
            }

            apiResult.Pagging.PageNum = paggings.PageNum;
            apiResult.Pagging.PageSize = paggings.PageSize;
            apiResult.AccountInfo.FULL_NAME = fullName;
            apiResult.AccountInfo.EMAIL = email;
            apiResult.AccountInfo.PHONE = phone;
            apiResult.AccountInfo.ADDRESS = address;

            if(fullName is null && email is null && phone is null && address is null)
            {
                var employee = await _accountService.GetAllAccountWithPagging(paggings);
                employee.AppResult.DataResult = new { employee.ListAccount, employee.Pagging };
                return Ok(employee.AppResult);
            }
            else
            {
                var employees = await _accountService.GetAllAccountByName(apiResult);
                employees.AppResult.DataResult = new { employees.ListAccount, employees.Pagging };
                return Ok(employees.AppResult);
            }
        }

        /// <summary>
        /// Create Account
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        public async Task<IActionResult> InsertAccount([FromBody] AccountInfo accountInfo)
        {
            var app = await _accountService.InsertAccount(accountInfo);
            if(app.AppResult.DataResult == null)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountInfo"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] AccountInfo accountInfo)
        {
            accountInfo.USER_NO = id;
            var app = await _accountService.UpdateAccount(accountInfo);
            if (app.AppResult.DataResult == null)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }

        /// <summary>
        /// update new password and check
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jObject"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpPut("Password/{id}")]
        public async Task<IActionResult> UpdatePassword(int id, JObject jObject)
        {
            string oldPassword = jObject["oldPassword"]?.ToString();
            string newPassword = jObject["newPassword"]?.ToString();

            AccountInfo accountInfo = new AccountInfo();
            accountInfo.USER_NO = id;
            accountInfo.PASSWORD = newPassword;

            bool check = _accountService.CheckPass(id, oldPassword);
            if (!check)
            {
                return BadRequest(new { Message = "The old password is incorrect!" });
            }
            if(oldPassword == newPassword)
            {
                return BadRequest(new { Message = "The new password must be different from the old password!" });
            }

            var app = await _accountService.UpdatePassword(accountInfo);
            if (app.AppResult.DataResult == null)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }

        /// <summary>
        /// Delete Account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var app = await _accountService.DeleteAccount(id);
            if (app.AppResult.DataResult == null)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }

        /// <summary>
        /// Set token into cookie
        /// </summary>
        /// <param name="token"></param>
        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        /// <summary>
        /// Get IP address
        /// </summary>
        /// <returns></returns>
        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
