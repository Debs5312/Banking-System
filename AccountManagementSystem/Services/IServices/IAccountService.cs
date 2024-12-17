using Models;

namespace AccountManagementSystem.Services.IServices
{
    public interface IAccountService
    {
        Task<List<Account>> GetAccounts();
        Task<Account> GetSingleAccount(Guid id);
        Task<Account> CreateNewAccount(Account account);
    }
}