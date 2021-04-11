using ExemploMeetingHangfire.Repositories.Interfaces;
using ExemploMeetingHangfire.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ExemploMeetingHangfire.Services.Interfaces;

namespace ExemploMeetingHangfire.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostoController : ControllerBase
    {
        private readonly IRepository<PostoParaAtualizar> _repositorio;
        private readonly IPostoService _postoService;
        public PostoController(IRepository<PostoParaAtualizar> repositorio, IPostoService postoService)
        {
            _repositorio = repositorio;
            _postoService = postoService;
        }

        [HttpPost("InserirPosto")]
        public async Task<ActionResult> InserirPosto([FromBody] PostoParaAtualizar request)
        {
            try
            {
                await _repositorio.InsertAsync(request);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("GerarMassaEmPostos")]
        public async Task<ActionResult> GerarMassaEmPostos()
        {
            try
            {
                await _postoService.GerarMassaEmPostos();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("GerarMassaEmPostosParaAtualizar")]
        public async Task<ActionResult> GerarMassaEmPostosParaAtualizar()
        {
            try
            {
                await _postoService.GerarMassaEmPostosParaAtualizar();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
