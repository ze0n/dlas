namespace Debug.Like.A.Scientist
{
    public class DataFrame<TIndexType, TValueType>
    {
        public TIndexType[] Index { get; set; }
        public DataColumn<TValueType>[] Columns { get; set; }
    }
}
