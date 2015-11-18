namespace DY.Inven
{
    /// <summary>
    /// 자사원
    /// </summary>
    public class Employee : IBasic
    {
        public string Name { get; set; }
        public string UUID { get; set; }
        public bool IsEnable { get; set; }
    }
}