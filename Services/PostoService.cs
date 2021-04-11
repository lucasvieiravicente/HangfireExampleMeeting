using ExemploMeetingHangfire.Enums;
using ExemploMeetingHangfire.Models;
using ExemploMeetingHangfire.Repositories.Interfaces;
using ExemploMeetingHangfire.Services.Interfaces;
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
        private readonly IRepository<Processamento> _repositorioProcessamento;

        public PostoService(
            IRepository<Posto> repositorioPosto, 
            IRepository<PostoParaAtualizar> repositorioPostoParaAtualizar,
            IRepository<Processamento> repositorioProcessamento)
        {
            _repositorioPosto = repositorioPosto;
            _repositorioPostoParaAtualizar = repositorioPostoParaAtualizar;
            _repositorioProcessamento = repositorioProcessamento;
        }

        public async Task GerarMassas()
        {
            Processamento processamento = null;

            try
            {
                processamento = await RegistrarProcessamento();
                await GerarMassaEmPostos();
                await GerarMassaEmPostosParaAtualizar();
                await AtualizarProcessamento(processamento, StatusProcessamento.Sucesso);
            }
            catch
            {
                await RemoverValoresExistentes(_repositorioPosto);
                await RemoverValoresExistentes(_repositorioPostoParaAtualizar);
                await AtualizarProcessamento(processamento, StatusProcessamento.Falha);
            }
        }

        private async Task AtualizarProcessamento(Processamento processamento, StatusProcessamento status)
        {
            if (processamento is null)
                return;

            processamento.Status = status;
            processamento.DataFinalizada = DateTime.Now;

            await _repositorioProcessamento.UpdateAsync(processamento);
        }

        private async Task<Processamento> RegistrarProcessamento()
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

        private async Task GerarMassaEmPostos()
        {
            await RemoverValoresExistentes(_repositorioPosto);

            var lista = new List<Posto>();

            for(int i = 0; i < 1000; i++)
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
            var valores = repositorio.GetAll();
            var temValor = valores.Any();

            if (temValor)
                await repositorio.RemoveAsync(valores);
        }
    }
}
