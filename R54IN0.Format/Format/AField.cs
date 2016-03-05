using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public abstract class AField : IField
    {
        public const int NAME_LENGTH_LIMIT = 256;
        private string _name;

        protected AField()
        {
        }

        protected AField(string name)
        {
            Name = name;
            ID = Guid.NewGuid().ToString();
        }

        public virtual string ID
        {
            get;
            set;
        }

        public virtual string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = this.CutString(value, NAME_LENGTH_LIMIT);
            }
        }
    }
}