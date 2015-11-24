namespace R54IN0.Lib
{
    /// <summary>
    /// 제품의 규격
    /// </summary>
    public class Specification : IBasic
    {
        public string UUID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public decimal PurchaseUnitPrice { get; set; }
        public decimal SalesUnitPrice { get; set; }
        public string ItemUUID { get; set; }
    }
}