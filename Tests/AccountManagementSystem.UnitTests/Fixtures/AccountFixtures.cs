using Models;

namespace AccountManagementSystem.UnitTests.Fixtures
{
    public class AccountFixtures
    {
        public static List<Account> AccountsList() => new()
        {
            new Account()
            {
                Id = Guid.NewGuid(),
                AccountNumber = 4857487,
                PrimaryUserId = Guid.NewGuid(),
                SecondaryUserId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            },
            new Account()
            {
                Id = Guid.NewGuid(),
                AccountNumber = 574983,
                PrimaryUserId = Guid.NewGuid(),
                SecondaryUserId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            },
            new Account()
            {
                Id = Guid.NewGuid(),
                AccountNumber = 7594212,
                PrimaryUserId = Guid.NewGuid(),
                SecondaryUserId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            }
        };
    }
}