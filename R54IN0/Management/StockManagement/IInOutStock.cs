using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.Inven
{
    public interface IInOutStock : IUUID, IItemStandardUUID
    {
        /// <summary>
        /// 기록 날짜
        /// </summary>
        DateTime Date { get; set; }
        /// <summary>
        /// 물품(규격기준) 수량
        /// </summary>
        int Count { get; set; }
        /// <summary>
        /// 입출고 거래처
        /// </summary>
        string EnterpriseUUID { get; set; }
        string EmployeeUUID { get; set; }
        string WarehouseUUID { get; set; }
        /// <summary>
        /// 비고(메모)
        /// </summary>
        string Remark { get; set; }
    }
}