using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;

namespace R54IN0
{
    public static class DatabaseDirector
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
                _customLexDb = new CustomLexDb("test.db", "./");
#else
               _customLexDb = new CustomLexDb("daily inventory", "./");
#endif
                _customLexDb.LoadData();
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

        public void LoadData()
        {
            CustomLexDb me = this;
            me.Map<Currency>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsDeleted);
            me.Map<Employee>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsDeleted);
            me.Map<Maker>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsDeleted);
            me.Map<Item>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsDeleted).
            WithIndex("MeasureUUID", i => i.MeasureUUID).
            WithIndex("CurrencyUUID", i => i.CurrencyUUID).
            WithIndex("MakerUUID", i => i.MakerUUID);
            me.Map<Specification>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsDeleted).
            WithIndex("PurchaseUnitPrice", i => i.PurchaseUnitPrice).
            WithIndex("SalesUnitPrice", i => i.SalesUnitPrice).
            WithIndex("ItemUUID", i => i.ItemUUID).
            WithIndex("Remark", i => i.Remark);
            me.Map<Measure>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsDeleted);
            me.Map<Client>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsDeleted).
            WithIndex("Delegator", i => i.Delegator).
            WithIndex("PhoneNumber", i => i.PhoneNumber).
            WithIndex("MobileNumber", i => i.MobileNumber);
            me.Map<Warehouse>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsDeleted);
            me.Map<Inventory>().Automap(i => i.UUID).
                WithIndex("SpecificationUUID", i => i.SpecificationUUID).
                WithIndex("WarehouseUUID", i => i.WarehouseUUID).
                WithIndex("Remark", i => i.Remark).
                WithIndex("ItemUUID", i => i.ItemUUID).
                WithIndex("Quantity", i => i.Quantity);
            me.Map<InOutStock>().Automap(i => i.UUID).
                WithIndex("StockType", i => i.StockType).
                WithIndex("Date", i => i.Date).
                WithIndex("SpecificationUUID", i => i.SpecificationUUID).
                WithIndex("Quantity", i => i.Quantity).
                WithIndex("EnterpriseUUID", i => i.EnterpriseUUID).
                WithIndex("EmployeeUUID", i => i.EmployeeUUID).
                WithIndex("WarehouseUUID", i => i.WarehouseUUID).
                WithIndex("ItemUUID", i => i.ItemUUID).
                WithIndex("Remark", i => i.Remark).
                WithIndex("InventoryUUID", i => i.InventoryUUID);
            me.Map<FinderTreeNodeJsonRecord>().Automap(i => i.UUID).
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
