using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.Inven
{
    /// <summary>
    /// DB 입고 기록 클래스
    /// </summary>
    public class InStock : IInOutStock
    {
        public string UUID { get; set; }
        public DateTime Date { get; set; }
        public string ItemStandardUUID { get; set; }
        public int Count { get; set; }
        public string EnterpriseUUID { get; set; }
        public string EmployeeUUID { get; set; }
        public string WarehouseUUID { get; set; }
        public string Remark { get; set; }
    }
}