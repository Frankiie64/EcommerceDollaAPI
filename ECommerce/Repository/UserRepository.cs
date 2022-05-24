using AutoMapper;
using ECommerce.Data;
using ECommerce.Helper;
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
    public class UserRepository : IRepositoryUser
    {
        private readonly ApplicationDbContext _db;
        EmailSender email;        
        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHasd, passwordSalt;

            CrearPassWordHash(password, out passwordHasd, out passwordSalt);

            user.PasswordHash = passwordHasd;
            user.PasswordSalt = passwordSalt;

            await _db.AddAsync(user);

            await Save();

            return user;
        }
        public async Task<User> Login(string password, string Username)
        {
            User item = await _db.user.FirstOrDefaultAsync(x => x.Username == Username);

            if (item == null)
            {
                return null;
            }

            if (!validarPasswordHash(password, item.PasswordHash, item.PasswordSalt))
            {
                return null;
            }

            return item;
        }


        public async Task<bool> ExistUser(string Username)
        {
            return await _db.user.AnyAsync(u => u.Username == Username);
        }

        public async Task<bool> ExistUser(int Id)
        {
            return await _db.user.AnyAsync(u => u.Id == Id);
        }

        public async Task<int> FindId(string Username)
        {
            User user = await _db.user.FirstOrDefaultAsync(u => u.Username.Trim().ToLower() == Username.Trim().ToLower());

            return user.Id;
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() >= 0 ? true : false;
        }
        #region Helper

        //metodos predefinidos para su uso
        private bool validarPasswordHash(string password, byte[] passwordHasd, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var hashComputado = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int x = 0; x < hashComputado.Length; x++)
                {
                    if (hashComputado[x] != passwordHasd[x]) return false;
                }
            }
            return true;
        }
        private void CrearPassWordHash(string password, out byte[] passwordHasd, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHasd = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool EnviarCorreo(string destinario, string titulo, string cuerpo)
        {
            email = new EmailSender();

            return email.EnviarEmail(destinario, titulo, cuerpo);
            
        }

        #endregion
    }
}
