using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using WebAPI.Services.Accounts;
using WebAPI.Models.Token;
using WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net;
using WebAPI.Models;
using WebAPI.ViewModels;

namespace WebAPI_Test.Controllers
{
    public class Account : ControllerBase
    {
        DefaultHttpContext httpContext;
        Mock<IAccountService> mockRepo;
        AccountController controller;

        [SetUp]
        public void Setup()
        {
            httpContext = new DefaultHttpContext()
            {
                Connection =
                {
                    RemoteIpAddress = new IPAddress(16885952)
                }
            };

            mockRepo = new Mock<IAccountService>();

            controller = new AccountController(mockRepo.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext
                }
            };
        }

        [Test]
        public async Task Post_Authenticate_ReturnsBadRequest_WhenUsernameOrPasswordIsIncorrect()
        {
            AuthenticateRequest model = new AuthenticateRequest
            {
                Username = "",
                Password = ""
            };
            string ipAddress = string.Empty;
            mockRepo.Setup(repo => repo.Authenticate(model, ipAddress))
                .ReturnsAsync(GetTestAuthenticateResponse());

            // Act
            var result = await controller.Authenticate(model);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
            var oObject = new BadRequestObjectResult(new { Message = "Username or password is incorrect" });
            Xunit.Assert.Equal(oObject.Value.ToString(), badRequestResult.Value.ToString());
        }

        [Test]
        public async Task Post_InsertAccount_ReturnsOk_WhenInsertSuccess()
        {
            AccountInfo model = new AccountInfo
            {
                USER_NAME = "1",
                PASSWORD = "1"
            };
            mockRepo.Setup(repo => repo.InsertAccount(model))
                .ReturnsAsync(GetTestAccountViewModel());

            // Act
            var result = await controller.InsertAccount(model);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var app = Xunit.Assert.IsType<AppResult>(okResult.Value);
            var returnAccount= Xunit.Assert.IsType<AccountInfo>(app.DataResult);
            mockRepo.Verify();
            Xunit.Assert.Equal(model.USER_NAME, returnAccount.USER_NAME);
            Xunit.Assert.Equal(model.PASSWORD, returnAccount.PASSWORD);
        }

        private AuthenticateResponse GetTestAuthenticateResponse()
        {
            var response = new AuthenticateResponse(new WebAPI.Models.AccountInfo(), string.Empty, string.Empty);
            return response;
        }

        private AccountViewModel GetTestAccountViewModel()
        {
            var response = new AccountViewModel
            {
                AccountInfo = new AccountInfo
                {
                    USER_NAME = "1",
                    PASSWORD = "1"
                }
            };
            response.AppResult.DataResult = response.AccountInfo;
            return response;
        }
    }
}
