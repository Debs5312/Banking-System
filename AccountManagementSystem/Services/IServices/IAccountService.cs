using Models;
using Models.DTOs;

namespace AccountManagementSystem.Services.IServices
{
    public interface IAccountService
    {
        Task<List<Account>> GetAccountsWithRef();
        Task<List<Account>> GetAccounts();
        Task<Account> GetSingleAccountWithRef(Guid id);
        Task<Account> GetSingleAccount(Guid id);
        Task<Account> CreateNewAccount(Account account);
        Task<Account> UpdateAccount(Guid id, UpdateAccountDTO updatedAccount);
        Task<Guid?> DeleteAccount(Guid id);
    }
}