using ECommerce.Models.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Models
{
    public class Pedidos
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int IdStatus { get; set; }
        [ForeignKey("IdStatus")]
        public Status status { get; set; }
        public int IdOwner { get; set; }
        [ForeignKey("IdOwner")]
        public User Owner { get; set; }
        public int IdProduct { get; set; }
        [ForeignKey("IdProduct")]
        public Mercancia product { get; set; }
        public int CantidadComprada { get; set; }
        public DateTime FechaDeCompra { get; set; }


    }
}
