using System;

namespace Debug.Like.A.Scientist
{
    public class ApiSettings
    {
        public TimeSpan AsyncOperationsTimeout { get; set; }
        public bool Silent { get; internal set; }

        public static ApiSettings Default = new ApiSettings()
        {
            AsyncOperationsTimeout = new TimeSpan(0, 0, 1, 0),
            Silent = true
        };
    }
}
