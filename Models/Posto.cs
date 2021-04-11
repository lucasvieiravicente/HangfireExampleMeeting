namespace ExemploMeetingHangfire.Models
{
    public class Posto : PostoBase
    {
        public Posto()
        {

        }

        public Posto(PostoParaAtualizar posto)
        {
            Endereco = posto.Endereco;
            Responsavel = posto.Responsavel;
            Cnpj = posto.Cnpj;
            Operando = posto.Operando;
            Ativo = true;
            Processando = true;
        }

        public bool Rollback { get; set; }

        public bool Processando { get; set; }
    }
}
