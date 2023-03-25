using Moq;
using FluentAssertions;
using BLL.Infrastructure;
using BLL.Services;
using BLL.Interfaces;
using BLL.DTO;
using PL_WEB.Util;
using PL_WEB.Controllers;
using Microsoft.AspNetCore.Mvc;
using PL_WEB.Models;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace xNetLab3_Test
{
    public class AccountControllerTest
    {
        [Fact]
        public void Login_ReturnsCorrectView()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            AccountController controller = new AccountController(mockUserService.Object);

            // Act
            var result = controller.Login();

            // Assert
            result.Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeOfType<LoginModel>();
            result.As<ViewResult>().ViewName.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task Login_InValidModel_ReturnsView()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            AccountController controller = new AccountController(mockUserService.Object);
            var loginModel = new LoginModel { UserName = "TestUser", Password = "TestPass" };
            

            // Act
            var result = await controller.Login(loginModel);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().BeOfType<LoginModel>();
        }

        [Fact]
        public async Task Login_ValidModel_RedirectsToHomeIndex()
        {
            // Arrange
            var user = new UserDTO { Id = 1, Login = "testuser", Password = "testpassword", Role = "customer" };

            var model = new LoginModel { UserName = user.Login,Password = user.Password, RememberLogin = false, ReturnUrl = "/Home/Index" };

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(m => m.GetUser(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(user);

            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            AccountController controller = new AccountController(mockUserService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        RequestServices = serviceProviderMock.Object
                    }
                }
            };

            // Act
            var result = await controller.Login(model);

            // Assert
            result.Should().BeOfType<LocalRedirectResult>()
                .Which.Url.Should().Be(model.ReturnUrl);
        }

        [Fact]
        public async Task Login_Model_ReturnsViewWithErrorMessage()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(m => m.GetUser(It.IsAny<string>(), It.IsAny<string>()))
                            .Throws(new ValidationException("Invalid username or password", ""));

            AccountController controller = new AccountController(mockUserService.Object);

            var loginModel = new LoginModel { UserName = "TestUser", Password = "TestPass" };

            // Act
            var result = await controller.Login(loginModel);

            // Assert
            result.Should().BeOfType<ViewResult>();

            string str = controller.ViewBag.Message;
            str.Should().Be("Invalid username or password");
        }

        [Fact]
        public void Register_ReturnsCorrectView()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            AccountController controller = new AccountController(mockUserService.Object);

            // Act
            var result = controller.Register();

            // Assert
            result.Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeOfType<RegistrationModel>();
            result.As<ViewResult>().ViewName.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Register_ModelStateIsInvalid_ReturnsView()
        {
            // Arrange
            var controller = new AccountController(null);

            var model = new RegistrationModel { Password = "password1", PasswordConfirm = "password2" };
            controller.ModelState.AddModelError(nameof(RegistrationModel.PasswordConfirm), "Passwords do not match");

            // Act
            var result = controller.Register(model);

            // Assert
            result.Should().BeAssignableTo<ViewResult>();
        }

        [Fact]
        public void Register_ServiceError_ReturnsView()
        {
            // Arrange
            var validationErrorMessage = "Error message";
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.RegUser(It.IsAny<UserDTO>())).Throws(new ValidationException(validationErrorMessage, " "));
            var controller = new AccountController(userServiceMock.Object);

            var model = new RegistrationModel { UserName = "TestUser", Password = "TestPass", PasswordConfirm = "TestPass" };

            // Act
            var result = controller.Register(model);

            // Assert
            result.Should().BeAssignableTo<ViewResult>();
            string str = controller.ViewBag.Message;
            str.Should().Be(validationErrorMessage);
        }

        [Fact]
        public void Register_ModelError_ReturnsView()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();

            AccountController controller = new AccountController(userServiceMock.Object);

            var model = new RegistrationModel { UserName = "TestUser", Password = "TestPass", PasswordConfirm = "TestPass2" };

            // Act
            var result = controller.Register(model);

            // Assert
            result.Should().BeAssignableTo<ViewResult>();
            string str = controller.ViewBag.Message;
            str.Should().Be("Password and PasswordConfirm are different!");
        }

        [Fact]
        public void Register_ModelStateIsValidAndUserServiceRegistersUser_ReturnsRedirect()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.RegUser(It.IsAny<UserDTO>()));
            var controller = new AccountController(userServiceMock.Object);

            var returnUrl = "/home/index";
            var model = new RegistrationModel { UserName = "user1", Password = "password1", PasswordConfirm = "password1", ReturnUrl = returnUrl };

            // Act
            var result = controller.Register(model);

            // Assert
            result.Should().BeAssignableTo<LocalRedirectResult>();
            result.As<LocalRedirectResult>().Url.Should().Be(returnUrl);
            userServiceMock.Verify(x => x.RegUser(It.IsAny<UserDTO>()), Times.Once);
        }

        [Fact]
        public async Task SignOut_RedirectsToHomeIndex()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();

            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            AccountController controller = new AccountController(mockUserService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        RequestServices = serviceProviderMock.Object
                    }
                }
            };

            // Act
            var result = await controller.LogOut();

            // Assert
            result.Should().BeOfType<LocalRedirectResult>()
                .Which.Url.Should().Be("/");
        }

        [Fact]
        public void Dispose_ShouldCallUserServiceDispose()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var controller = new AccountController(mockUserService.Object);

            // Act
            controller.Dispose();

            // Assert
            mockUserService.Verify(u => u.Dispose(), Times.Once);
        }
    }
}