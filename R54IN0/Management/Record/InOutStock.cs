using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Lib
{
    public enum StockType
    {
        NONE,
        IN,
        OUT
    }

    /// <summary>
    /// DB 입고 기록 클래스
    /// </summary>
    public class InOutStock : IUUID, IStock
    {
        public StockType StockType { get; set; }
        public string UUID { get; set; }
        public DateTime Date { get; set; }
        public string SpecificationUUID { get; set; }
        public int ItemCount { get; set; }
        public string EnterpriseUUID { get; set; }
        public string EmployeeUUID { get; set; }
        public string WarehouseUUID { get; set; }
        public string Remark { get; set; }
    }
}