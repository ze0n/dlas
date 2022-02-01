using System;
using System.Threading;
using System.Threading.Tasks;

namespace Debug.Like.A.Scientist
{
    public class DebugSession : IDebugSession
    {
        public DebugSession(IClient client, string existingSessionId, DebugSettings settings)
        {
            Client = client;
            ExistingSessionId = existingSessionId;
            Settings = settings;

            if (string.IsNullOrWhiteSpace(existingSessionId))
            {
                Id = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-") + Guid.NewGuid().ToString();
            }
            else
            {
                Id = existingSessionId;
            }
        }

        public async Task Close(CancellationToken token)
        {
            if (Client.Status == ClientStatus.Connected)
                return;
            else
                await Client.Disconnect(token);
        }

        public string Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? FinishedAt { get; private set; }
        public DebugSessionStatus Status { get; private set; }

        public IClient Client { get; }
        public string ExistingSessionId { get; }
        public DebugSettings Settings { get; }

        private async Task EnsureClientConnected(CancellationToken token)
        {
            if (Client.Status == ClientStatus.Connected)
                return;
            else
                await Client.Connect(token);
        }

        internal async Task SendValue(ReporterBase reporter, CancellationToken token)
        {
            await EnsureClientConnected(token);
            reporter.Context.SessionId = Id;
            await Client.SendValue(reporter, token);
        }
    }
}
