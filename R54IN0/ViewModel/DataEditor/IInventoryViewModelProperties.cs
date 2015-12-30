using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public interface IInventoryViewModelProperties
    {
        IStock Stock { get; set; }
        ItemWrapper Item { get; set; }
        SpecificationWrapper Specification { get; set; }
        Observable<Warehouse> Warehouse { get; set; }
        int Quantity { get; set; }
    }
}