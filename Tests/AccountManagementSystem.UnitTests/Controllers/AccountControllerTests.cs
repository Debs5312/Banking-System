using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManagementSystem.Controllers;
using AccountManagementSystem.Services.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;
using Moq;
using Xunit;
using AccountManagementSystem.UnitTests.Fixtures;

namespace AccountManagementSystem.UnitTests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<AccountController>> _mockLogger;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockAccountService = new Mock<IAccountService>();
            _mockLogger = new Mock<ILogger<AccountController>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AccountManagementSystem.UnitTests.Fixtures.TestMappingProfile>();
            });
            _mapper = config.CreateMapper();

            _controller = new AccountController(_mockAccountService.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task Get_ReturnsOk_WithAccounts()
        {
            var accounts = AccountFixtures.AccountsList();
            _mockAccountService.Setup(s => s.GetAccounts()).ReturnsAsync(accounts);

            var result = await _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnAccounts = Assert.IsAssignableFrom<IEnumerable<Account>>(okResult.Value);
            Assert.Equal(accounts.Count, returnAccounts.Count());
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenNoAccounts()
        {
            _mockAccountService.Setup(s => s.GetAccounts()).ReturnsAsync(new List<Account>());

            var result = await _controller.Get();

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetWithRef_ReturnsOk_WithMappedAccounts()
        {
            var accounts = AccountFixtures.AccountsList();
            _mockAccountService.Setup(s => s.GetAccountsWithRef()).ReturnsAsync(accounts);

            var result = await _controller.GetWithRef();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnAccounts = Assert.IsAssignableFrom<IEnumerable<AccountReadDTO>>(okResult.Value);
            Assert.Equal(accounts.Count, returnAccounts.Count());
        }

        [Fact]
        public async Task GetWithRef_ReturnsNotFound_WhenNoAccounts()
        {
            _mockAccountService.Setup(s => s.GetAccountsWithRef()).ReturnsAsync(new List<Account>());

            var result = await _controller.GetWithRef();

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAccountWithRef_ReturnsOk_WhenAccountExists()
        {
            var account = AccountFixtures.AccountsList().First();
            _mockAccountService.Setup(s => s.GetSingleAccountWithRef(account.Id)).ReturnsAsync(account);

            var cancellationToken = CancellationToken.None;
            var result = await _controller.GetAccountWithRef(account.Id, cancellationToken);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnAccount = Assert.IsType<AccountReadDTO>(okResult.Value);
            Assert.Equal(account.Id, returnAccount.Id);
        }

        [Fact]
        public async Task GetAccountWithRef_ReturnsNotFound_WhenAccountDoesNotExist()
        {
            _mockAccountService.Setup(s => s.GetSingleAccountWithRef(It.IsAny<Guid>())).ReturnsAsync((Account)null);

            var cancellationToken = CancellationToken.None;
            var result = await _controller.GetAccountWithRef(Guid.NewGuid(), cancellationToken);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAccountWithRef_ReturnsStatus499_WhenCancelled()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            var result = await _controller.GetAccountWithRef(Guid.NewGuid(), cts.Token);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(499, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetAccount_ReturnsOk_WhenAccountExists()
        {
            var account = AccountFixtures.AccountsList().First();
            _mockAccountService.Setup(s => s.GetSingleAccount(account.Id)).ReturnsAsync(account);

            var cancellationToken = CancellationToken.None;
            var result = await _controller.GetAccount(account.Id, cancellationToken);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnAccount = Assert.IsType<Account>(okResult.Value);
            Assert.Equal(account.Id, returnAccount.Id);
        }

        [Fact]
        public async Task GetAccount_ReturnsNotFound_WhenAccountDoesNotExist()
        {
            _mockAccountService.Setup(s => s.GetSingleAccount(It.IsAny<Guid>())).ReturnsAsync((Account)null);

            var cancellationToken = CancellationToken.None;
            var result = await _controller.GetAccount(Guid.NewGuid(), cancellationToken);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAccount_ReturnsStatus499_WhenCancelled()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            var result = await _controller.GetAccount(Guid.NewGuid(), cts.Token);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(499, statusResult.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenInputIsNull()
        {
            var result = await _controller.Create(null, CancellationToken.None);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Account details cannot be null.", badRequestResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenAccountCreated()
        {
            var accountInput = new AccountModelInputDTO
            {
                PrimaryUserId = Guid.NewGuid()
            };
            var account = _mapper.Map<Account>(accountInput);

            _mockAccountService.Setup(s => s.CreateNewAccount(It.IsAny<Account>())).ReturnsAsync(account);

            var result = await _controller.Create(accountInput, CancellationToken.None);

            var createdResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            var returnAccount = Assert.IsType<Account>(createdResult.Value);
            Assert.Equal(account.Id, returnAccount.Id);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenAccountNotCreated()
        {
            var accountInput = new AccountModelInputDTO
            {
                PrimaryUserId = Guid.NewGuid()
            };

            _mockAccountService.Setup(s => s.CreateNewAccount(It.IsAny<Account>())).ReturnsAsync((Account)null);

            var result = await _controller.Create(accountInput, CancellationToken.None);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Could not create the account with the provided details.", badRequestResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsStatus499_WhenCancelled()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            var result = await _controller.Create(new AccountModelInputDTO(), cts.Token);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(499, statusResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenInputIsNull()
        {
            var result = await _controller.Update(Guid.NewGuid(), null, CancellationToken.None);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Update details cannot be null.", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenAccountUpdated()
        {
            var updateDto = new UpdateAccountDTO
            {
            };
            var updatedAccount = AccountFixtures.AccountsList().First();

            _mockAccountService.Setup(s => s.UpdateAccount(It.IsAny<Guid>(), updateDto)).ReturnsAsync(updatedAccount);

            var result = await _controller.Update(Guid.NewGuid(), updateDto, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnAccount = Assert.IsType<Account>(okResult.Value);
            Assert.Equal(updatedAccount.Id, returnAccount.Id);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenAccountNotUpdated()
        {
            var updateDto = new UpdateAccountDTO
            {
            };

            _mockAccountService.Setup(s => s.UpdateAccount(It.IsAny<Guid>(), updateDto)).ReturnsAsync((Account)null);

            var result = await _controller.Update(Guid.NewGuid(), updateDto, CancellationToken.None);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Could not update the account with the provided details.", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsStatus499_WhenCancelled()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            var result = await _controller.Update(Guid.NewGuid(), new UpdateAccountDTO(), cts.Token);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(499, statusResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenAccountDeleted()
        {
            var deletedAccountId = Guid.NewGuid();

            _mockAccountService.Setup(s => s.DeleteAccount(It.IsAny<Guid>())).ReturnsAsync(deletedAccountId);

            var result = await _controller.Delete(Guid.NewGuid(), CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnId = Assert.IsType<Guid>(okResult.Value);
            Assert.Equal(deletedAccountId, returnId);
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenAccountNotDeleted()
        {
            _mockAccountService.Setup(s => s.DeleteAccount(It.IsAny<Guid>())).ReturnsAsync((Guid?)null);

            var result = await _controller.Delete(Guid.NewGuid(), CancellationToken.None);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Could not delete the account.", badRequestResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsStatus499_WhenCancelled()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            var result = await _controller.Delete(Guid.NewGuid(), cts.Token);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(499, statusResult.StatusCode);
        }
    }
}
