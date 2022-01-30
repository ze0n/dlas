using System.Runtime.Serialization;

namespace Debug.Like.A.Scientist
{
    [DataContract]
    public class ValueInfo
    { 
        public string Id { get; set; }
        public object Value { get; set; }
        public string Instance { get; set; }
    }
}
