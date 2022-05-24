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
    [Route("api/stores")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "stores")]

    public class StoresController : Controller
    {
        private readonly IRepositoryStore _repo;
        private readonly IMapper _mapper;

        public StoresController(IRepositoryStore repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<StoresDto>))]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetStores()
        {
            List<Stores> list = await _repo.GetStores();
            List<StoresDto> listDto = new List<StoresDto>();

            foreach (var item in list)
            {
                listDto.Add(_mapper.Map<StoresDto>(item));
            }

            return Ok(listDto);
        }

        [AllowAnonymous]
        [HttpGet("{Id:int}", Name = "getstoresbyid")]
        [ProducesResponseType(200, Type = typeof(List<StoresDto>))]
        [ProducesResponseType(404)]

        public async Task<ActionResult> GetStoresById(int Id)
        {
            Stores item = await _repo.GetStoresById(Id);

            if (item == null)
            {
                return NotFound();
            }

            StoresDto storeDto = _mapper.Map<StoresDto>(item);

            return Ok(storeDto);
        }
        [AllowAnonymous]
        [HttpGet("getstoresbykeyword/{keyword}")]
        [ProducesResponseType(200, Type = typeof(List<StoresDto>))]
        [ProducesResponseType(404)]

        public async Task<ActionResult> GetStoresByKeyword(string keyword)
        {
            try
            {
                var result = await _repo.GetStoresByKeyword(keyword);
                if (result.Any())
                {
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception Ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando datos de la aplicacion");
            }
        }
    }
}
