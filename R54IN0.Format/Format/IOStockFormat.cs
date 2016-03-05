using System;

namespace R54IN0
{
    public class IOStockFormat : IID, IIOStockFormat
    {
        private const int MEMO_LENGTH_LIMIT = 256;

        private string _memo;

        public string ID { get; set; }
        /// <summary>
        /// 입출고 종류
        /// </summary>
        public virtual IOStockType StockType { get; set; }

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
        public virtual string Memo
        {
            get
            {
                return _memo;
            }
            set
            {
                _memo = this.CutString(value, MEMO_LENGTH_LIMIT);
            }
        }

        /// <summary>
        /// 재고 범위식별자
        /// </summary>
        public string InventoryID { get; set; }

        /// <summary>
        /// 출고처 범위식별자
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 입고처 범위식별자
        /// </summary>
        public string SupplierID { get; set; }

        /// <summary>
        /// 프로젝트 범위 식별자
        /// </summary>
        public string ProjectID { get; set; }

        /// <summary>
        /// 보관장소 범위 식별자
        /// </summary>
        public string WarehouseID { get; set; }

        /// <summary>
        /// 직원 범위 식별자
        /// </summary>
        public string EmployeeID { get; set; }

        public IOStockFormat()
        {
        }

        public IOStockFormat(IOStockFormat thiz)
        {
            ID = thiz.ID;
            StockType = thiz.StockType;
            Date = thiz.Date;
            UnitPrice = thiz.UnitPrice;
            Quantity = thiz.Quantity;
            Memo = thiz.Memo;
            InventoryID = thiz.InventoryID;
            CustomerID = thiz.CustomerID;
            SupplierID = thiz.SupplierID;
            ProjectID = thiz.ProjectID;
            WarehouseID = thiz.WarehouseID;
            EmployeeID = thiz.EmployeeID;
        }
    }
}