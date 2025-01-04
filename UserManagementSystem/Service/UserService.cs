using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTOs;
using Persistance;
using UserManagementSystem.Service.IService;

namespace UserManagementSystem.Service
{
    public class UserService : IUserService
    {
        private readonly AppDBContext _dbContext;
        private readonly TokenService _tokenService;
        public UserService(AppDBContext dbContext, TokenService tokenService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
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
        public async Task<int> RegisterNewUser(RegistrationDTO registrationDTO)
        {
            if(await _dbContext.Users.AnyAsync(x => x.UserName == registrationDTO.UserName))
            {
                return 1;
            }
            else if(await _dbContext.Adhars.AnyAsync(x => x.Id.ToString() == registrationDTO.AdharId))
            {
                return 2;
            }
            else{
                User newUser = new User();
                newUser.UserName = registrationDTO.UserName;
                newUser.Password = PasswordHashedService.Hash(registrationDTO.HashedPassword);
                newUser.Address = registrationDTO.Address;
                newUser.AdharId = Guid.Parse(registrationDTO.AdharId);
                newUser.RegisteredDate = DateTime.Now;

                await _dbContext.Users.AddAsync(newUser);
                var result = await _dbContext.SaveChangesAsync();
                if(result == 1) return 3;
                else return 4;
            }

        }

        public async Task<LoggedInStatus> LoginUser(LoginDTO loginDTO)
        {
            var user = await _dbContext.Users.FindAsync(loginDTO.UserName);
            var loggedInResult = new LoggedInStatus();
            if(user == null)
            {
                loggedInResult.LoggedIn = false;
                loggedInResult.Token = "";
                loggedInResult.Message = "UserName is wrong!";
                return loggedInResult;
            }
            else
            {
                var passwordMatched = PasswordHashedService.Verify(loginDTO.Password, user.Password);
                if(!passwordMatched)
                {
                    loggedInResult.LoggedIn = false;
                    loggedInResult.Token = "";
                    loggedInResult.Message = "Password not matched";
                }
                else
                {
                    loggedInResult.LoggedIn = true;
                    loggedInResult.Token = _tokenService.CreateToken(user);
                    loggedInResult.Message = "User is logged in succesfully";
                }
                return loggedInResult;
            }
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            var user = await _dbContext.Users
                        .Include(x => x.PrimaryAccount)
                        .Include(x => x.SecondaryAccounts)
                        .Include(x => x.Nominees)
                        .FirstOrDefaultAsync(x => x.Id == id);
            if(user != null)
            {
                if(user.PrimaryAccount != null)
                {
                    var removed = await RemovePrimaryAccount(user.PrimaryAccount);
                    if(removed)
                    {
                        Console.WriteLine($"Primary Account with number {user.PrimaryAccount.AccountNumber} removed for user {user.UserName}");
                    }
                    else
                    {
                        Console.WriteLine($"Account with number {user.PrimaryAccount.AccountNumber} cannot be removed for user {user.UserName}. Kindly check with DBA for more details.");
                        return false;
                    }
                    
                }
                if(user.SecondaryAccounts.Count() > 0)
                {
                    var removed = await RemoveSecondaryUserDetails(user.Id);
                    if(removed)
                    {
                        Console.WriteLine($"User with username {user.UserName} has removed as a Secondary user from single or multiple accounts");
                    }
                    else
                    {
                        Console.WriteLine($"User with username {user.UserName} cannot be removed as a Secondary user from single or multiple accounts");
                        return false;
                    }
                    
                }
                if(user.Nominees.Count() > 0)
                {
                    var removed = await RemoveNomineeUserDetails(user.Id);
                    if(removed)
                    {
                        Console.WriteLine($"User with username {user.UserName} has removed as a nominee user from single or multiple accounts");
                    }
                    else
                    {
                        Console.WriteLine($"User with username {user.UserName} cannot be removed as a nominee user from single or multiple accounts");
                        return false;
                    }
                    
                }
                _dbContext.Users.Remove(user);
                var result = await _dbContext.SaveChangesAsync();
                if(result == 1) return true;
                else return false;
            }
            else return false;
        }

        private async Task<bool> RemovePrimaryAccount(Account account)
        {
            _dbContext.Accounts.Remove(account);
            var result = await _dbContext.SaveChangesAsync();
            if(result == 1) return true;
            return false;
        }

        private async Task<bool> RemoveSecondaryUserDetails(Guid id)
        {
            var accounts = await _dbContext.Accounts.Where(x => x.SecondaryUserId == id).ToListAsync();
            if(accounts.Count() > 0)
            {
                accounts.ForEach(account => account.SecondaryUserId = null);
                var result = await _dbContext.SaveChangesAsync();
                if(result == accounts.Count()) return true;
                return false;
            }
            else return false;
        }

        private async Task<bool> RemoveNomineeUserDetails(Guid id)
        {
            var accounts = await _dbContext.Accounts.Where(x => x.NomineeId == id).ToListAsync();
            if(accounts.Count() > 0)
            {
                accounts.ForEach(account => account.NomineeId = null);
                var result = await _dbContext.SaveChangesAsync();
                if(result == accounts.Count()) return true;
                return false;
            }
            else return false;
        }
    }
}