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

        public PostoService(IRepository<Posto> repositorioPosto, IRepository<PostoParaAtualizar> repositorioPostoParaAtualizar)
        {
            _repositorioPosto = repositorioPosto;
            _repositorioPostoParaAtualizar = repositorioPostoParaAtualizar;
        }

        public async Task GerarMassaEmPostos()
        {
            var postos = _repositorioPosto.GetAll();
            var temValor = postos.Any();

            if (temValor)
                await _repositorioPosto.RemoveAsync(postos);

            var lista = new List<Posto>();

            for(int i = 0; i < 5000; i++)
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

        public async Task GerarMassaEmPostosParaAtualizar()
        {

            var postosParaAtualizar = _repositorioPostoParaAtualizar.GetAll();
            var temValor = postosParaAtualizar.Any();

            if (temValor)
                await _repositorioPostoParaAtualizar.RemoveAsync(postosParaAtualizar);

            var lista = new List<PostoParaAtualizar>();

            for (int i = 0; i < 5000; i++)
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
    }
}
