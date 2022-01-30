namespace Debug.Like.A.Scientist
{
    public class DlasDebuggerSystem
    {
        public ISessionManager SessionManager { get; }

        public DlasDebuggerSystem()
        {
            SessionManager = new SessionManager();
        }
    }
}
