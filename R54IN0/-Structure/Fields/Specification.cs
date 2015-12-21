namespace R54IN0
{
    /// <summary>
    /// 제품의 규격
    /// </summary>
    public class Specification : IField
    {
        public string UUID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public decimal PurchaseUnitPrice { get; set; }
        public decimal SalesUnitPrice { get; set; }
        public string ItemUUID { get; set; }
        public string Remark { get; set; }

        public Specification()
        {

        }

        public Specification(Specification thiz)
        {
            UUID = thiz.UUID;
            Name = thiz.Name;
            IsDeleted = thiz.IsDeleted;
            PurchaseUnitPrice = thiz.PurchaseUnitPrice;
            SalesUnitPrice = thiz.SalesUnitPrice;
            ItemUUID = thiz.ItemUUID;
            Remark = thiz.Remark;
        }
    }
}