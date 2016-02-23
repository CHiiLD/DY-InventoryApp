using System;

namespace R54IN0
{
    /// <summary>
    /// 보관 장소 및 창고
    /// </summary>
    public class Warehouse : IField
    {
        public Warehouse()
        {
        }
        public bool IsDeleted { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
    }
}