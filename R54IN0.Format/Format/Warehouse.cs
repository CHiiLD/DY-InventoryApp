using System;

namespace R54IN0
{
    /// <summary>
    /// 보관 장소 및 창고
    /// </summary>
    public class Warehouse : AField
    {
        public const string HEADER = "보관장소";

        public Warehouse() : base()
        {
        }

        public Warehouse(string name) : base(name)
        {
        }
    }
}