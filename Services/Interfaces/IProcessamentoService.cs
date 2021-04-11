using ExemploMeetingHangfire.Enums;
using ExemploMeetingHangfire.Models;
using System.Threading.Tasks;

namespace ExemploMeetingHangfire.Services.Interfaces
{
    public interface IProcessamentoService
    {
        Task AtualizarProcessamento(Processamento processamento, StatusProcessamento status);

        Task<Processamento> RegistrarProcessamento();
    }
}
