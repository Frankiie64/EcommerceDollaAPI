using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Models.Dtos
{
    public class MercanciaPhotoDto
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile Photo { get; set; }

        public int Price { get; set; }
        public int Stock { get; set; }
        public int IdCategoria { get; set; }
        public int IdStore { get; set; }
    }
}
