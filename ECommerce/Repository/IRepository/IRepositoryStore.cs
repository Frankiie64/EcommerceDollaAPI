using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Repository.IRepository
{
    public interface IRepositoryStore
    {
        public Task<List<Stores>> GetStores();
        public Task<IEnumerable<Stores>> GetStoresByKeyword(string keyword);
        public Task<Stores> GetStoresById(int id);
    }
}
