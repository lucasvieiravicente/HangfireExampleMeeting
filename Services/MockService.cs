using ExemploMeetingHangfire.Enums;
using ExemploMeetingHangfire.Models;
using ExemploMeetingHangfire.Repositories.Interfaces;
using ExemploMeetingHangfire.Services.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExemploMeetingHangfire.Services
{
    public class MockService : IMockService
    {
        private readonly IRepository<Posto> _repositorioPosto;
        private readonly IRepository<PostoParaAtualizar> _repositorioPostoParaAtualizar;
        private readonly IProcessamentoService _processamentoService;
        private readonly ILogger _logger;

        public MockService
        (
            IRepository<Posto> repositorioPosto, 
            IRepository<PostoParaAtualizar> repositorioPostoParaAtualizar,
            IProcessamentoService processamentoService,
            ILogger logger
        )
        {
            _repositorioPosto = repositorioPosto;
            _repositorioPostoParaAtualizar = repositorioPostoParaAtualizar;
            _processamentoService = processamentoService;
            _logger = logger;
        }

        public async Task GerarMassas()
        {
            Processamento processamento = null;
            var status = StatusProcessamento.Sucesso;

            try
            {
                _logger.Information("Iniciando geração de massa");

                processamento = await _processamentoService.RegistrarProcessamento();
                await GerarMassaEmPostos();
                await GerarMassaEmPostosParaAtualizar();

                _logger.Information("Geração de massa concluída com sucesso");
            }
            catch(Exception ex)
            {
                _logger.Error($"Houve o seguinte erro: {ex.Message}");
                _logger.Information("Iniciando processo de remoção");

                await RemoverValoresExistentes(_repositorioPosto);
                await RemoverValoresExistentes(_repositorioPostoParaAtualizar);
                status = StatusProcessamento.Falha;

                _logger.Information("Processo de remoção concluído");
            }
            finally
            {
                await _processamentoService.AtualizarProcessamento(processamento, status);

                string textoStatus = status == StatusProcessamento.Sucesso ? "sucesso" : "falha";
                _logger.Information($"Finalizando processamento com status: {textoStatus}");
            }
        }

        private async Task GerarMassaEmPostos()
        {
            _logger.Information("Gerando massa na tabela Postos");

            await RemoverValoresExistentes(_repositorioPosto);

            var lista = new List<Posto>();

            for (int i = 0; i < 500; i++)
            {
                var cnpj = 500 + i;

                lista.Add(new Posto()
                {
                    Cnpj = cnpj.ToString(),
                    Endereco = $"Rua Exemplo {i}",
                    Operando = true,
                    Responsavel = $"Fulano {i}",
                    Rollback = true,
                    Ativo = true
                });
            }

            await _repositorioPosto.InsertAsync(lista);
        }

        private async Task GerarMassaEmPostosParaAtualizar()
        {
            _logger.Information("Gerando massa na tabela PostosParaAtualizar");

            await RemoverValoresExistentes(_repositorioPostoParaAtualizar);

            var lista = new List<PostoParaAtualizar>();

            for (int i = 0; i < 500; i++)
            {
                var cnpj = 500 + i;

                lista.Add(new PostoParaAtualizar()
                {
                    Cnpj = cnpj.ToString(),
                    Endereco = $"Rua Exemplo para atualizar {i}",
                    Operando = true,
                    Responsavel = $"Fulano para atualizar {i}",
                    Processado = false,
                    Ativo = true
                });
            }

            await _repositorioPostoParaAtualizar.InsertAsync(lista);
        }

        private async Task RemoverValoresExistentes<T>(IRepository<T> repositorio) where T : EntidadeBase
        {
            IEnumerable<T> valores = repositorio.GetAll();
            bool temValor = valores.Any();

            if (temValor)
                await repositorio.RemoveAsync(valores);
        }
    }
}
