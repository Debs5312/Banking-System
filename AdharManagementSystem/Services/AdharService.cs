using AdharManagementSystem.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTOs;
using Persistance;

namespace AdharManagementSystem.Services
{
    public class AdharService : IAdharService
    {
        private readonly AppDBContext _dbContext;

        public AdharService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Adhar> CreateNewAdhar(Adhar adhar)
        {
            Random rnd = new Random();
            adhar.UpdatedDate = DateTime.Now;
            await _dbContext.Adhars.AddAsync(adhar);
            var result = await _dbContext.SaveChangesAsync();
            if(result == 1) return adhar;
            else return null;
        }

        public async Task<List<Adhar>> GetAdhars()
        {
            return await _dbContext.Adhars.AsNoTracking().ToListAsync();
        }

        // public async Task<List<Adhar>> GetAdharsWithRef()
        // {
        //     return await _dbContext.Adhars.AsNoTracking()
        //                     .Include(x => x.AdharCardHolder)
        //                     .ToListAsync();
        // }

        public async Task<Adhar> GetSingleAdhar(Guid id)
        {
            return await _dbContext.Adhars.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Adhar> GetSingleAdharWithRef(Guid id)
        {
            return await _dbContext.Adhars.AsNoTracking()
                            .Include(x => x.AdharCardHolder)
                            .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Adhar> UpdateAdhar(Guid id, int updatedNumber)
        {
            var adhar = await _dbContext.Adhars.FirstOrDefaultAsync(x => x.Id == id);
            adhar.Number = updatedNumber;
            adhar.UpdatedDate = DateTime.Now;
            var result = await _dbContext.SaveChangesAsync();
            if(result ==1) return adhar;
            else return null;
        }

        public async Task<Guid?> DeleteAdhar(Guid id)
        {
            var adhar = await _dbContext.Adhars.FirstOrDefaultAsync(x => x.Id == id);
            if (adhar != null)
            {
                _dbContext.Adhars.Remove(adhar);
                var result = await _dbContext.SaveChangesAsync();
                if(result ==1) return adhar.Id;
                else return null;
            }
            else return null;
        }
    }
}