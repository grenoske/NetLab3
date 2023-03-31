using Moq;
using FluentAssertions;
using BLL.Infrastructure;
using BLL.Services;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Repositories;
using PL.Models;
using PL.View;
using BLL.DTO;
using DAL.Entities;
using System.Linq.Expressions;

namespace xNetLab3_Test
{
    public class UserServiceTest
    {
        private Mock<IUnitOfWork> mockUnitOfWork;
        private UserService userService;

        public UserServiceTest()
        {
            mockUnitOfWork = new Mock<IUnitOfWork>();
            userService = new UserService(mockUnitOfWork.Object);
        }

        [Fact]
        public void GetUser_NullLogin_ErrorReturn()
        {
            // Arrange
            string Login = null;

            // Act
            Action action = new Action(() => userService.GetUser(Login, null));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Login is Null");
        }

        [Fact]
        public void GetUser_NoUser_ErrorReturn()
        {
            // Arrange
            mockUnitOfWork.Setup(u => u.Users.GetFirstOrDefault(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(User));

            string Login = "TestLogin";

            // Act
            Action action = new Action(() => userService.GetUser(Login, null));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("User not Found");
        }

        [Fact]
        public void GetUser_NotCorrectPass_ErrorReturn()
        {
            // Arrange
            mockUnitOfWork.Setup(u => u.Users.GetFirstOrDefault(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(new User { Id = 1, Login = "TestLogin", Password = "Correct" });

            string Login = "TestLogin";
            string Password = "notCorrect";

            // Act
            Action action = new Action(() => userService.GetUser(Login, Password));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Password is Incorrect");
        }

        [Fact]
        public void GetUser_Correct()
        {
            // Arrange
            User user = new User { Id = 1, Login = "TestLogin", Password = "Correct" };
            UserDTO expectedUser = new UserDTO { Id = 1, Login = "TestLogin", Password = "Correct" };

            mockUnitOfWork.Setup(u => u.Users.GetFirstOrDefault(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(user);

            string Login = "TestLogin";
            string Password = "Correct";

            // Act
            var User = userService.GetUser(Login, Password);

            // Assert
            User.Should().NotBeNull();
            User.Should().BeEquivalentTo(expectedUser);
        }

        [Fact]
        public void GetUsers_Correct()
        {
            // Arrange
            List<User> users = new List<User>()
            { 
                new User { Id = 1, Login = "TestLogin", Password = "Correct" },
                new User { Id = 2, Login = "TestLogin2", Password = "Correct2" }
            };
            List<UserDTO> expectedUsers = new List<UserDTO>()
            { 
                new UserDTO { Id = 1, Login = "TestLogin", Password = "Correct" },
                new UserDTO { Id = 2, Login = "TestLogin2", Password = "Correct2" },
            };

            mockUnitOfWork.Setup(u => u.Users.GetAll(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
                )).Returns(users);

            // Act
            var Users = userService.GetUsers();

            // Assert
            Users.Should().NotBeNull();
            Users.Should().BeEquivalentTo(expectedUsers);
            Users.Should().HaveCount(2);

        }

        [Fact]
        public void RegUser_LoginIsAlreadyExists_ErrorReturn()
        {
            // Arrange
            User user = new User { Id = 1, Login = "TestLogin", Password = "Correct", Role = "test" };
            UserDTO UserToReg = new UserDTO { Id = 1, Login = "TestLogin", Password = "Correct", Role = "test" };


            mockUnitOfWork.Setup(u => u.Users.GetFirstOrDefault(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(user);

            // Act
            Action action = new Action(() => userService.RegUser(UserToReg));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("User is already exists");
        }

        [Fact]
        public void RegUser_Correct()
        {
            // Arrange
            UserDTO UserToReg = new UserDTO { Id = 1, Login = "TestLogin", Password = "Correct", Role = "test" };

            var userRepository = new Mock<IUserRepository>();

            mockUnitOfWork.Setup(uof => uof.Users).Returns(userRepository.Object);

            mockUnitOfWork.Setup(u => u.Users.GetFirstOrDefault(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(User));

            // Act
            userService.RegUser(UserToReg);

            // Assert
            userRepository.Verify(ur => ur.Add(It.IsAny<User>()), Times.Once);
            mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void Dispose_UserService()
        {
            // Arrange

            // Act
            userService.Dispose();

            // Assert
            mockUnitOfWork.Verify(uow => uow.Dispose(), Times.Once);
        }


    }
}