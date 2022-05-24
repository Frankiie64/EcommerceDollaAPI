using AutoMapper;
using ECommerce.Models;
using ECommerce.Models.Dtos;
using ECommerce.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Controllers
{
    [Authorize]
    [Route("api/products")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Products")]
    public class ProductsController : Controller
    {
        private readonly IRepositoryProductos _repo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _Hosting;


        public ProductsController(IRepositoryProductos repo, IMapper mapper, IWebHostEnvironment hosting)
        {
            _repo = repo;
            _mapper = mapper;
            _Hosting = hosting;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<Mercancia>))]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetProducts()
        {
            List<Mercancia> list = await _repo.GetMercancia();
            List<MercanciaDto> listDto = new List<MercanciaDto>();

            foreach (var item in list)
            {
                listDto.Add(_mapper.Map<MercanciaDto>(item));
            }

            return Ok(listDto);
        }
        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "getproductbyid")]
        [ProducesResponseType(200, Type = typeof(List<MercanciaDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]

        public async Task<ActionResult> GetProductById(int id)
        {
            Mercancia item = await _repo.GetMercanciaById(id);

            if (item == null)
            {
                return NotFound();
            }

            MercanciaDto mercanciaDto = _mapper.Map<MercanciaDto>(item);

            return Ok(mercanciaDto);
        }

        [AllowAnonymous]
        [HttpGet("getproductbykeyword/{keyword}")]
        [ProducesResponseType(200, Type = typeof(List<Mercancia>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> GetProductByKeyword(string keyword)
        {
            try
            {               
                IEnumerable<Mercancia> result = await _repo.GetMercanciaByKeyword(keyword);

                var ResultDto = _mapper.Map<IEnumerable<Mercancia>, List<MercanciaDto>>(result);


                if (ResultDto.Any())
                {
                    return Ok(ResultDto);
                }
                return NotFound();
            }
            catch (Exception Ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando datos de la aplicacion");
            }
        }
        [HttpPost("createprodcuto")]
        [ProducesResponseType(201, Type = typeof(MercanciaDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> CreateProdcuto([FromForm] MercanciaPhotoDto mercanciaDto)
        {
            try
            {
                if (mercanciaDto == null)
                {
                    return BadRequest(ModelState);
                }

                bool value = await _repo.ExisteMercancia(mercanciaDto.Name);

                if (value)
                {
                    ModelState.AddModelError("", "Este producto ya esta creado ya fue creado.");
                    return StatusCode(404, ModelState);
                }

                var File = mercanciaDto.Photo;
                string PrincipalRoute = _Hosting.WebRootPath;
                var Files = HttpContext.Request.Form.Files;

                Mercancia item = _mapper.Map<Mercancia>(mercanciaDto);

                if (File.Length > 0)
                {
                    var PhotoName = Guid.NewGuid().ToString();
                    var Uploads = Path.Combine(PrincipalRoute, @"Photos");
                    var Extension = Path.GetExtension(Files[0].FileName);

                    using (var fileStreams = new FileStream(Path.Combine(Uploads, PhotoName + Extension), FileMode.Create))
                    {
                        Files[0].CopyTo(fileStreams);
                    }

                    item.ImagenRoute = @"\Photos\" + PhotoName + Extension;
                }

                value = await _repo.CreateMercancia(item);

                if (!value)
                {
                    ModelState.AddModelError("", "Ha pasado algo con la base de datos, por favor comuniquese con el servicio tecnico.");
                    return StatusCode(500, ModelState);
                }

                return CreatedAtRoute("GetProductById", new { id = item.Id }, item);
            }
            catch
            {
                ModelState.AddModelError("", $"Ha pasado algo con la base de datos, por favor comuniquese con servicio tecnico.");
                return StatusCode(500, ModelState);
            }
        }

    }
}
