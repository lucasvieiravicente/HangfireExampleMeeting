﻿using ExemploMeetingHangfire.Domains.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExemploMeetingHangfire.Domains.Contexts
{
    public class EFContext : DbContext
    {
        public static readonly LoggerFactory _loggerFactory = new LoggerFactory(new[] { new DebugLoggerProvider() });

        public EFContext(DbContextOptions<EFContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory);
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Posto> Postos { get; set; }

        public DbSet<PostosParaAtualizar> PostosParaAtualizar { get; set; }
    }
}
