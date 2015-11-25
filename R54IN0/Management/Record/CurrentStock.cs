using System.Collections.Generic;

namespace R54IN0
{
    /// <summary>
    /// DB재고품목 기록 클래스
    /// </summary>
    public class CurrentStock : IUUID, IStock
    {
        public string UUID { get; set; }
        public string SpecificationUUID { get; set; }
        public string WarehouseUUID { get; set; }
        public int ItemCount { get; set; }
        public string Remark { get; set; }
    }
}