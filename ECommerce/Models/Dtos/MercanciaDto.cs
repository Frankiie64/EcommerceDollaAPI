using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Models.Dtos
{
    public class MercanciaDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagenRoute { get; set; }

        public int Price { get; set; }
        public int Stock { get; set; }
        public CategoriaDto Categoria { get; set; }
        public StoresDto Store { get; set; }

    }
}
