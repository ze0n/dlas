using System;
using System.Collections.Generic;

namespace Debug.Like.A.Scientist
{
    /// <summary>
    /// Entry point
    /// </summary>
    public static class Dlas
    {
        private static readonly DlasDebuggerSystem DlasDebugger = new DlasDebuggerSystem();
        public static ISessionManager SessionManager => DlasDebugger.SessionManager;

        public static string GenerateValueName()
        {
            return Guid.NewGuid().ToString();
        }

        public static ScalarReporter Report(double value, string id, string instance = null)
        {
            return (ScalarReporter) Report(value, id, DataType.Scalar, instance);
        }

        public static ScalarReporter Report(int value, string id, string instance = null)
        {
            return (ScalarReporter)Report(value, id, DataType.Scalar, instance);
        }

        public static VectorReporter Report(IEnumerable<double> value, string id, string instance = null)
        {
            return (VectorReporter)Report(value, id, DataType.Vector, instance);
        }

        public static ReporterBase Report(object value, string id, DataType dataType, string instance = null)
        {
            // TODO: somehow understand is it the same var or not
            if (string.IsNullOrWhiteSpace(id))
            {
                id = GenerateValueName();
            }

            ReporterBase reporter;

            switch (dataType)
            {
                case DataType.Scalar:
                    reporter = new ScalarReporter(SessionManager);
                    break;
                case DataType.Vector:
                    reporter = new VectorReporter(SessionManager);
                    break;
                default:
                    throw new NotImplementedException($"Reporter {dataType} is not implemented");
            }

            reporter.ValueInfo.Id = id;
            reporter.ValueInfo.Instance = instance;
            reporter.ValueInfo.Value = value;

            return reporter;
        }

    }
}
