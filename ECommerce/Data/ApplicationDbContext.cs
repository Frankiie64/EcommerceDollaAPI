using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }
       public DbSet<User> user { get; set; }
       public DbSet<Stores> store { get; set; }
       public DbSet<Categoria> categoria { get; set; }
       public DbSet<Mercancia> mercancias { get; set; }
       public DbSet<Pedidos> pedido { get; set; }
       public DbSet<Status> status { get; set; }

    }
}
