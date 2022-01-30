using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;

namespace Debug.Like.A.Scientist
{
    [DataContract]
    public class ValueContext
    {
        public string MachineName { get; set; }
        public DateTime Timestamp { get; set; }
        public int ProcessId { get; set; }
        public int ThreadId { get; set; }
        public StackTrace StackTrace { get; set; }
        public string SessionId { get; set; }

        public ValueContext()
        {
            MachineName = Environment.MachineName;
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            ProcessId = Process.GetCurrentProcess().Id;
            Timestamp = DateTime.Now;
            StackTrace = new StackTrace();
        }
    }
}
