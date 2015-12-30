using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class StockFormat : IID
    {
        public string ID { get; set; }

        /// <summary>
        /// 입출고 종류
        /// </summary>
        public virtual StockType StockType { get; set; }

        /// <summary>
        /// 기록된 날짜
        /// </summary>
        public virtual DateTime Date { get; set; }

        /// <summary>
        /// 제품의 개별적 입고가, 출고가
        /// </summary>
        public virtual decimal UnitPrice { get; set; }

        /// <summary>
        /// 입고 또는 출고 수량
        /// </summary>
        public virtual int Quantity { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public virtual string Memo { get; set; }

        /// <summary>
        /// 재고 범위식별자
        /// </summary>
        public string InventoryItemID { get; set; }

        /// <summary>
        /// 출고처 범위식별자
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 입고처 범위식별자
        /// </summary>
        public string SupplierID { get; set; }

        /// <summary>
        /// 데이터 등록자 범위식별자
        /// </summary>
        public string EmployeeID { get; set; }

        /// <summary>
        /// 프로젝트 범위 식별자
        /// </summary>
        public string ProjectID { get; set; }
    }
}