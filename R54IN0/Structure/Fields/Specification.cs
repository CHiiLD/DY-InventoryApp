namespace R54IN0
{
    /// <summary>
    /// 제품의 규격
    /// </summary>
    public class Specification : IField
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public decimal PurchaseUnitPrice { get; set; }
        public decimal SalesUnitPrice { get; set; }
        public string ItemID { get; set; }
        public string Remark { get; set; }

        public Specification()
        {

        }

        public Specification(Specification thiz)
        {
            ID = thiz.ID;
            Name = thiz.Name;
            IsDeleted = thiz.IsDeleted;
            PurchaseUnitPrice = thiz.PurchaseUnitPrice;
            SalesUnitPrice = thiz.SalesUnitPrice;
            ItemID = thiz.ItemID;
            Remark = thiz.Remark;
        }
    }
}