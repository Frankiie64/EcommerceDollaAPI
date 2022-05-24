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
    public class MercanciaRepository : IRepositoryProductos
    {
        private readonly ApplicationDbContext _db;

        public MercanciaRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> CreateMercancia(Mercancia producto)
        {
           await _db.mercancias.AddAsync(producto);
            return await Save();
            
        }

        public async Task<bool> ExisteMercancia(string producto)
        {
            return await _db.mercancias.AnyAsync(p =>
            p.Name.Trim().ToLower() == producto.Trim().ToLower());
        }

        public async Task<List<Mercancia>> GetMercancia()
        {
            return await _db.mercancias.Include(ca => ca.Categoria).Include(st => st.Store)
                .OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<Mercancia> GetMercanciaById(int id)
        {
            return await _db.mercancias.Include(ca => ca.Categoria).Include(st => st.Store)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Mercancia>> GetMercanciaByKeyword(string keyword)
        {
            IQueryable<Mercancia> query = _db.mercancias;

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(e => e.Name.Contains(keyword) || e.Description.Contains(keyword)
                || Convert.ToString(e.Price).Contains(keyword) || e.Store.Name.Contains(keyword) ||
                e.Store.City.Contains(keyword) || e.Store.Address.Contains(keyword) || 
                e.Store.OpeningHours.Contains(keyword) || e.Categoria.Name.Contains(keyword));
            }
            return await query.Include(ca => ca.Categoria).Include(st => st.Store)
                .ToListAsync();           
        }
        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() >= 0 ? true : false;
        }
    }
}
