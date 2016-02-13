using Lex.Db;
using System;

namespace R54IN0
{
    internal static class LexDb
    {
        private static CustomLexDb _customLexDb;

        public static void Destroy()
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
                _customLexDb = new CustomLexDb("TesT.Db");
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

#if false
            me.Map<Employee>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name);
            me.Map<Maker>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name);
            me.Map<Measure>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name);
            me.Map<Warehouse>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name);
            me.Map<Project>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name);
            me.Map<Product>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name);
            me.Map<Customer>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name);
            me.Map<Supplier>().Automap(i => i.ID).
            WithIndex("Name", i => i.Name);

            me.Map<InventoryFormat>().Automap(i => i.ID).
            WithIndex("MeasureID", i => i.MeasureID).
            WithIndex("ProductID", i => i.ProductID).
            WithIndex("Specification", i => i.Specification).
            WithIndex("Memo", i => i.Memo).
            WithIndex("MakerID", i => i.MakerID).
            WithIndex("Quantity", i => i.Quantity);

            me.Map<IOStockFormat>().Automap(i => i.ID).
            WithIndex("CustomerID", i => i.CustomerID).
            WithIndex("SupplierID", i => i.SupplierID).
            WithIndex("Date", i => i.Date).
            WithIndex("InventoryID", i => i.InventoryID).
            WithIndex("Memo", i => i.Memo).
            WithIndex("WarehouseID", i => i.WarehouseID).
            WithIndex("EmployeeID", i => i.EmployeeID).
            WithIndex("ProjectID", i => i.ProjectID).
            WithIndex("Quantity", i => i.Quantity).
            WithIndex("StockType", i => i.StockType).
            WithIndex("RemainingQuantity", i => i.RemainingQuantity).
            WithIndex("UnitPrice", i => i.UnitPrice);
#endif

            me.Map<TreeViewNodeJsonFormat>().Automap(i => i.ID).
            WithIndex("Data", i => i.Data);

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