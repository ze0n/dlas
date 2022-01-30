namespace Debug.Like.A.Scientist
{
    public class DataColumn<TValueType>
    {
        public string Name { get; set; }
        public TValueType[] Values { get; set; }
        public string DataType { get; set; }
    }
}
