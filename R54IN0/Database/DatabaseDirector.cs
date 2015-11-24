using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;

namespace R54IN0.Lib
{
    public static class DatabaseDirector
    {
        private static void RegisterVitalData(DbInstance db)
        {
            db.Save(new Currency() { UUID = "CRC000", Name = "원", IsDeleted = true });
            db.Save(new Currency() { UUID = "CRC001", Name = "미국달러", IsDeleted = true });
            db.Save(new Currency() { UUID = "CRC002", Name = "엔", IsDeleted = true });
            db.Save(new Currency() { UUID = "CRC003", Name = "인민폐", IsDeleted = true });

            db.Save(new Measure() { UUID = "MSR000", Name = "EA", IsDeleted = true });
            db.Save(new Measure() { UUID = "MSR001", Name = "PCS", IsDeleted = true });
            db.Save(new Measure() { UUID = "MSR002", Name = "SET", IsDeleted = true });

            db.Save(new Warehouse() { UUID = "WAR000", Name = "본사창고", IsDeleted = true });

            db.Save(new Employee() { UUID = "EEP000", Name = "관리자", IsDeleted = true });
            db.Save(new Seller() { UUID = "SLL000", Name = "본사", IsDeleted = true });
        }

        public static DbInstance GetDbInstance()
        {
            DbInstance db = new DbInstance("dyinven.db");

            db.Map<Currency>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsDeleted);

            db.Map<Employee>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsDeleted);

            db.Map<Item>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsDeleted).
            WithIndex("MeasureUUID", i => i.MeasureUUID).
            WithIndex("CurrencyUUID", i => i.CurrencyUUID);

            db.Map<Specification>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsDeleted).
            WithIndex("PurchaseUnitPrice", i => i.PurchaseUnitPrice).
            WithIndex("SalesUnitPrice", i => i.SalesUnitPrice).
            WithIndex("ItemUUID", i => i.ItemUUID);

            db.Map<Measure>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsDeleted);

            db.Map<Seller>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsDeleted).
            WithIndex("Delegator", i => i.Delegator).
            WithIndex("PhoneNumber", i => i.PhoneNumber).
            WithIndex("MobileNumber", i => i.MobileNumber);

            db.Map<Warehouse>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsDeleted);

            db.Map<CurrentStock>().Automap(i => i.UUID).
                WithIndex("SpecificationUUID", i => i.SpecificationUUID).
                WithIndex("WarehouseUUID", i => i.WarehouseUUID).
                WithIndex("ItemCount", i => i.ItemCount);

            db.Map<InOutStock>().Automap(i => i.UUID).
                WithIndex("StockType", i => i.StockType).
                WithIndex("Date", i => i.Date).
                WithIndex("SpecificationUUID", i => i.SpecificationUUID).
                WithIndex("ItemCount", i => i.ItemCount).
                WithIndex("EnterpriseUUID", i => i.EnterpriseUUID).
                WithIndex("EmployeeUUID", i => i.EmployeeUUID).
                WithIndex("WarehouseUUID", i => i.WarehouseUUID).
                WithIndex("Remark", i => i.Remark);

            db.Initialize();
            //RegisterVitalData(db);
            return db;
        }
    }
}
