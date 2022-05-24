using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Models
{
    public class Mercancia
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagenRoute { get; set; }

        public int Price { get; set; }
        public int Stock { get; set; }
        public int IdCategoria { get; set; }
        [ForeignKey("IdCategoria")]
        public Categoria Categoria { get; set; }
        public int IdStore { get; set; }
        [ForeignKey("IdStore")]
        public Stores Store { get; set; }
    }
}
