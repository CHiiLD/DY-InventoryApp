using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class Maker : IField
    {
        public string UUID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public Maker()
        {

        }

        public Maker(Maker thiz)
        {
            UUID = thiz.UUID;
            Name = thiz.Name;
            IsDeleted = thiz.IsDeleted;
        }
    }
}
