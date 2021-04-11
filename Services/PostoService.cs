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
                var quantidadePostos = PegarQuantidadeDePostosParaAtualizar();
                var quantidadesDeProcessamentos = quantidadePostos / 20;

                for (int i = 0; i < quantidadesDeProcessamentos; i++)
                    await AtualizarPostosComAsNovasInformacoes(i);
            }
            catch
            {
                status = StatusProcessamento.Falha;
            }
            finally
            {
                await _processamentoService.AtualizarProcessamento(processamento, status);
            }
        }

        #region [Comandos de atualização]
        private async Task AtualizarPostosComAsNovasInformacoes(int index)
        {
            IEnumerable<PostoParaAtualizar> postosParaAtualizar = PegarPostosParaAtualizar(index);

            await DesativarPostos(postosParaAtualizar);
            await InserirNovosPostos(postosParaAtualizar);
        }

        private async Task InserirNovosPostos(IEnumerable<PostoParaAtualizar> postosParaAtualizar)
        {
            IEnumerable<Posto> postosNovos = postosParaAtualizar.Select(x => new Posto(x));
            await _repositorioPosto.InsertAsync(postosNovos);
        }

        private async Task DesativarPostos(IEnumerable<PostoParaAtualizar> postosParaAtualizar)
        {
            var cnpjs = postosParaAtualizar.Select(x => x.Cnpj).ToList();
            IEnumerable<Posto> postos = PegarPostosPorCpnj(cnpjs);
            postos.All(x => x.Ativo = false);

            await _repositorioPosto.UpdateAsync(postos);
        }
        #endregion

        #region [Métodos utilitários]

        private IEnumerable<Posto> PegarPostosPorCpnj(List<string> cnpjs)
        {
            return _repositorioPosto.Query()
                                        .Where(x => cnpjs.Contains(x.Cnpj))
                                        .ToList();
        }

        private IEnumerable<PostoParaAtualizar> PegarPostosParaAtualizar(int index)
        {
            int quantidade = 100;
            int quantidadeIgnorada = index * quantidade;

            return _repositorioPostoParaAtualizar.Query()
                                                    .Skip(quantidadeIgnorada)
                                                    .Take(quantidade)
                                                    .ToList();
        }

        private int PegarQuantidadeDePostosParaAtualizar()
        {
            return _repositorioPostoParaAtualizar.GetAllActive().Count();
        }

        #endregion
    }
}
