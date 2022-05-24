using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Repository.IRepository
{
    public interface IRepositoryUser
    {
        public Task<bool> ExistUser(string Username);
        public Task<bool> ExistUser(int id);
        public Task<User> Login(string password, string Username);
        public Task<User> Register(User user, string password);
        public Task<int> FindId(string Username);
        public bool EnviarCorreo(string destinario, string titulo, string cuerpo);
        public Task<bool> Save();


    }

}

