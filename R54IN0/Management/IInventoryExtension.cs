using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public static class IInventoryExtension
    {
        public static Specification TraceItemStandard(this IInventory iiven)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                return db.LoadByKey<Specification>(iiven.SpecificationUUID);
            }
        }

        public static Item TraceItem(this IInventory iiven)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Specification istd = db.LoadByKey<Specification>(iiven.SpecificationUUID);
                Item item = db.LoadByKey<Item>(istd.ItemUUID);
                return item;
            }
        }

        public static Measure TraceMeasure(this IInventory iiven)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Specification istd = db.LoadByKey<Specification>(iiven.SpecificationUUID);
                Item item = db.LoadByKey<Item>(istd.ItemUUID);
                Measure measure = db.LoadByKey<Measure>(item.MeasureUUID);
                return measure;
            }
        }

        public static Currency TraceCurrency(this IInventory iiven)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Specification istd = db.LoadByKey<Specification>(iiven.SpecificationUUID);
                Item item = db.LoadByKey<Item>(istd.ItemUUID);
                Currency currency = db.LoadByKey<Currency>(item.CurrencyUUID);
                return currency;
            }
        }

        public static Warehouse TraceWarehouse(this IInventory iiven)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Warehouse warehouse = db.LoadByKey<Warehouse>(iiven.WarehouseUUID);
                return warehouse;
            }
        }
    }
}
