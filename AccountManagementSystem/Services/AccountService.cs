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