using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public class VillaRepository : Repository<Villa>,IVillaRepository
    {
        private readonly ApplicationDbContext _db;
        public VillaRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
       

          

        public async Task<Villa> UpdateAsync(Villa entity)
        {
            _db.Update(entity);
            await _db.SaveChangesAsync();

            return entity;
        }

       
    }
}
