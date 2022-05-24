using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Models.Dtos
{
    public class PedidosCreateDto
    {
        public string Name { get; set; }
        public int CantidadComprada { get; set; }
    }
}
