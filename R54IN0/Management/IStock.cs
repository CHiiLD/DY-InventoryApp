using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public interface IStock
    {
        string SpecificationUUID { get; set; }
        string WarehouseUUID { get; set; }
        int ItemCount { get; set; }
    }
}