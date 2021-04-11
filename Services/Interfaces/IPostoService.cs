using System.Threading.Tasks;

namespace ExemploMeetingHangfire.Services.Interfaces
{
    public interface IPostoService
    {
        Task GerarMassaEmPostos();

        Task GerarMassaEmPostosParaAtualizar();
    }
}
