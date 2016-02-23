using System;

namespace R54IN0
{
    /// <summary>
    /// 자사원
    /// </summary>
    public class Employee : IField
    {
        public Employee()
        {
        }
        public bool IsDeleted { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
    }
}