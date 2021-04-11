using ExemploMeetingHangfire.Enums;
using ExemploMeetingHangfire.Models;
using ExemploMeetingHangfire.Repositories.Interfaces;
using ExemploMeetingHangfire.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace ExemploMeetingHangfire.Services
{
    public class ProcessamentoService : IProcessamentoService
    {
        private readonly IRepository<Processamento> _repositorioProcessamento;

        public ProcessamentoService(IRepository<Processamento> repositorioProcessamento)
        {
            _repositorioProcessamento = repositorioProcessamento;
        }

        public async Task AtualizarProcessamento(Processamento processamento, StatusProcessamento status)
        {
            if (processamento is null)
                return;

            processamento.Status = status;
            processamento.DataFinalizada = DateTime.Now;

            await _repositorioProcessamento.UpdateAsync(processamento);
        }

        public async Task<Processamento> RegistrarProcessamento()
        {
            var processamento = new Processamento
            {
                Ativo = true,
                DataInicio = DateTime.Now,
                Status = StatusProcessamento.Processando
            };

            await _repositorioProcessamento.InsertAsync(processamento);

            return processamento;
        }
    }
}
