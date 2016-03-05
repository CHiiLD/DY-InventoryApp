using System;

namespace R54IN0
{
    /// <summary>
    /// 납품처, 공급회사
    /// </summary>
    public class Supplier : AField
    {
        public const string HEADER = "구매처";

        public Supplier() : base()
        {
        }

        public Supplier(string name) : base(name)
        {
        }
    }
}