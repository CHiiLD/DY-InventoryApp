using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;
using System.Diagnostics;

namespace DY.Inven
{
    public class InOutStockConditionRecord
    {
        private IInOutStock _inOutStock;

        public InOutStockConditionRecord(IInOutStock stock)
        {
            stock = _inOutStock;
        }

        public DateTime Date
        {
            get
            {
                return _inOutStock.Date;
            }
        }

        public string ItemName
        {
            get
            {
                using (var db = DatabaseDirector.GetBase().GetDbInstance())
                {
                    ItemStandard istd = db.LoadByKey<ItemStandard>(_inOutStock.ItemStandardUUID);
                    Item item = db.LoadByKey<Item>(istd.ItemUUID);
                    if (item == null)
                        Debug.Assert(false);
                    return item.Name;
                }
            }
        }

        public string ItemStandardName
        {
            get
            {
                using (var db = DatabaseDirector.GetBase().GetDbInstance())
                {
                    ItemStandard istd = db.LoadByKey<ItemStandard>(_inOutStock.ItemStandardUUID);
                    if (istd == null)
                        Debug.Assert(false);
                    return istd.Name;
                }
            }
        }

        public int Count
        {
            get
            {
                return _inOutStock.Count;
            }
        }

        public string Measure
        {
            get
            {
                using (var db = DatabaseDirector.GetBase().GetDbInstance())
                {
                    ItemStandard istd = db.LoadByKey<ItemStandard>(_inOutStock.ItemStandardUUID);
                    Item item = db.LoadByKey<Item>(istd.ItemUUID);
                    Measure measure = db.LoadByKey<Measure>(item.MeasureUUID);
                    if (measure == null)
                        Debug.Assert(false);
                    return measure.Name;
                }
            }
        }

        public string Enterprise
        {
            get
            {
                using (var db = DatabaseDirector.GetBase().GetDbInstance())
                {
                    Seller seller = db.LoadByKey<Seller>(_inOutStock.EnterpriseUUID);
                    if (seller == null)
                        Debug.Assert(false);
                    return seller.Name;
                }
            }
        }

        public string Employee
        {
            get
            {
                using (var db = DatabaseDirector.GetBase().GetDbInstance())
                {
                    Employee eeployee = db.LoadByKey<Employee>(_inOutStock.EmployeeUUID);
                    if (eeployee == null)
                        Debug.Assert(false);
                    return eeployee.Name;
                }
            }
        }

        public string Warehouse
        {
            get
            {
                using (var db = DatabaseDirector.GetBase().GetDbInstance())
                {
                    Warehouse warehouse = db.LoadByKey<Warehouse>(_inOutStock.WarehouseUUID);
                    if (warehouse == null)
                        Debug.Assert(false);
                    return warehouse.Name;
                }
            }
        }

        public string Remark
        {
            get
            {
                return _inOutStock.Remark;
            }
        }
    }
}