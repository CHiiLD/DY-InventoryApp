﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    /// <summary>
    /// DB 입고 기록 클래스
    /// </summary>
    public class InOutStock : IUUID, IStock
    {
        public StockType StockType { get; set; }
        public string ItemUUID { get; set; }
        public string UUID { get; set; }
        public DateTime Date { get; set; }
        public string SpecificationUUID { get; set; }
        public int Quantity { get; set; }
        public string EnterpriseUUID { get; set; }
        public string EmployeeUUID { get; set; }
        //public string WarehouseUUID { get; set; }
        public string Remark { get; set; }
        public string InventoryUUID { get; set; }

        public InOutStock()
        {

        }

        public InOutStock(InOutStock thiz)
        {
            StockType = thiz.StockType;
            UUID = thiz.UUID;
            Date = thiz.Date;
            SpecificationUUID = thiz.SpecificationUUID;
            Quantity = thiz.Quantity;
            EnterpriseUUID = thiz.EnterpriseUUID;
            EmployeeUUID = thiz.EmployeeUUID;
            //WarehouseUUID = thiz.WarehouseUUID;
            Remark = thiz.Remark;
            ItemUUID = thiz.ItemUUID;
            InventoryUUID = thiz.InventoryUUID;
        }

        public object Clone()
        {
            return new InOutStock(this);
        }
    }
}