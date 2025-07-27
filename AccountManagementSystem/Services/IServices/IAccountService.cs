using Models;
using Models.DTOs;

namespace AccountManagementSystem.Services.IServices
{
    /// <summary>
    /// Interface for Account Service defining account-related operations.
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Gets the list of accounts including related references.
        /// </summary>
        /// <returns>List of accounts with references.</returns>
        Task<List<Account>> GetAccountsWithRef();

        /// <summary>
        /// Gets the list of all accounts.
        /// </summary>
        /// <returns>List of accounts.</returns>
        Task<List<Account>> GetAccounts();

        /// <summary>
        /// Gets a single account by ID including related references.
        /// </summary>
        /// <param name="id">Account ID.</param>
        /// <returns>Account with references.</returns>
        Task<Account> GetSingleAccountWithRef(Guid id);

        /// <summary>
        /// Gets a single account by ID.
        /// </summary>
        /// <param name="id">Account ID.</param>
        /// <returns>Account.</returns>
        Task<Account> GetSingleAccount(Guid id);

        /// <summary>
        /// Creates a new account.
        /// </summary>
        /// <param name="account">Account to create.</param>
        /// <returns>Created account.</returns>
        Task<Account> CreateNewAccount(Account account);

        /// <summary>
        /// Updates an existing account.
        /// </summary>
        /// <param name="id">Account ID.</param>
        /// <param name="updatedAccount">Updated account data.</param>
        /// <returns>Updated account.</returns>
        Task<Account> UpdateAccount(Guid id, UpdateAccountDTO updatedAccount);

        /// <summary>
        /// Deletes an account by ID.
        /// </summary>
        /// <param name="id">Account ID.</param>
        /// <returns>Deleted account ID or null.</returns>
        Task<Guid?> DeleteAccount(Guid id);
    }
}
