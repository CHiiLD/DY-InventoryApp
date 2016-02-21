using System;

namespace R54IN0
{
    /// <summary>
    /// 보관 장소 및 창고
    /// </summary>
    public class Warehouse : IField
    {
        public const string HEADER = "보관장소";

        public Warehouse()
        {
        }

        public Warehouse(string name)
        {
            Name = name;
            ID = Guid.NewGuid().ToString();
        }

        public string ID { get; set; }
        public string Name { get; set; }
    }
}