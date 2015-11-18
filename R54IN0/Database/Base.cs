using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;

namespace DY.Inven
{
    public class Base
    {
        private void RegisterVitalData(DbInstance db)
        {
            db.Save(new Currency() { UUID = "CRC000", Name = "Won", IsEnable = true });
            db.Save(new Currency() { UUID = "CRC001", Name = "US dollar", IsEnable = true });
            db.Save(new Currency() { UUID = "CRC002", Name = "Yen", IsEnable = true });

            db.Save(new Measure() { UUID = "MSR000", Name = "EA", IsEnable = true });
            db.Save(new Measure() { UUID = "MSR001", Name = "PCS", IsEnable = true });
            db.Save(new Measure() { UUID = "MSR002", Name = "SET", IsEnable = true });

            db.Save(new Employee() { UUID = "EEP000", Name = "Empty", IsEnable = true });
            db.Save(new Seller() { UUID = "SLL000", Name = "Empty", IsEnable = true });
            db.Save(new Warehouse() { UUID = "WAR000", Name = "Empty", IsEnable = true });
        }

        public DbInstance GetDbInstance()
        {
            DbInstance db = new DbInstance("./base_registration_information.db");

            db.Map<Currency>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsEnable);

            db.Map<Employee>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsEnable);

            db.Map<Item>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsEnable).
            WithIndex("MeasureUUID", i => i.MeasureUUID).
            WithIndex("CurrencyUUID", i => i.CurrencyUUID);

            db.Map<ItemStandard>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsEnable).
            WithIndex("PurchaseUnitPrice", i => i.PurchaseUnitPrice).
            WithIndex("SalesUnitPrice", i => i.SalesUnitPrice).
            WithIndex("ItemUUID", i => i.ItemUUID);

            db.Map<Measure>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsEnable);

            db.Map<Seller>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsEnable).
            WithIndex("Delegator", i => i.Delegator).
            WithIndex("PhoneNumber", i => i.PhoneNumber).
            WithIndex("MobileNumber", i => i.MobileNumber);

            db.Map<Warehouse>().Automap(i => i.UUID).
            WithIndex("Name", i => i.Name).
            WithIndex("IsEnable", i => i.IsEnable);

            db.Initialize();
            RegisterVitalData(db);
            return db;
        }
    }
}