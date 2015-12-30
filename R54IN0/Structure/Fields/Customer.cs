using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class Customer : IField
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}