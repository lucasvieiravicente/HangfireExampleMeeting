using Microsoft.AspNetCore.Mvc;
using System;
using ExemploMeetingHangfire.Services.Interfaces;
using Hangfire;
using Serilog;

namespace ExemploMeetingHangfire.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostoController : ControllerBase
    {
        private readonly ILogger _logger;

        public PostoController(ILogger logger)
        {
            _logger = logger;
        }

        [HttpPost("GerarMassas")]
        public ActionResult GerarMassas()
        {
            try
            {
                _logger.Information("Enfileirando geração de massa.");
                BackgroundJob.Enqueue<IMockService>(x => x.GerarMassas());
                _logger.Information("Geração de massa enfileirada com sucesso.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost("AtualizarPostos")]
        public ActionResult AtualizarPostos()
        {
            try
            {
                _logger.Information("Enfileirando Atualização de postos.");
                BackgroundJob.Enqueue<IPostoService>(x => x.AtualizarPostos());
                _logger.Information("Atualização de postos enfileirada com sucesso.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return BadRequest(ex);
            }
        }
    }
}
