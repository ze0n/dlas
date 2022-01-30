using System.Runtime.Serialization;

namespace Debug.Like.A.Scientist
{
    [DataContract]
    public class ValueRepresentationInfo 
    {
        public const string AxisValue = "__value__";
        public const string AxisTimestamp = "__timestamp__";
        public const string AxisIteration = "__iteration__";

        public Chart AsChartType { get; set; } = Chart.Unknown; 

        public string X { get; set; } = null;
        public string Y { get; set; } = null;
        public string Z { get; set; } = null;
    }
}
