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
                return loggedInResult;
            }
            else
            {
                var passwordMatched = PasswordHashedService.Verify(loginDTO.Password, user.Password);
                if(!passwordMatched)
                {
                    loggedInResult.LoggedIn = false;
                    loggedInResult.Token = "";
                }
                else
                {
                    loggedInResult.LoggedIn = true;
                    loggedInResult.Token = _tokenService.CreateToken(user);
                }
                return loggedInResult;
            }
        }
    }
}