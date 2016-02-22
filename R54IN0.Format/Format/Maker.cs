using System;

namespace R54IN0
{
    public class Maker : IField
    {
        public const string HEADER = "제조사";

        public Maker()
        {
        }

        public Maker(string name)
        {
            Name = name;
            ID = Guid.NewGuid().ToString();
        }

        public string ID { get; set; }
        public string Name { get; set; }
    }
}