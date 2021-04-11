using ExemploMeetingHangfire.Enums;
using System;

namespace ExemploMeetingHangfire.Models
{
    public class Processamento : EntidadeBase
    {
        public StatusProcessamento Status { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataFinalizada { get; set; }
    }
}
