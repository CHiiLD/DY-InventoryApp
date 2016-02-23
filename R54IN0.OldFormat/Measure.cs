using System;

namespace R54IN0
{
    /// <summary>
    /// 단위
    /// </summary>
    public class Measure : IField
    {
        public Measure()
        {
        }
        public bool IsDeleted { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
    }
}