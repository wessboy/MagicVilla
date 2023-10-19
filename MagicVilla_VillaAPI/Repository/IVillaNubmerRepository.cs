using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI.Repository
{
    public interface IVillaNubmerRepository : IRepository<VillaNumber>
    {
        Task<VillaNumber> UpdateAsync(VillaNumber entity);
    }
}
