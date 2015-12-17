using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
#if false
    public static class IInventoryExtension
    {
        public static Specification TraceSpecification(this IInventory iiven)
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
                Item item = db.LoadByKey<Item>(iiven.ItemUUID);
                return item;
            }
        }

        public static Measure TraceMeasure(this IInventory iiven)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Item item = db.LoadByKey<Item>(iiven.ItemUUID);
                Measure measure = db.LoadByKey<Measure>(item.MeasureUUID);
                return measure;
            }
        }

        public static Currency TraceCurrency(this IInventory iiven)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Item item = db.LoadByKey<Item>(iiven.ItemUUID);
                Currency currency = db.LoadByKey<Currency>(item.CurrencyUUID);
                return currency;
            }
        }

        public static Maker TraceMaker(this IInventory iiven)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Item item = db.LoadByKey<Item>(iiven.ItemUUID);
                Maker maker = db.LoadByKey<Maker>(item.MakerUUID);
                return maker;
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
#endif
}
