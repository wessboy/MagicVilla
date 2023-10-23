using MagicVilla_Web.Models.Dtos;

namespace MagicVilla_Web.Services.IServices
{
    public interface IVillaNubmerService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);

        Task<T> CreateAsync<T>(VillaNumberCreatedDTO dto);
        Task<T> UpdateAsync<T>(VillaNumberUpdateDTO dto);

        Task<T> DeleteAsync<T>(int id);
    }
}
