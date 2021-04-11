using ExemploMeetingHangfire.Repositories.Interfaces;
using ExemploMeetingHangfire.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ExemploMeetingHangfire.Services.Interfaces;
using Hangfire;

namespace ExemploMeetingHangfire.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostoController : ControllerBase
    {
        [HttpPost("GerarMassas")]
        public ActionResult GerarMassas()
        {
            try
            {
                BackgroundJob.Enqueue<IMockService>(x => x.GerarMassas());
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("AtualizarPostos")]
        public ActionResult AtualizarPostos()
        {
            try
            {
                BackgroundJob.Enqueue<IPostoService>(x => x.AtualizarPostos());
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
