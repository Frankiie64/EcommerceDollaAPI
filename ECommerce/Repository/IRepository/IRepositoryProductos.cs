using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Repository.IRepository
{
    public interface IRepositoryProductos
    {
        public Task<List<Mercancia>> GetMercancia();
        public Task<IEnumerable<Mercancia>> GetMercanciaByKeyword(string keyword);
        public Task<Mercancia> GetMercanciaById(int id);
        public Task<bool> CreateMercancia(Mercancia producto);
        public Task<bool> ExisteMercancia(string producto);

        public Task<bool> Save();

    }
}
