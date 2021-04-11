using Hangfire;
using System.Threading.Tasks;

namespace ExemploMeetingHangfire.Services.Interfaces
{
    public interface IPostoService
    {
        Task AtualizarPostos();
    }
}
