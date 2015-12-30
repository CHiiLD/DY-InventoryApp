using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    /// <summary>
    /// DB 입고 기록 클래스
    /// </summary>
    public class InOutStock : IID, IStock
    {
        public StockType StockType { get; set; }
        public string ItemID { get; set; }
        public string ID { get; set; }
        public DateTime Date { get; set; }
        public string SpecificationID { get; set; }
        public int Quantity { get; set; }
        public string EnterpriseID { get; set; }
        public string EmployeeID { get; set; }
        //public string WarehouseID { get; set; }
        public string Remark { get; set; }
        public string InventoryID { get; set; }

        public InOutStock()
        {

        }

        public InOutStock(InOutStock thiz)
        {
            StockType = thiz.StockType;
            ID = thiz.ID;
            Date = thiz.Date;
            SpecificationID = thiz.SpecificationID;
            Quantity = thiz.Quantity;
            EnterpriseID = thiz.EnterpriseID;
            EmployeeID = thiz.EmployeeID;
            //WarehouseID = thiz.WarehouseID;
            Remark = thiz.Remark;
            ItemID = thiz.ItemID;
            InventoryID = thiz.InventoryID;
        }

        public object Clone()
        {
            return new InOutStock(this);
        }
    }
}