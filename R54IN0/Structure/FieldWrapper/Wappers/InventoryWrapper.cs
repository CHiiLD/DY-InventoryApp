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
    public class InventoryWrapper : RecordWrapper<Inventory>
    {
        public InventoryWrapper() : base()
        {
        }

        public InventoryWrapper(Inventory inventory)
            : base(inventory)
        {
        }

        public string Code
        {
            get
            {
                return Record.ItemUUID != null ? Record.ItemUUID.Substring(0, 6).ToUpper() : "";
            }
        }

        public string SubCode
        {
            get
            {
                return Record.SpecificationUUID != null ? Record.SpecificationUUID.Substring(0, 6).ToUpper() : "";
            }
        }

        public override string Remark
        {
            get
            {
                return Specification.Remark;
            }
        }
    }
}