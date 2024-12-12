using AccountManagementSystem.Services;
using AccountManagementSystem.Services.IServices;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
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
            
            var accountList = fixture.Build<Account>()
                                .Without(x => x.PrimaryAccountHolder)
                                .Without(x => x.SecondaryAccountHolder)
                                .Without(x => x.Nominee)
                                .CreateMany<Account>();

            accountList.Append(singleAccount);

            var dbSetAccountList = accountList.AsQueryable().BuildMockDbSet();

            _context.Setup(m => m.Accounts)
                .Returns(() => dbSetAccountList.Object.AsQueryable().FirstOrDefault(x => x.Id == id));                                 

            // Act
            var results = await _accountService.GetSingleAccount(id);

            // Assert
            results.Should().NotBeNull();
        }
    }
}