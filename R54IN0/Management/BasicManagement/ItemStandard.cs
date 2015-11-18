namespace DY.Inven
{
    /// <summary>
    /// 규격
    /// </summary>
    public class ItemStandard : IBasic
    {
        public string Name { get; set; }
        public string UUID { get; set; }
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 입고(구입) 단가
        /// </summary>
        public decimal PurchaseUnitPrice { get; set; }

        /// <summary>
        /// 출고(판매) 단가
        /// </summary>
        public decimal SalesUnitPrice { get; set; }

        /// <summary>
        /// 품목 UUID
        /// </summary>
        public string ItemUUID { get; set; }
    }
}