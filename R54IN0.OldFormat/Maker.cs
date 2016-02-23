using System;

namespace R54IN0
{
    public class Maker : IField
    {
        public Maker()
        {
        }
        public bool IsDeleted { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
    }
}