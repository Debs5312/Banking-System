using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTOs;
using Persistance;
using Microsoft.Extensions.Logging;
using UserManagementSystem.Service.IService;

namespace UserManagementSystem.Service
{
    public class UserService : IUserService
    {
        private readonly AppDBContext _dbContext;
        private readonly TokenService _tokenService;
        private readonly ILogger<UserService> _logger;
        public UserService(AppDBContext dbContext, TokenService tokenService, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<List<User>> ReturnUsers()
        {
            return await _dbContext.Users
                            .AsNoTracking()
                            .Include(x => x.PrimaryAccount)
                            .Include(x => x.SecondaryAccounts)
                            .Include(x => x.Nominees)
                            .ToListAsync();
        }
        public async Task<RegistrationResult> RegisterNewUser(RegistrationDTO registrationDTO)
        {
            if (await _dbContext.Users.AnyAsync(x => x.UserName == registrationDTO.UserName))
            {
                return RegistrationResult.UserNameExists;
            }

            if (!Guid.TryParse(registrationDTO.AdharId, out var adharGuid))
            {
                return RegistrationResult.InvalidAdharId;
            }

            if (await _dbContext.Adhars.AnyAsync(x => x.Id == adharGuid))
            {
                return RegistrationResult.AdharIdInUse;
            }

            // Note: The DTO property `HashedPassword` is misleading. It should likely be `Password`,
            // as it contains the plain text password to be hashed.
            var newUser = new User
            {
                UserName = registrationDTO.UserName,
                Password = PasswordHashedService.Hash(registrationDTO.HashedPassword),
                Address = registrationDTO.Address,
                AdharId = adharGuid,
                RegisteredDate = DateTime.UtcNow
            };

            await _dbContext.Users.AddAsync(newUser);
            var result = await _dbContext.SaveChangesAsync();

            return result > 0 ? RegistrationResult.Success : RegistrationResult.SaveChangesFailure;
        }

        public async Task<LoggedInStatus> LoginUser(LoginDTO loginDTO)
        {
            // Use FirstOrDefaultAsync for non-primary key lookups. FindAsync is for PKs.
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == loginDTO.UserName);
            var loggedInResult = new LoggedInStatus();

            // Verify password only if user exists. Use a generic error message
            // to prevent username enumeration attacks.
            if (user == null || !PasswordHashedService.Verify(loginDTO.Password, user.Password))
            {
                return new LoggedInStatus
                {
                    LoggedIn = false,
                    Token = string.Empty,
                    Message = "Invalid username or password."
                };
            }

            return new LoggedInStatus
            {
                LoggedIn = true,
                Token = _tokenService.CreateToken(user),
                Message = "User is logged in successfully."
            };
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            var user = await _dbContext.Users
                .Include(x => x.PrimaryAccount)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return false;
            }

            try
            {
                // Stage all changes within the DbContext without saving yet.
                if (user.PrimaryAccount != null)
                {
                    RemovePrimaryAccount(user.PrimaryAccount);
                }

                await RemoveUserAsSecondaryHolder(user.Id);
                await RemoveUserAsNominee(user.Id);

                _dbContext.Users.Remove(user);

                // Save all staged changes in a single atomic transaction.
                var result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogInformation("Successfully deleted user {UserId} and all related account links.", user.Id);
                    return true;
                }

                _logger.LogWarning("Attempted to delete user {UserId}, but no changes were saved to the database.", user.Id);
                return false;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "A database error occurred while trying to delete user {UserId}.", user.Id);
                return false;
            }
        }

        // Helper methods now only stage changes and do not call SaveChangesAsync.
        private void RemovePrimaryAccount(Account account)
        {
            _dbContext.Accounts.Remove(account);
            _logger.LogInformation("Staged primary account {AccountNumber} for deletion for user.", account.AccountNumber);
        }

        private async Task RemoveUserAsSecondaryHolder(Guid id)
        {
            var accounts = await _dbContext.Accounts.Where(x => x.SecondaryUserId == id).ToListAsync();
            if (accounts.Any())
            {
                accounts.ForEach(account =>
                {
                    account.SecondaryUserId = null;
                    _logger.LogInformation("Staged removal of secondary user {UserId} from account {AccountNumber}.", id, account.AccountNumber);
                });
            }
        }

        private async Task RemoveUserAsNominee(Guid id)
        {
            var accounts = await _dbContext.Accounts.Where(x => x.NomineeId == id).ToListAsync();
            if (accounts.Any())
            {
                accounts.ForEach(account =>
                {
                    account.NomineeId = null;
                    _logger.LogInformation("Staged removal of nominee {UserId} from account {AccountNumber}.", id, account.AccountNumber);
                });
            }
        }
    }
}