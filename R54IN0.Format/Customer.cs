using System;

namespace R54IN0
{
    public class Customer : IField
    {
        public const string HEADER = "판매처";

        public Customer()
        {
        }

        public Customer(string name)
        {
            Name = name;
            ID = Guid.NewGuid().ToString();
        }

        public string ID { get; set; }
        public string Name { get; set; }
    }
}