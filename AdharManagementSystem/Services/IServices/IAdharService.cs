using Models;
using Models.DTOs;

namespace AdharManagementSystem.Services.IServices
{
    public interface IAdharService
    {
        Task<List<Adhar>> GetAdhars();
        Task<Adhar> GetSingleAdhar(int adharNum);
        Task<Adhar> CreateNewAdhar(int adharNumber);
        Task<Adhar> UpdateAdhar(Guid id, int updatedNumber);
        Task<Guid?> DeleteAdhar(Guid id);
    }
}