using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace R54IN0
{
    public interface IObservableField
    {
        string ID { get; set; }
        string Name { get; set; }
        bool IsDeleted { get; set; }
        IField Field { get; set; }
    }
}