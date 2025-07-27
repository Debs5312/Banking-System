using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using UserManagementSystem.Controller;
using UserManagementSystem.Service.IService;
using Models;
using Models.DTOs;
using Xunit;

namespace UserManagementSystem.UnitTests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _controller = new UserController(_mockUserService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Register_ReturnsOk_OnSuccess()
        {
            _mockUserService.Setup(s => s.RegisterNewUser(It.IsAny<RegistrationDTO>()))
                .ReturnsAsync(RegistrationResult.Success);

            var result = await _controller.Register(new RegistrationDTO(), CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User registered successfully.", okResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsConflict_WhenUserNameExists()
        {
            _mockUserService.Setup(s => s.RegisterNewUser(It.IsAny<RegistrationDTO>()))
                .ReturnsAsync(RegistrationResult.UserNameExists);

            var result = await _controller.Register(new RegistrationDTO(), CancellationToken.None);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("A user with this username already exists.", conflictResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsConflict_WhenAdharIdInUse()
        {
            _mockUserService.Setup(s => s.RegisterNewUser(It.IsAny<RegistrationDTO>()))
                .ReturnsAsync(RegistrationResult.AdharIdInUse);

            var result = await _controller.Register(new RegistrationDTO(), CancellationToken.None);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("This Adhar ID is already in use.", conflictResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenInvalidAdharId()
        {
            _mockUserService.Setup(s => s.RegisterNewUser(It.IsAny<RegistrationDTO>()))
                .ReturnsAsync(RegistrationResult.InvalidAdharId);

            var result = await _controller.Register(new RegistrationDTO(), CancellationToken.None);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The provided Adhar ID is not a valid format.", badRequestResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_OnUnexpectedError()
        {
            _mockUserService.Setup(s => s.RegisterNewUser(It.IsAny<RegistrationDTO>()))
                .ReturnsAsync(RegistrationResult.SaveChangesFailure);

            var result = await _controller.Register(new RegistrationDTO(), CancellationToken.None);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("An unexpected error occurred during registration.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenLoggedIn()
        {
            var loggedInStatus = new LoggedInStatus
            {
                LoggedIn = true,
                Token = "token",
                Message = "User is logged in successfully."
            };

            _mockUserService.Setup(s => s.LoginUser(It.IsAny<LoginDTO>()))
                .ReturnsAsync(loggedInStatus);

            var result = await _controller.Login(new LoginDTO(), CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(loggedInStatus, okResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenNotLoggedIn()
        {
            var loggedInStatus = new LoggedInStatus
            {
                LoggedIn = false,
                Token = string.Empty,
                Message = "Invalid username or password."
            };

            _mockUserService.Setup(s => s.LoginUser(It.IsAny<LoginDTO>()))
                .ReturnsAsync(loggedInStatus);

            var result = await _controller.Login(new LoginDTO(), CancellationToken.None);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(loggedInStatus, unauthorizedResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenDeleted()
        {
            _mockUserService.Setup(s => s.DeleteUser(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            var result = await _controller.Delete(Guid.NewGuid(), CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User deleted successfully.", okResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenUserNotFound()
        {
            _mockUserService.Setup(s => s.DeleteUser(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            var result = await _controller.Delete(Guid.NewGuid(), CancellationToken.None);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task Get_ReturnsOk_WithUsers()
        {
            var users = new List<User>
            {
                new User { UserName = "user1" },
                new User { UserName = "user2" }
            };

            _mockUserService.Setup(s => s.ReturnUsers())
                .ReturnsAsync(users);

            var result = await _controller.Get(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
            Assert.Contains(returnedUsers, u => u.UserName == "user1");
            Assert.Contains(returnedUsers, u => u.UserName == "user2");
        }
    }
}
