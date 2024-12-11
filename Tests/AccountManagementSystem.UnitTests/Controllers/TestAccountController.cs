using AccountManagementSystem.Controllers;
using AccountManagementSystem.Services.IServices;
using AccountManagementSystem.UnitTests.Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;

namespace AccountManagementSystem.UnitTests.Controllers
{
    public class TestAccountController
    {
        [Fact]
        public async Task Get_OnSuccess_ReturnStatusCode200()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            mockAccountService.Setup(service => service.GetAccounts())
                .ReturnsAsync(AccountFixtures.AccountsList());
                
            var accountController = new AccountController(mockAccountService.Object);

            // Act
            var result = (OkObjectResult) await accountController.Get();

            // Assert
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Get_OnSuccess_InvokeService()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            mockAccountService.Setup(service => service.GetAccounts())
                .ReturnsAsync(AccountFixtures.AccountsList());
                
            var accountController = new AccountController(mockAccountService.Object);

            // Act
            var result = (OkObjectResult) await accountController.Get();

            // Assert
            result.StatusCode.Should().Be(200);
            mockAccountService.Verify(service => service.GetAccounts(), Times.Once);
        }

        [Fact]
        public async Task Get_OnSuccess_ReturnsListofAccounts()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            mockAccountService.Setup(service => service.GetAccounts())
                .ReturnsAsync(AccountFixtures.AccountsList());
                
            var accountController = new AccountController(mockAccountService.Object);

            // Act
            var result = (OkObjectResult) await accountController.Get();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result.Value.Should().BeOfType<List<Account>>();
        }

        [Fact]
        public async Task Get_OnNoAccounts_ReturnsNotfound()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            mockAccountService.Setup(service => service.GetAccounts())
                .ReturnsAsync(new List<Account>());
                
            var accountController = new AccountController(mockAccountService.Object);

            // Act
            var result = (NotFoundResult) await accountController.Get();

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            
        }
        
    }
}