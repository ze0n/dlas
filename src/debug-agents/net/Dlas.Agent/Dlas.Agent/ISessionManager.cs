using System.Threading;
using System.Threading.Tasks;

namespace Debug.Like.A.Scientist
{
    public interface ISessionManager
    {
        Task<DebugSession> OpenSession(CancellationToken token, string id = null, DebugSettings settings = null);
        Task<DebugSession> CloseCurrentSession(CancellationToken token);

        DebugSession CurrentSession { get; }

        void SendValue(ReporterBase reporter);
        Task SendValue(ReporterBase reporter, CancellationToken token);
    }
}
