using Models;

namespace AccountManagementSystem.Services.IServices
{
    public interface IAccountService
    {
        Task<List<Account>> GetAccounts();
    }
}