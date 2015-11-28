using System.Collections.Generic;

namespace R54IN0
{
    /// <summary>
    /// DB재고품목 기록 클래스
    /// </summary>
    public class Inventory : IUUID, IInventory
    {
        public string UUID { get; set; }
        public string SpecificationUUID { get; set; }
        public string WarehouseUUID { get; set; }
        public int ItemCount { get; set; }
        public string Remark { get; set; }

        public Inventory()
        {
        }

        public Inventory(Inventory thiz)
        {
            UUID = thiz.UUID;
            SpecificationUUID = thiz.SpecificationUUID;
            WarehouseUUID = thiz.WarehouseUUID;
            ItemCount = thiz.ItemCount;
            Remark = thiz.Remark;
        }

        public object Clone()
        {
            return new Inventory(this);
        }
    }
}