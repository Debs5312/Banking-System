using Models.DTOs;

namespace UserManagementSystem.Service.IService
{
    public interface IUserService
    {
        Task<int> RegisterNewUser(RegistrationDTO registrationDTO);
        Task<LoggedInStatus> LoginUser(LoginDTO loginDTO);
    }
}