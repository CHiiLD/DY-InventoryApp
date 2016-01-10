using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public interface IDirectorAction
    {
        void Add(object pipe);
        void Remove(object pipe);
    }
}