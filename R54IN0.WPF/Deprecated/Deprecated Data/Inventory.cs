namespace R54IN0
{
    /// <summary>
    /// DB재고품목 기록 클래스
    /// </summary>
    public class Inventory : IID, IStock
    {
        public string ItemID { get; set; }
        public string ID { get; set; }
        public string SpecificationID { get; set; }
        public string WarehouseID { get; set; }
        public int Quantity { get; set; }
        public string Remark { get; set; }

        public Inventory()
        {
        }

        public Inventory(Inventory thiz)
        {
            ID = thiz.ID;
            SpecificationID = thiz.SpecificationID;
            WarehouseID = thiz.WarehouseID;
            Quantity = thiz.Quantity;
            Remark = thiz.Remark;
            ItemID = thiz.ItemID;
        }

        public object Clone()
        {
            return new Inventory(this);
        }
    }
}