using ExemploMeetingHangfire.Repositories;
using ExemploMeetingHangfire.Repositories.Interfaces;
using ExemploMeetingHangfire.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ExemploMeetingHangfire.Configs
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
