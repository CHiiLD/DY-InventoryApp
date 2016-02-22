using System;

namespace R54IN0
{
    /// <summary>
    /// 제품명
    /// </summary>
    public class Product : IField
    {
        public const string HEADER = "제품";

        public Product()
        {
        }

        public Product(string name)
        {
            Name = name;
            ID = Guid.NewGuid().ToString();
        }

        public string ID { get; set; }
        public string Name { get; set; }
    }
}