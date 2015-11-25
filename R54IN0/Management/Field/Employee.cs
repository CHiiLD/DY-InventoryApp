namespace R54IN0
{
    /// <summary>
    /// 자사원
    /// </summary>
    public class Employee : IBasic
    {
        public string UUID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}