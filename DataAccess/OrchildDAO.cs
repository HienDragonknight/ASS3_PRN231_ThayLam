using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViroCureDAL.basic;

namespace DataAccess
{
    public class OrchildDAO : GenericRepository<Orchid>
    {
        public OrchildDAO(MyNewStoreDbContext context) : base(context)
        {
        }


        public async Task<Orchid> CreateOrchidAsync(Orchid Orchid)
        {
            _context.Orchids.Add(Orchid);
            await _context.SaveChangesAsync();
            return Orchid;
        }

        public async Task<bool> OrchidExistsAsync(int OrchidId)
        {
            return await _context.Orchids.Include(p=> p.Category).AnyAsync(p => p.OrchidId == OrchidId);
        }

        public async Task<Orchid?> GetOrchidWithVirusesAsync(int OrchidId)
        {
            return await _context.Orchids
                .Include(p => p.Category)
               
                .FirstOrDefaultAsync(p => p.OrchidId == OrchidId);
        }

        public async Task<List<Orchid>> GetAllOrchidsWithVirusesAsync()
        {
            return await _context.Orchids
                .Include(p => p.Category)
                
                .ToListAsync();
        }

        public async Task<Orchid> UpdateOrchidAsync(Orchid Orchid)
        {
            var existingOrchid = await _context.Orchids
                 .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.OrchidId == Orchid.OrchidId);

            if (existingOrchid == null)
                throw new InvalidOperationException("Orchid not found");

            existingOrchid.OrchidDescription = Orchid.OrchidDescription;
            existingOrchid.OrchidName = Orchid.OrchidName;
            existingOrchid.OrchidUrl = Orchid.OrchidUrl;
            existingOrchid.Price = Orchid.Price;
            existingOrchid.IsNatural = Orchid.IsNatural;
            existingOrchid.CategoryId = Orchid.CategoryId;

           

            await _context.SaveChangesAsync();

            var existingOrchidA = await _context.Orchids
                 .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.OrchidId == Orchid.OrchidId);

            existingOrchidA.Category = Orchid.Category;

            await _context.SaveChangesAsync();
            return existingOrchid;
        }

        public async Task<bool> DeleteOrchidAsync(int OrchidId)
        {
            var Orchid = await _context.Orchids
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.OrchidId == OrchidId);

            if (Orchid == null)
                return false;

          

            // Remove Orchid
            _context.Orchids.Remove(Orchid);
            await _context.SaveChangesAsync();
            return true;
        }
    }
    
    
}
