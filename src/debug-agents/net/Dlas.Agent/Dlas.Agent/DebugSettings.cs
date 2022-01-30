using System;

namespace Debug.Like.A.Scientist
{
    public class DebugSettings
    {
        public bool IsAsynchronous { get; set; }
        public bool IsInteractive { get; set; }

        public static DebugSettings Default = new DebugSettings() { 
            IsInteractive = false,
            IsAsynchronous = false
        };
    }
}
