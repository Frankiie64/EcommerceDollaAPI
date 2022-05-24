using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ECommerce.Helper.Email;

namespace ECommerce.Repository
{
    public class PedidosRepository : IRepositoryPedidos
    {
        private readonly ApplicationDbContext _db;
        EmailSender email;
        public PedidosRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> CreatePedidos(Pedidos pedidos)
        {
            pedidos.Name = CreatingName(pedidos);
            await _db.pedido.AddAsync(pedidos);
            return await Save();         
        }

        public async Task<bool> ExisteMercancia(string name)
        {
             return await _db.mercancias.AnyAsync(pr =>
           pr.Name.Trim().ToLower() == name.Trim().ToLower());
        }
        public async Task<bool> StocksDisponibles(string name,int Stocks)
        {
            Mercancia mercancia = await GetMercancia(name);

            return mercancia.Stock < Stocks ? false : true;

        }

        public async Task<Pedidos> GetPedidosByIdInUser(int IdUser, int IdPedido)
        {
            return await _db.pedido.Include(o => o.Owner).Include(pr => pr.product).Include(pr => pr.product.Store)
            .Include(pr => pr.product.Categoria).Include(o => o.status).FirstOrDefaultAsync(o => o.Id == IdPedido && o.IdOwner == IdUser);
        }
        public async Task<Pedidos> GetPedidosByNameIdInUser(string name , int IdUser)
        {
            return await _db.pedido.Include(o => o.Owner).Include(pr => pr.product).Include(pr => pr.product.Store)
            .Include(pr => pr.product.Categoria).Include(o => o.status).FirstOrDefaultAsync(o => 
            o.Name.Trim().ToLower() == name.Trim().ToLower() && o.IdOwner == IdUser);
        }

        public async Task<IEnumerable<Pedidos>> GetPedidosByKeywordInUser(string keyword, int IdUser)
        {
            IQueryable<Pedidos> query = _db.pedido;

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(e => e.Name.Contains(keyword) || e.Owner.Username.Contains(keyword) ||
               e.Owner.PhoneNumber.Contains(keyword) || e.Owner.Email.Contains(keyword) || e.product.Name.Contains(keyword) ||
               e.product.Description.Contains(keyword) || Convert.ToString(e.product.Price).Contains(keyword) || e.status.Estado.Contains(keyword)
               || e.product.Categoria.Name.Contains(keyword) || e.product.Store.City.Contains(keyword) || e.product.Store.Address.Contains(keyword)
               || e.product.Store.Name.Contains(keyword) || e.product.Store.OpeningHours.Contains(keyword));
            }

            query = query.Where(e => e.IdOwner == IdUser);

            return await query.Include(pr => pr.product).Include(u => u.Owner).Include(pr => pr.product.Categoria)
                .Include(pr => pr.product.Store).Include(o => o.status).ToListAsync();
        }

        public async Task<List<Pedidos>> GetPedidosInUser(int IdUser)
        {
            return await _db.pedido.Where(u => u.IdOwner == IdUser).Include(o => o.Owner).Include(pr => pr.product)
                .Include(pr => pr.product.Categoria).Include(pr => pr.product.Store).Include(o => o.status)
                .OrderBy(o => o.Id).ToListAsync();
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() >= 0 ? true : false;
        }

        public async Task<Mercancia> GetMercancia(string name)
        {
            return await _db.mercancias.FirstOrDefaultAsync(o => o.Name.Trim().ToLower() == name.ToLower().Trim());            
        }
        private async Task<Mercancia> GetMercanciaById(int id)
        {
            return await _db.mercancias.FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<bool> UpdateMercancia(int stocks, int id)
        {
            Mercancia mercancia = await GetMercanciaById(id);

            mercancia.Stock -= stocks;

            _db.mercancias.Update(mercancia);

            return await Save();
        }

        public async Task<bool> UpdatePedido(Pedidos pedido)
        {
            _db.pedido.Update(pedido);
            return await Save();
        }
        public async Task<bool> DeletePedido(Pedidos pedido)
        {
            _db.pedido.Remove(pedido);
            return await Save();
        }
        public Task<bool> ExistePedido(string name)
        {
            return _db.pedido.AnyAsync(p => p.Name.Trim().ToLower() == name.Trim().ToLower());
        }

        private  string CreatingName(Pedidos pedido)
        {
            string name = $"{pedido.IdOwner}"+ Convert.ToString(DateTime.Now);

            string[] charsToRemove = new string[] { "/",":"," ","AM" };

            foreach (var c in charsToRemove)
            {
                name = name.Replace(c, string.Empty);
            }

            return name;
        }

        public bool EnviarCorreo(string destinario, string titulo, string cuerpo)
        {
            email = new EmailSender();

            return email.EnviarEmail(destinario, titulo, cuerpo);

        }

        public async Task<User> FindEmail(int id)
        {
            return await _db.user.FirstOrDefaultAsync(u => u.Id == id);
        }
    } 
}

