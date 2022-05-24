using AutoMapper;
using ECommerce.Models;
using ECommerce.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Mapper
{
    public class ECommerceMapper : Profile
    {
        public ECommerceMapper()
        {
            CreateMap<User,UserRegisterDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Stores, StoresDto>().ReverseMap();
            CreateMap<Mercancia,MercanciaDto>().ReverseMap();
            CreateMap<Mercancia, MercanciaPhotoDto>().ReverseMap();
            CreateMap<Mercancia, MercanciaCompradaDto>().ReverseMap();
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            CreateMap<Pedidos,PedidosDto>().ReverseMap();
            CreateMap<Status, StatusDto>().ReverseMap();
        }
    }
}
