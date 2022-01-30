using MongoDB.Bson.Serialization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Debug.Like.A.Scientist
{
    public class SessionManager : ISessionManager
    {
        public ApiSettings ApiSettings { get; } = ApiSettings.Default;
        public IClient Client { get; } = new MongoDbClient();
        public DebugSession CurrentSession { get; private set; }

        public SessionManager()
        {
            BsonClassMap.RegisterClassMap<ValueContext>();
            BsonClassMap.RegisterClassMap<ValueInfo>();
            BsonClassMap.RegisterClassMap<ValueRepresentationInfo>();
            BsonClassMap.RegisterClassMap<ReporterBase>();
            BsonClassMap.RegisterClassMap<ScalarReporter>();
        }

        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public async Task<DebugSession> CloseCurrentSession(CancellationToken token)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                await CurrentSession.Close(token);
                var session = CurrentSession;
                CurrentSession = null;
                return session;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task<DebugSession> OpenSession(
            CancellationToken token,
            string existingSessionId = null,
            DebugSettings settings = null)
        {
            var session = new DebugSession(Client, existingSessionId, settings);
            // await session.Initialize();
            return session;
        }

        private async Task EnsureSessionExists(CancellationToken token)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                if (CurrentSession != null)
                    return;

                var session = await OpenSession(token);
                CurrentSession = session;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public void SendValue(ReporterBase reporter)
        {
            try
            {
                var task = SendValue(reporter, CancellationToken.None);
                if (!task.Wait(ApiSettings.AsyncOperationsTimeout) && !ApiSettings.Silent)
                {
                    throw new TimeoutException($"Timeout on sending the report");
                }
                // TODO: else Log
            }
            catch
            {
                if (!ApiSettings.Silent)
                {
                    throw;
                }
                else
                {
                    // TODO: Log at least
                }
            }
        }

        public async Task SendValue(ReporterBase reporter, CancellationToken token)
        {
            try
            {
                await EnsureSessionExists(token);
                await CurrentSession.SendValue(reporter, token);
            }
            catch
            {
                if (!ApiSettings.Silent)
                {
                    throw;
                }
                else
                {
                    // TODO: Log at least
                }
            }
        }
    }
}
