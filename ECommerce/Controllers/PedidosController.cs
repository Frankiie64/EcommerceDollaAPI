using AutoMapper;
using ECommerce.Models;
using ECommerce.Models.Dtos;
using ECommerce.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Controllers
{
    [Authorize]
    [Route("api/pedidos")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "pedidos")]

    public class PedidosController : Controller
    {
        private readonly IRepositoryPedidos _repo;
        private readonly IMapper _mapper;

        public PedidosController(IRepositoryPedidos repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet("{IdUser:int}", Name = "getpedidosinuser")]
        [ProducesResponseType(200, Type = typeof(List<Pedidos>))]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetPedidosInUser(int IdUser)
        {
            List<Pedidos> list = await _repo.GetPedidosInUser(IdUser);
            List<PedidosDto> listDto = new List<PedidosDto>();

            foreach (var item in list)
            {
                listDto.Add(_mapper.Map<PedidosDto>(item));
            }

            return Ok(listDto);
        }
        [HttpGet("getpedidosbyidinuser/{IdUser:int}/{IdPedidos:int}")]
        [ProducesResponseType(200, Type = typeof(Pedidos))]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetPedidosByIdInUser(int IdUser, int IdPedidos)
        {
            Pedidos pedido = await _repo.GetPedidosByIdInUser(IdUser, IdPedidos);

            if (pedido == null)
            {
                return NotFound();
            }

            PedidosDto pedidoDto = new PedidosDto();

            pedidoDto = _mapper.Map<PedidosDto>(pedido);

            return Ok(pedidoDto);
        }
        [HttpGet("getPedidosbykeywordinuser/{IdUser:int}/{keyword}")]
        [ProducesResponseType(200, Type = typeof(Pedidos))]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetPedidosByKeywordInUser(int IdUser, string keyword)
        {

            if (!(IdUser != 0))
            {
                return Unauthorized("Debe registrarse para poder ver sus pedidos");
            }

            IEnumerable<Pedidos> pedido = await _repo.GetPedidosByKeywordInUser(keyword, IdUser);

            List<PedidosDto> ResultDto = _mapper.Map<IEnumerable<Pedidos>, List<PedidosDto>>(pedido);

            if (ResultDto.Any())
            {
                return Ok(ResultDto);
            }

            return NotFound();
        }
        [HttpPost("createpedido/{IdUser:int}")]
        [ProducesResponseType(201, Type = typeof(MercanciaDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> CreatePedido([FromBody] PedidosCreateDto mercanciaDto, int IdUser)
        {
            try
            {
                if (mercanciaDto == null)
                {
                    return BadRequest(ModelState);
                }

                bool value = await _repo.ExisteMercancia(mercanciaDto.Name);

                if (!value)
                {
                    ModelState.AddModelError("", "El proudcto que desea comprar no se encuetra disponible en la tienda");
                    return StatusCode(404, ModelState);
                }

                value = await _repo.StocksDisponibles(mercanciaDto.Name, mercanciaDto.CantidadComprada);

                if (!value)
                {
                    ModelState.AddModelError("", "El proudcto que desea comprar esta agotado");
                    return StatusCode(400, ModelState);
                }

                var mercancia = await _repo.GetMercancia(mercanciaDto.Name);

                Pedidos pedidoComprado = new Pedidos();
                {
                    pedidoComprado.Name = "pruebaDeCreacion";
                    pedidoComprado.IdStatus = 1;
                    pedidoComprado.IdOwner = IdUser;
                    pedidoComprado.IdProduct = mercancia.Id;
                    pedidoComprado.CantidadComprada = mercanciaDto.CantidadComprada;
                    pedidoComprado.FechaDeCompra = DateTime.Today;
                }

                bool response = await _repo.CreatePedidos(pedidoComprado);
                bool update = await _repo.UpdateMercancia(pedidoComprado.CantidadComprada, pedidoComprado.IdProduct);

                if (!response || !update)
                {
                    return BadRequest("Ha ocurrido un problema a la hora de realizar el pedido, intentelo de nuevo y si el error " +
                        "persiste por favor comunicarse con el servicio tecnico.");
                }

                User user = await _repo.FindEmail(IdUser);

                if(user == null)
                {
                    return BadRequest("Ha ocurrido un problema a la hora de realizar la confirmacion del pedido, dirigase hacia la pagina de pedidos " +
                        "para comprobar que el pedido no se haya hecho.");
                }

                string mensaje = $"Se ha creado con éxito su pedido, por favor entre en la app y verifique el proceso de envió que hay en curso.\n" +
                    $"\nEl código para tener los detalles del pedido es :\n\ncódigo : {pedidoComprado.Name}.\n\nPara cualquier duda puede contactarse con los administradores" +
                    $" Por esta misma vía, DollaStore les desea un excelente dia.";

                _repo.EnviarCorreo(user.Email,"Se ha creado con exito su pedido",mensaje);

                return Ok("Se ha creado de manera correcta el pedido, puede revisarlo en el apartado de pedidos para confirmarlo.");
            }
            catch
            {
                ModelState.AddModelError("", $"Ha pasado algo con la base de datos, por favor comuniquese con servicio tecnico.");
                return StatusCode(500, ModelState);
            }
        }

        [HttpPatch("cancelarpedido/{IdUser:int}")]
        [ProducesResponseType(201, Type = typeof(MercanciaDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> CancelarPedido([FromBody] PedidosUpdate pedidosUpdate, int IdUser)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pedidosUpdate.NameOfPedido))
                {
                    BadRequest();
                    return StatusCode(400, ModelState);
                }

                bool value = await _repo.ExistePedido(pedidosUpdate.NameOfPedido);

                if (!value)
                {
                    return NotFound("El pedido que desea cancelar no existe o ya fue cancelado.");
                }

                Pedidos pedido = await _repo.GetPedidosByNameIdInUser(pedidosUpdate.NameOfPedido, IdUser);

                if (pedido == null)
                {
                    return NotFound();
                }

                if (pedido.IdStatus != 1)
                {
                    return BadRequest("El pedido que desea cancelar, ya ha sido alterado visite el apartado de pedidos para ver la situacion actual del mismo.");
                }
                pedido.IdStatus = 3;

                value = await _repo.UpdatePedido(pedido);

                if (!value)
                {
                    return BadRequest("El pedido que desea cancelar no existe o ya fue cancelado.");
                }

                User user = await _repo.FindEmail(IdUser);

                if (user == null)
                {
                    return BadRequest("Ha ocurrido un problema a la hora de realizar la cancelacion del pedido, dirigase hacia la pagina de pedidos " +
                        "para comprobar que el pedido no se haya cancelado.");
                }

                string mensaje = $"Se ha cancelado su pedido con el codigo: {pedidosUpdate.NameOfPedido}, si no conoce de este movimiento por favor " +
                    $"contactenos los antes posible de lo contrario puede verificar el estado del pedido en el historial de pedidos.";

                _repo.EnviarCorreo(user.Email, "!!SE HA CANCELADO SU PEDIDO!!", mensaje);
                return Ok("El pedido se ha cancelado de manera correcta por favor revisa la ventana de pedidos.");
            }
            catch
            {
                ModelState.AddModelError("", $"Ha pasado algo con la base de datos, por favor comuniquese con servicio tecnico.");
                return StatusCode(500, ModelState);
            }
        }
        [HttpPatch("completarpedido/{IdUser:int}")]
        [ProducesResponseType(201, Type = typeof(MercanciaDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> CompletarPedido([FromBody] PedidosUpdate pedidosUpdate, int IdUser)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pedidosUpdate.NameOfPedido))
                {
                    BadRequest();
                    return StatusCode(400, ModelState);
                }

                bool value = await _repo.ExistePedido(pedidosUpdate.NameOfPedido);

                if (!value)
                {
                    return NotFound("El pedido que desea cancelar no existe o ya fue cancelado.");
                }

                Pedidos pedido = await _repo.GetPedidosByNameIdInUser(pedidosUpdate.NameOfPedido, IdUser);

                if (pedido == null)
                {
                    return NotFound();
                }

                if (pedido.IdStatus != 1)
                {
                    return BadRequest("El pedido que desea completar, ya ha sido alterado visite el apartado de pedidos para ver la situacion actual del mismo.");
                }
                pedido.IdStatus = 2;

                value = await _repo.UpdatePedido(pedido);

                if (!value)
                {
                    return BadRequest("El pedido que desea cancelar no existe o ya fue cancelado.");
                }

                User user = await _repo.FindEmail(IdUser);

                if (user == null)
                {
                    return BadRequest("Ha ocurrido un problema a la hora de realizar la entrega del pedido, dirigase hacia la pagina de pedidos " +
                        "para comprobar que el pedido no se haya entregado.");
                }

                string mensaje = $"Se ha entregado de manera exitosa su pedido con el código: {pedidosUpdate.NameOfPedido}, gracias por su compra\n" +
                    $"\nDollaStore se despide esperando su próxima visita, pase feliz resto del día.";

                _repo.EnviarCorreo(user.Email, "Se ha completado con exito su pedido", mensaje);

                return Ok("El pedido se ha completado de manera correcta por favor revisa la ventana de pedidos.");
            }
            catch
            {
                ModelState.AddModelError("", $"Ha pasado algo con la base de datos, por favor comuniquese con servicio tecnico.");
                return StatusCode(500, ModelState);
            }
        }
        [HttpDelete("elimiarpedidocancelados/{IdUser:int}")]
        [ProducesResponseType(201, Type = typeof(MercanciaDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> ElimiarPedidoCancelados([FromBody] PedidosUpdate pedidoDto, int IdUser)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pedidoDto.NameOfPedido))
                {
                    BadRequest();
                    return StatusCode(400, ModelState);
                }

                bool value = await _repo.ExistePedido(pedidoDto.NameOfPedido);

                if (!value)
                {
                    return NotFound("El pedido que desea cancelar no existe o ya fue cancelado.");
                }

                Pedidos pedido = await _repo.GetPedidosByNameIdInUser(pedidoDto.NameOfPedido,IdUser);

                if (pedido == null)
                {
                    return NotFound();
                }

                value = await _repo.DeletePedido(pedido);

                if (!value)
                {
                    return BadRequest("El pedido que desea cancelar no existe o ya fue cancelado.");
                }

                return Ok("El pedido se ha eliminado de manera correcta por favor revisa la ventana de pedidos y de permanecer ahi por favor comuniquese con el personal.");
            }
            catch
            {
                ModelState.AddModelError("", $"Ha pasado algo con la base de datos, por favor comuniquese con servicio tecnico.");
                return StatusCode(500, ModelState);
            }

        }
    }
}
