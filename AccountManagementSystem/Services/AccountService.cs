using AccountManagementSystem.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTOs;
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

        public async Task<List<Account>> GetAccountsWithRef()
        {
            return await _dbContext.Accounts.AsNoTracking()
                            .Include(x => x.PrimaryAccountHolder).ThenInclude(item => item.AdharNumber)
                            .Include(m => m.SecondaryAccountHolder).ThenInclude(item => item.AdharNumber)
                            .Include(y => y.Nominee).ThenInclude(item => item.AdharNumber)
                            .ToListAsync();
        }

        public async Task<Account> GetSingleAccount(Guid id)
        {
            return await _dbContext.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Account> GetSingleAccountWithRef(Guid id)
        {
            return await _dbContext.Accounts.AsNoTracking()
                            .Include(x => x.PrimaryAccountHolder).ThenInclude(item => item.AdharNumber)
                            .Include(m => m.SecondaryAccountHolder).ThenInclude(item => item.AdharNumber)
                            .Include(y => y.Nominee).ThenInclude(item => item.AdharNumber)
                            .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Account> UpdateAccount(Guid id, UpdateAccountDTO updateAccount)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == id);
            account.SecondaryUserId = updateAccount.SecondaryUserId;
            account.NomineeId = updateAccount.NomineeId;
            account.UpdatedDate = DateTime.Now;
            var result = await _dbContext.SaveChangesAsync();
            if(result ==1) return account;
            else return null;
        }

        public async Task<Guid?> DeleteAccount(Guid id)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == id);
            if (account != null)
            {
                _dbContext.Accounts.Remove(account);
                var result = await _dbContext.SaveChangesAsync();
                if(result ==1) return account.Id;
                else return null;
            }
            else return null;
        }
        
    }
}