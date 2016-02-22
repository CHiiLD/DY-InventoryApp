using System;

namespace R54IN0
{
    /// <summary>
    /// 납품처, 공급회사
    /// </summary>
    public class Supplier : IField
    {
        public const string HEADER = "구매처";

        public Supplier()
        {
        }

        public Supplier(string name)
        {
            Name = name;
            ID = Guid.NewGuid().ToString();
        }

        public string ID { get; set; }
        public string Name { get; set; }
    }
}