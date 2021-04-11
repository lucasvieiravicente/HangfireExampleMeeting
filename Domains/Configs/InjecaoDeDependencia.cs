using ExemploMeetingHangfire.Domain.Repositories;
using ExemploMeetingHangfire.Domain.Repositories.Interfaces;
using ExemploMeetingHangfire.Domains.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ExemploMeetingHangfire.Domains.Configs
{
    public static class InjecaoDeDependencia
    {
        public static void InjetarServicos(IServiceCollection services)
        {

        }

        public static void InjetarRepositorios(IServiceCollection services)
        {
            services.AddTransient<IRepository<Posto>, Repository<Posto>>();
            services.AddTransient<IRepository<PostoParaAtualizar>, Repository<PostoParaAtualizar>>();
        }
    }
}
