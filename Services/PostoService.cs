using ExemploMeetingHangfire.Enums;
using ExemploMeetingHangfire.Models;
using ExemploMeetingHangfire.Repositories.Interfaces;
using ExemploMeetingHangfire.Services.Interfaces;
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

        public PostoService
        (
            IRepository<Posto> repositorioPosto, 
            IRepository<PostoParaAtualizar> repositorioPostoParaAtualizar,
            IProcessamentoService processamentoService
        )
        {
            _repositorioPosto = repositorioPosto;
            _repositorioPostoParaAtualizar = repositorioPostoParaAtualizar;
            _processamentoService = processamentoService;
        }

        public async Task AtualizarPostos()
        {
            Processamento processamento = await _processamentoService.RegistrarProcessamento();
            var status = StatusProcessamento.Sucesso;

            try
            {
                await DesativarPostosAtuais();
                await InserirNovosPostos();
                await AtualizarRollbackNovosPostos();
                await DesativarRollbackPostosAntigos();
            }
            catch
            {
                await FazerRollback();
                status = StatusProcessamento.Falha;
            }
            finally
            {
                await RemoverPostosParaAtualizar();
                await _processamentoService.AtualizarProcessamento(processamento, status);
            }
        }

        #region [Comandos de atualização de posto]

        private async Task InserirNovosPostos()
        {
            IEnumerable<PostoParaAtualizar> postosParaAtualizar = _repositorioPostoParaAtualizar.GetAllActive();
            IEnumerable<Posto> postosNovos = postosParaAtualizar.Select(x => new Posto(x));
            await _repositorioPosto.InsertAsync(postosNovos);
        }

        private async Task DesativarPostosAtuais()
        {
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
            List<Posto> postos = PegarNovosPostosProcessando();
            postos.ForEach(x => x.Rollback = true);
            await _repositorioPosto.UpdateAsync(postos);
        }

        public async Task DesativarRollbackPostosAntigos()
        {
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
