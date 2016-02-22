using System;

namespace R54IN0
{
    /// <summary>
    /// 자사원
    /// </summary>
    public class Employee : IField
    {
        public const string HEADER = "담당자";

        public Employee()
        {
        }

        public Employee(string name)
        {
            Name = name;
            ID = Guid.NewGuid().ToString();
        }

        public string ID { get; set; }
        public string Name { get; set; }
    }
}