using ExemploMeetingHangfire.Enums;
using ExemploMeetingHangfire.Models;
using ExemploMeetingHangfire.Repositories.Interfaces;
using ExemploMeetingHangfire.Services.Interfaces;
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

        public MockService
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

        public async Task GerarMassas()
        {
            Processamento processamento = null;
            var status = StatusProcessamento.Sucesso;

            try
            {
                processamento = await _processamentoService.RegistrarProcessamento();
                await GerarMassaEmPostos();
                await GerarMassaEmPostosParaAtualizar();
            }
            catch
            {
                await RemoverValoresExistentes(_repositorioPosto);
                await RemoverValoresExistentes(_repositorioPostoParaAtualizar);
                status = StatusProcessamento.Falha;               
            }
            finally
            {
                await _processamentoService.AtualizarProcessamento(processamento, status);
            }
        }

        private async Task GerarMassaEmPostos()
        {
            await RemoverValoresExistentes(_repositorioPosto);

            var lista = new List<Posto>();

            for (int i = 0; i < 1000; i++)
            {
                var cnpj = 1000 + i;

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
            await RemoverValoresExistentes(_repositorioPostoParaAtualizar);

            var lista = new List<PostoParaAtualizar>();

            for (int i = 0; i < 1000; i++)
            {
                var cnpj = 1000 + i;

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
