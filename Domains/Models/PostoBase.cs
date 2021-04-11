namespace ExemploMeetingHangfire.Domains.Models
{
    public class PostoBase : EntidadeBase
    {
        public string Endereco { get; set; }

        public string Responsavel { get; set; }

        public bool Operando { get; set; }
    }
}
