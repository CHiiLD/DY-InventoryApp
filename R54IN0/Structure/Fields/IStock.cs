using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public interface IStock : ICloneable
    {
        string ItemUUID { get; set; }
        string SpecificationUUID { get; set; }
        string WarehouseUUID { get; set; }
        int Quantity { get; set; }
        string Remark { get; set; }
    }
}