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
    public class PostoService : IPostoService
    {
        private readonly IRepository<Posto> _repositorioPosto;
        private readonly IRepository<PostoParaAtualizar> _repositorioPostoParaAtualizar;
        private readonly IProcessamentoService _processamentoService;
        private readonly ILogger _logger;

        public PostoService
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

        public async Task AtualizarPostos()
        {
            Processamento processamento = await _processamentoService.RegistrarProcessamento();
            var status = StatusProcessamento.Sucesso;

            try
            {
                _logger.Information("Iniciado processo de atualização de postos");

                await DesativarPostosAtuais();
                await InserirNovosPostos();
                await AtualizarRollbackNovosPostos();
                await DesativarRollbackPostosAntigos();

                _logger.Information("Processo de atualização de postos finalizado com sucesso");
            }
            catch(Exception ex)
            {
                _logger.Error($"Houve o seguinte erro: {ex.Message}");
                _logger.Information("Iniciando processo de rollback");

                await FazerRollback();
                status = StatusProcessamento.Falha;

                _logger.Information("Processo de rollback concluído");
            }
            finally
            {
                _logger.Information("Removendo postos da tabela PostosParaAtualizar");
                await RemoverPostosParaAtualizar();

                string textoStatus = status == StatusProcessamento.Sucesso ? "sucesso" : "falha";
                _logger.Information($"Finalizando processamento com status: {textoStatus}");
                await _processamentoService.AtualizarProcessamento(processamento, status);
            }
        }

        #region [Comandos de atualização de posto]

        private async Task InserirNovosPostos()
        {
            _logger.Information("Inserindo novos postos");
            IEnumerable<PostoParaAtualizar> postosParaAtualizar = _repositorioPostoParaAtualizar.GetAllActive();
            IEnumerable<Posto> postosNovos = postosParaAtualizar.Select(x => new Posto(x));
            await _repositorioPosto.InsertAsync(postosNovos);
        }

        private async Task DesativarPostosAtuais()
        {
            _logger.Information("Desativando postos atuais");
            var postos = _repositorioPosto.Query().Where(x => x.Ativo && x.Rollback).ToList();
            postos.ForEach(x => x.Ativo = false);

            await _repositorioPosto.UpdateAsync(postos);
        }

        private async Task RemoverPostosParaAtualizar()
        {
            var postos = _repositorioPostoParaAtualizar.Query().Where(x => x.Ativo).ToList();
            await _repositorioPostoParaAtualizar.RemoveAsync(postos);
        }
        #endregion

        #region [Métodos de Rollback e atualização de propriedade rollback]
        public async Task AtualizarRollbackNovosPostos()
        {
            _logger.Information("Atualizando informação de rollback dos novos postos");
            List<Posto> postos = PegarNovosPostosProcessando();
            postos.ForEach(x => x.Rollback = true);
            await _repositorioPosto.UpdateAsync(postos);
        }

        public async Task DesativarRollbackPostosAntigos()
        {
            _logger.Information("Desativando rollback dos postos antigos");
            List<Posto> postos = PegarAntigosPostosDesativados();
            postos.ForEach(x => x.Rollback = false);
            await _repositorioPosto.UpdateAsync(postos);
        }

        public async Task FazerRollback()
        {
            IEnumerable<Posto> postosNovos = PegarNovosPostosProcessando();
            await _repositorioPosto.RemoveAsync(postosNovos);

            IEnumerable<Posto> postosAntigos = PegarAntigosPostosDesativados();
            foreach(var posto in postosAntigos)
            {
                posto.Ativo = true;
                posto.Rollback = true;
            }
            await _repositorioPosto.UpdateAsync(postosAntigos);
        }
        #endregion

        #region [Métodos utilitários]
        private List<Posto> PegarAntigosPostosDesativados()
        {
            return _repositorioPosto.Query()
                                        .Where(x => !x.Ativo && x.Rollback)
                                        .ToList();
        }

        private List<Posto> PegarNovosPostosProcessando()
        {
            return _repositorioPosto.Query()
                                        .Where(x => x.Ativo && x.Processando)
                                        .ToList();
        }
        #endregion
    }
}
