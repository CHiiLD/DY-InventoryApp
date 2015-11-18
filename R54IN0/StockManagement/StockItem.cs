using System.Collections.Generic;

namespace DY.Inven
{
    /// <summary>
    /// DB재고품목 기록 클래스
    /// </summary>
    public class StockItem
    {
        public string ItemStandardUUID { get; set; }
        /// <summary>
        /// 자재보관창고와 보관수량
        /// WarehouseUUID : Count 매칭
        /// </summary>
        public Dictionary<string, int> Warehouse { get; set; }
    }
}