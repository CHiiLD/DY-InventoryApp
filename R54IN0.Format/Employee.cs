namespace R54IN0
{
    /// <summary>
    /// 자사원
    /// </summary>
    public class Employee : IField
    {
        public const string HEADER = "담당자";


        public string ID { get; set; }
        public string Name { get; set; }
    }
}