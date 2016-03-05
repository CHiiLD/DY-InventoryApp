using System;

namespace R54IN0
{
    /// <summary>
    /// 제품명
    /// </summary>
    public class Product : AField
    {
        public const string HEADER = "제품";

        public Product() : base()
        {
        }

        public Product(string name) : base(name)
        {
        }
    }
}