namespace Debug.Like.A.Scientist
{
    public class Series<TIndexType, TValueType>
    {
        public TIndexType[] Index { get; set; }
        public TValueType[] Values { get; set; }
    }
}
