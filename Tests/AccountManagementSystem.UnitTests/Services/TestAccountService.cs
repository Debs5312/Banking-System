using AccountManagementSystem.Services;
using AccountManagementSystem.Services.IServices;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Models;
using Moq;
using Persistance;

namespace AccountManagementSystem.UnitTests.Services
{
    public class TestAccountService
    {
        private readonly Fixture fixture;
        private readonly Mock<AppDBContext> _context;
        private readonly IAccountService _accountService;

        public TestAccountService()
        {
            fixture = new Fixture();
            _context = new Mock<AppDBContext>();
            _accountService = new AccountService(_context.Object);
        }

        [Fact]
        public async Task Get_OnSuccess_ReturnAccountList()
        {
            // Arrange
            var accountDbSet = fixture.Build<Account>()
                            .Without(x => x.PrimaryAccountHolder)
                            .Without(x => x.SecondaryAccountHolder)
                            .Without(x => x.Nominee)
                            .CreateMany<Account>()
                            .AsQueryable()
                            .BuildMockDbSet();

            _context.Setup(m => m.Accounts).Returns(() => accountDbSet.Object);

            // Act
            var results = await _accountService.GetAccounts();

            // Assert
            results.Should().NotBeEmpty();
        }

        [Theory, AutoData]
        public async Task Get_OnSuccess_ReturnAccountOfSpecificId(Guid id)
        {
            // Arrange
            var singleAccount = fixture.Build<Account>()
                                .With(c => c.Id, id)
                                .Without(x => x.PrimaryAccountHolder)
                                .Without(x => x.SecondaryAccountHolder)
                                .Without(x => x.Nominee)
                                .Create<Account>();
            
            var accountQueryableObject = fixture.Build<Account>()
                            .Without(x => x.PrimaryAccountHolder)
                            .Without(x => x.SecondaryAccountHolder)
                            .Without(x => x.Nominee)
                            .CreateMany<Account>()
                            .AsQueryable();

            var accountList = accountQueryableObject.ToList();

            accountList.Add(singleAccount);
            var accountDbSet = accountList.BuildMockDbSet();              

            _context.Setup(m => m.Accounts)
                .Returns(() => accountDbSet.Object);                                 

            // Act
            var results = await _accountService.GetSingleAccount(id);

            // Assert
            results.Should().NotBeNull();
            results.Id.Should().Be(id);
        }

        [Fact]
        public async Task Create_OnSuccess_NewDataToDatabase()
        {
            // Arrange
            var singleAccount = fixture.Build<Account>()
                                .Without(x => x.PrimaryAccountHolder)
                                .Without(x => x.SecondaryAccountHolder)
                                .Without(x => x.Nominee)
                                .Without(x => x.AccountNumber)
                                .Without(x => x.CreatedDate)
                                .Without(x => x.UpdatedDate)
                                .Create<Account>();               

            Mock<DbSet<Account>> mockDBSet = new Mock<DbSet<Account>>();

            _context.Setup(x => x.Accounts).Returns(mockDBSet.Object);

            // Act
            var result = await _accountService.CreateNewAccount(singleAccount);

            // Assert
            mockDBSet.Verify(m => m.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Once);
            _context.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}