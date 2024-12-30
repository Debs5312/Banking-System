using Models.DTOs;

namespace UserManagementSystem.Service.IService
{
    public interface IUserService
    {
        Task<bool> RegisterNewUser(RegistrationDTO registrationDTO);
    }
}