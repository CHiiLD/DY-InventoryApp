using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Lib
{
    public interface IStock
    {
        string SpecificationUUID { get; set; }
        string WarehouseUUID { get; set; }
        int ItemCount { get; set; }
    }
}