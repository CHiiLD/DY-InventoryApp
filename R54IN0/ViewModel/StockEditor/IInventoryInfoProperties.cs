using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public interface IInventoryInfoProperties
    {
        ItemWrapper Item { get; set; }
        SpecificationWrapper Specification { get; set; }
        FieldWrapper<Warehouse> Warehouse { get; set; }
        int ItemCount { get; set; }
    }
}