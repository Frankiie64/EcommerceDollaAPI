using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repository
{
    public class StoreRepository : IRepositoryStore
    {
        private readonly ApplicationDbContext _db;

        public StoreRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<List<Stores>> GetStores()
        {
            return await _db.store.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<Stores> GetStoresById(int id)
        {
            return await _db.store.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Stores>> GetStoresByKeyword(string keyword)
        {
            IQueryable<Stores> query = _db.store;

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(e => e.Name.Contains(keyword) || e.Address.Contains(keyword)
                || e.City.Contains(keyword) || e.OpeningHours.Contains(keyword));
            }
            return await query.OrderBy(c => c.Name).ToListAsync();
        }
    }
}
