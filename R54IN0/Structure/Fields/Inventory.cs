using System.Collections.Generic;

namespace R54IN0
{
    /// <summary>
    /// DB재고품목 기록 클래스
    /// </summary>
    public class Inventory : IUUID, IStock
    {
        public string ItemUUID { get; set; }
        public string UUID { get; set; }
        public string SpecificationUUID { get; set; }
        public string WarehouseUUID { get; set; }
        public int Quantity { get; set; }
        public string Remark { get; set; }

        public Inventory()
        {
        }

        public Inventory(Inventory thiz)
        {
            UUID = thiz.UUID;
            SpecificationUUID = thiz.SpecificationUUID;
            WarehouseUUID = thiz.WarehouseUUID;
            Quantity = thiz.Quantity;
            Remark = thiz.Remark;
            ItemUUID = thiz.ItemUUID;
        }

        public object Clone()
        {
            return new Inventory(this);
        }
    }
}