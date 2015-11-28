using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;

namespace R54IN0
{
    /// <summary>
    /// 재고 현황 CurrentStockWrapping
    /// </summary>
    public class InventoryPipe : InvenPipe<Inventory>
    {
        public InventoryPipe(Inventory inventory) : base(inventory)
        {
        }

        public string Code
        {
            get
            {
                return Inven.SpecificationUUID.Substring(0, 6).ToUpper();
            }
        }
    }
}