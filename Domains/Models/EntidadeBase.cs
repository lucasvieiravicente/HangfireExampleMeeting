using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExemploMeetingHangfire.Domains.Models
{
    public class EntidadeBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public bool Ativo { get; set; }
    }
}
