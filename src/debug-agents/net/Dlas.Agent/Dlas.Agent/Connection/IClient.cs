using System.Threading;
using System.Threading.Tasks;

namespace Debug.Like.A.Scientist
{
    public interface IClient
    {
        Task Connect(CancellationToken token);
        Task Disconnect(CancellationToken token);
        ClientStatus Status { get; }

        Task SendValue(ReporterBase reporter, CancellationToken token);
    }
}
