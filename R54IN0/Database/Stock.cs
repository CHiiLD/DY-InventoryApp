using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;

namespace DY.Inven
{
    public class Stock
    {
        public DbInstance GetDbInstance()
        {
            DbInstance db = new DbInstance("./stock_item_management.db");

            db.Map<StockItem>().Automap(i => i.ItemStandardUUID).
                WithIndex("Warehouse", i => i.Warehouse);

            db.Map<InStock>().Automap(i => i.UUID).
                WithIndex("Date", i => i.Date).
                WithIndex("ItemStandardUUID", i => i.ItemStandardUUID).
                WithIndex("Count", i => i.Count).
                WithIndex("EnterpriseUUID", i => i.EnterpriseUUID).
                WithIndex("EmployeeUUID", i => i.EmployeeUUID).
                WithIndex("WarehouseUUID", i => i.WarehouseUUID).
                WithIndex("Remark", i => i.Remark);

            db.Map<OutStock>().Automap(i => i.UUID).
                WithIndex("Date", i => i.Date).
                WithIndex("ItemStandardUUID", i => i.ItemStandardUUID).
                WithIndex("Count", i => i.Count).
                WithIndex("EnterpriseUUID", i => i.EnterpriseUUID).
                WithIndex("EmployeeUUID", i => i.EmployeeUUID).
                WithIndex("WarehouseUUID", i => i.WarehouseUUID).
                WithIndex("Remark", i => i.Remark);

            db.Initialize();

            return db;
        }
    }
}
