using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Models.Dtos
{
    public class PedidosDto
    {
        public string Name { get; set; }
        public int CantidadComprada { get; set; }
        public MercanciaCompradaDto product { get; set; }
        public UserDto Owner { get; set; }
        public StatusDto status { get; set; }

        public DateTime FechaDeCompra { get; set; }

    }
}
