using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;

namespace R54IN0
{
    public static class LexDb
    {
        private static CustomLexDb _customLexDb;

        public static void Distroy()
        {
            if (_customLexDb != null)
            {
                _customLexDb.RealDispose();
                _customLexDb = null;
            }
        }

        public static DbInstance GetDbInstance()
        {
            if (_customLexDb == null)
            {
#if DEBUG
                _customLexDb = new CustomLexDb("test.db");
#else
               _customLexDb = new CustomLexDb("daily inventory", "./");
#endif
                _customLexDb.InitializeIndex();
            }
            return _customLexDb;
        }
    }




    public class CustomLexDb : DbInstance, IDisposable
    {
        public CustomLexDb(string root, string path = null)
            : base(root, path)
        {

        }

        public void InitializeIndex()
        {
            CustomLexDb me = this;
            me.Map<Currency>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsDeleted", i => i.IsDeleted);
            me.Map<Employee>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsDeleted", i => i.IsDeleted);
            me.Map<Maker>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsDeleted", i => i.IsDeleted);
            me.Map<Item>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsDeleted", i => i.IsDeleted).
            WithIndex("MeasureID", i => i.MeasureID).
            WithIndex("CurrencyID", i => i.CurrencyID).
            WithIndex("MakerID", i => i.MakerID);
            me.Map<Specification>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsDeleted", i => i.IsDeleted).
            WithIndex("PurchaseUnitPrice", i => i.PurchaseUnitPrice).
            WithIndex("SalesUnitPrice", i => i.SalesUnitPrice).
            WithIndex("ItemID", i => i.ItemID).
            WithIndex("Remark", i => i.Remark);
            me.Map<Measure>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsDeleted", i => i.IsDeleted);
            me.Map<Client>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsDeleted", i => i.IsDeleted).
            WithIndex("Delegator", i => i.Delegator).
            WithIndex("PhoneNumber", i => i.PhoneNumber).
            WithIndex("MobileNumber", i => i.MobileNumber);
            me.Map<Warehouse>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsDeleted", i => i.IsDeleted);
            me.Map<Inventory>().Automap(i => i.ID).
                WithIndex("SpecificationID", i => i.SpecificationID).
                WithIndex("WarehouseID", i => i.WarehouseID).
                WithIndex("Remark", i => i.Remark).
                WithIndex("ItemID", i => i.ItemID).
                WithIndex("Quantity", i => i.Quantity);
            me.Map<InOutStock>().Automap(i => i.ID).
                WithIndex("StockType", i => i.StockType).
                WithIndex("Date", i => i.Date).
                WithIndex("SpecificationID", i => i.SpecificationID).
                WithIndex("Quantity", i => i.Quantity).
                WithIndex("EnterpriseID", i => i.EnterpriseID).
                WithIndex("EmployeeID", i => i.EmployeeID).
                //WithIndex("WarehouseID", i => i.WarehouseID).
                WithIndex("ItemID", i => i.ItemID).
                WithIndex("Remark", i => i.Remark).
                WithIndex("InventoryID", i => i.InventoryID);
            me.Map<FinderTreeNodeJsonRecord>().Automap(i => i.ID).
                WithIndex("Data", i => i.Data);

            me.Map<Project>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsDeleted", i => i.IsDeleted);
            me.Map<Product>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsDeleted", i => i.IsDeleted);
            me.Map<Customer>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsDeleted", i => i.IsDeleted);
            me.Map<Supplier>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsDeleted", i => i.IsDeleted);
            me.Map<InventoryFormat>().Automap(i => i.ID).
            WithIndex("CurrencyID", i => i.CurrencyID).
            WithIndex("MakerID", i => i.MakerID).
            WithIndex("MeasureID", i => i.MeasureID).
            WithIndex("ProductID", i => i.ProductID).
            WithIndex("Specification", i => i.Specification).
            WithIndex("Memo", i => i.Memo).
            WithIndex("Quantity", i => i.Quantity);

            me.Map<StockFormat>().Automap(i => i.ID).
            WithIndex("CustomerID", i => i.CustomerID).
            WithIndex("Date", i => i.Date).
            WithIndex("EmployeeID", i => i.EmployeeID).
            WithIndex("InventoryItemID", i => i.InventoryItemID).
            WithIndex("Memo", i => i.Memo).
            WithIndex("ProjectID", i => i.ProjectID).
            WithIndex("Quantity", i => i.Quantity).
            WithIndex("StockType", i => i.StockType).
            WithIndex("SupplierID", i => i.SupplierID).
            WithIndex("UnitPrice", i => i.UnitPrice);

            me.Initialize();
        }

        public void RealDispose()
        {
            base.Dispose();
        }

        public new void Dispose()
        {

        }
    }

}
