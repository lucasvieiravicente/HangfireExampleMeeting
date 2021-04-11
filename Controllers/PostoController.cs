using ExemploMeetingHangfire.Repositories.Interfaces;
using ExemploMeetingHangfire.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ExemploMeetingHangfire.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostoController : ControllerBase
    {
        private readonly IRepository<PostoParaAtualizar> _repositorio;
        public PostoController(IRepository<PostoParaAtualizar> repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpPost]
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
    }
}
