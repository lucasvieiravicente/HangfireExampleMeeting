﻿using ExemploMeetingHangfire.Repositories;
using ExemploMeetingHangfire.Repositories.Interfaces;
using ExemploMeetingHangfire.Models;
using Microsoft.Extensions.DependencyInjection;
using ExemploMeetingHangfire.Services.Interfaces;
using ExemploMeetingHangfire.Services;

namespace ExemploMeetingHangfire.Configs
{
    public static class InjecaoDeDependencia
    {
        public static void InjetarServicos(IServiceCollection services)
        {
            services.AddTransient<IPostoService, PostoService>();
        }

        public static void InjetarRepositorios(IServiceCollection services)
        {
            services.AddTransient<IRepository<Posto>, Repository<Posto>>();
            services.AddTransient<IRepository<PostoParaAtualizar>, Repository<PostoParaAtualizar>>();
        }
    }
}
