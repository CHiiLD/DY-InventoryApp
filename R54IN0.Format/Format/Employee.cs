using System;

namespace R54IN0
{
    /// <summary>
    /// 자사원
    /// </summary>
    public class Employee : AField
    {
        public const string HEADER = "담당자";

        public Employee() : base()
        {
        }

        public Employee(string name) : base(name)
        {
        }
    }
}