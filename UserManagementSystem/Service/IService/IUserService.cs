using Models;
using Models.DTOs;

namespace UserManagementSystem.Service.IService
{
    public interface IUserService
    {
        Task<List<User>> ReturnUsers();
        Task<RegistrationResult> RegisterNewUser(RegistrationDTO registrationDTO);
        Task<LoggedInStatus> LoginUser(LoginDTO loginDTO);
        Task<bool> DeleteUser(Guid id);
    }
}