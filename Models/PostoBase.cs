namespace ExemploMeetingHangfire.Models
{
    public class PostoBase : EntidadeBase
    {
        public string Endereco { get; set; }

        public string Responsavel { get; set; }

        public bool Operando { get; set; }

        public string Cnpj { get; set; }
    }
}
