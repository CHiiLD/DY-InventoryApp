using System;

namespace R54IN0
{
    public class Customer : IField
    {
        public Customer()
        {
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}