using AccountManagementSystem.Controllers;
using AccountManagementSystem.Services.IServices;
using AccountManagementSystem.UnitTests.Fixtures;
using AutoFixture;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;
using Moq;

namespace AccountManagementSystem.UnitTests.Controllers
{
    public class TestAccountController
    {
        private readonly Fixture fixture;
        private readonly Mock<IAccountService> _accountService;
        private readonly IMapper mapper;
        public TestAccountController()
        {
            fixture = new Fixture();
            _accountService = new Mock<IAccountService>();
            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<AccountModelInputDTO, Account>());
            mapper = new Mapper(config);
        }

        [Fact]
        public async Task Get_OnSuccess_ReturnStatusCode200()
        {
            // Arrange
            _accountService.Setup(service => service.GetAccounts())
                .ReturnsAsync(AccountFixtures.AccountsList());
                
            var accountController = new AccountController(_accountService.Object, mapper);

            // Act
            var result = (OkObjectResult) await accountController.Get();

            // Assert
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Get_OnSuccess_InvokeService()
        {
            // Arrange
            _accountService.Setup(service => service.GetAccounts())
                .ReturnsAsync(AccountFixtures.AccountsList());
                
            var accountController = new AccountController(_accountService.Object, mapper);

            // Act
            var result = (OkObjectResult) await accountController.Get();

            // Assert
            result.StatusCode.Should().Be(200);
            _accountService.Verify(service => service.GetAccounts(), Times.Once);
        }

        [Fact]
        public async Task Get_OnSuccess_ReturnsListofAccounts()
        {
            // Arrange
            _accountService.Setup(service => service.GetAccounts())
                .ReturnsAsync(AccountFixtures.AccountsList());
                
            var accountController = new AccountController(_accountService.Object, mapper);

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
            _accountService.Setup(service => service.GetAccounts())
                .ReturnsAsync(new List<Account>());
                
            var accountController = new AccountController(_accountService.Object, mapper);

            // Act
            var result = (NotFoundResult) await accountController.Get();

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            
        }

        [Theory, AutoData]
        public async Task Get_OnSuccess_ReturnSingleAccount(Guid id)
        {
            // Arrange
            var singleAccount = fixture.Build<Account>()
                                .With(c => c.Id, id)
                                .Without(x => x.PrimaryAccountHolder)
                                .Without(x => x.SecondaryAccountHolder)
                                .Without(x => x.Nominee)
                                .Create<Account>();
            _accountService.Setup(service => service.GetSingleAccount(id))
                .ReturnsAsync(singleAccount);
            var accountController = new AccountController(_accountService.Object, mapper);

            // Act
            var result = (OkObjectResult) await accountController.GetAccount(id, It.IsAny<CancellationToken>());
            var account = (Account) result.Value!;

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result.Value.Should().BeOfType<Account>();
            account!.Id.Should().Be(id);
        }
        
        [Theory, AutoData]
        public async Task Get_OnNotfound_ReturnsNoSingleAccountDetails(Guid id)
        {
            // Arrange
            _accountService.Setup(service => service.GetSingleAccount(id))
                .ReturnsAsync((Account)null!);
            var accountController = new AccountController(_accountService.Object, mapper);

            // Act
            var result = (NotFoundResult) await accountController.GetAccount(id, It.IsAny<CancellationToken>());

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }


    [Fact]
    public async Task Create_OnInvalidInput_ReturnsBadRequest()
    {
        // Arrange
        var accountController = new AccountController(_accountService.Object, mapper);

        // Act
        var result = await accountController.Create(null!, It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Create_OnServiceException_ReturnsStatusCode500()
    {
        // Arrange
        var inputAccountDetails = fixture.Build<AccountModelInputDTO>().Create();
        var singleAccount = mapper.Map<Account>(inputAccountDetails);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<AccountModelInputDTO, Account>(It.IsAny<AccountModelInputDTO>())).Returns(singleAccount);

        _accountService.Setup(service => service.CreateNewAccount(It.IsAny<Account>()))
            .ThrowsAsync(new Exception("Service failure"));

        var accountController = new AccountController(_accountService.Object, mapperMock.Object);

        // Act
        var result = await accountController.Create(inputAccountDetails, It.IsAny<CancellationToken>());

        // Assert
        var statusCodeResult = result as StatusCodeResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Get_OnServiceException_ReturnsStatusCode500()
    {
        // Arrange
        _accountService.Setup(service => service.GetAccounts())
            .ThrowsAsync(new Exception("Service failure"));

        var accountController = new AccountController(_accountService.Object, mapper);

        // Act
        var result = await accountController.Get();

        // Assert
        var statusCodeResult = result as StatusCodeResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }
    }
}