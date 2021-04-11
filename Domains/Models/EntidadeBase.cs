using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExemploMeetingHangfire.Domains.Models
{
    public class EntidadeBase
    {
        public Guid Id { get; set; }

        public bool Ativo { get; set; }
    }
}
