using System;

namespace R54IN0
{
    /// <summary>
    /// 단위
    /// </summary>
    public class Measure : IField
    {
        public const string HEADER = "단위";

        public Measure()
        {
        }

        public Measure(string name)
        {
            Name = name;
            ID = Guid.NewGuid().ToString();
        }

        public string ID { get; set; }
        public string Name { get; set; }
    }
}