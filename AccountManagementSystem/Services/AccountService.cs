using AccountManagementSystem.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Models;
using Persistance;

namespace AccountManagementSystem.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDBContext _dbContext;

        public AccountService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Account> CreateNewAccount(Account account)
        {
            Random rnd = new Random();
            account.AccountNumber = rnd.Next(10000, 1000000000);
            account.CreatedDate = DateTime.Now;
            account.UpdatedDate = DateTime.Now;
            await _dbContext.Accounts.AddAsync(account);
            var result = await _dbContext.SaveChangesAsync();
            if(result == 1) return account;
            else return null;
        }

        public async Task<List<Account>> GetAccounts()
        {
            return await _dbContext.Accounts.AsNoTracking().ToListAsync();
        }

        public async Task<Account> GetSingleAccount(Guid id)
        {
            return await _dbContext.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}