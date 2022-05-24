using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Repository.IRepository
{
    public interface IRepositoryPedidos
    {
        public Task<List<Pedidos>> GetPedidosInUser(int IdUser);
        public Task<IEnumerable<Pedidos>> GetPedidosByKeywordInUser(string name,int IdUser);
        public Task<Pedidos> GetPedidosByIdInUser(int idUser,int IdPedidos);
        public Task<Pedidos> GetPedidosByNameIdInUser(string name, int IdUser);
        public Task<bool> ExisteMercancia(string name);
        public Task<bool> ExistePedido(string name);
        public Task<bool> StocksDisponibles(string name, int Stocks);
        public Task<Mercancia> GetMercancia(string name);
        public Task<bool> UpdateMercancia(int stocks, int id);
        public Task<bool> UpdatePedido(Pedidos pedido);
        public Task<bool> DeletePedido(Pedidos pedido);
        public Task<bool> CreatePedidos(Pedidos pedidos);
        public bool EnviarCorreo(string destinario, string titulo, string cuerpo);
        public Task<User> FindEmail(int id);
        public Task<bool> Save();
    }
}
