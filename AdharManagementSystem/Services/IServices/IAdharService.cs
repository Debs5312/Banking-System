using Models;
using Models.DTOs;

namespace AdharManagementSystem.Services.IServices
{
    public interface IAdharService
    {
        Task<List<Adhar>> GetAdhars();
        Task<Adhar> GetSingleAdhar(Guid id);
        Task<Adhar> CreateNewAdhar(Adhar adhar);
        Task<Adhar> UpdateAdhar(Guid id, int updatedNumber);
        Task<Guid?> DeleteAdhar(Guid id);
    }
}