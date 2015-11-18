using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.Inven
{
    public static class ItemStandardExtension
    {
        public static ItemStandard TraceItemStandard(this IItemStandardUUID iistd)
        {
            using (var db = DatabaseDirector.GetBase().GetDbInstance())
            {
                ItemStandard istd = db.LoadByKey<ItemStandard>(iistd.ItemStandardUUID);
                return istd;
            }
        }

        public static Item TraceItem(this IItemStandardUUID iistd)
        {
            using (var db = DatabaseDirector.GetBase().GetDbInstance())
            {
                ItemStandard istd = db.LoadByKey<ItemStandard>(iistd.ItemStandardUUID);
                Item item = db.LoadByKey<Item>(istd.ItemUUID);
                return item;
            }
        }

        public static Measure TraceMeasure(this IItemStandardUUID iistd)
        {
            using (var db = DatabaseDirector.GetBase().GetDbInstance())
            {
                ItemStandard istd = db.LoadByKey<ItemStandard>(iistd.ItemStandardUUID);
                Item item = db.LoadByKey<Item>(istd.ItemUUID);
                Measure measure = db.LoadByKey<Measure>(item.MeasureUUID);
                return measure;
            }
        }

        public static Currency TraceCurrency(this IItemStandardUUID iistd)
        {
            using (var db = DatabaseDirector.GetBase().GetDbInstance())
            {
                ItemStandard istd = db.LoadByKey<ItemStandard>(iistd.ItemStandardUUID);
                Item item = db.LoadByKey<Item>(istd.ItemUUID);
                Currency currency = db.LoadByKey<Currency>(item.CurrencyUUID);
                return currency;
            }
        }
    }
}
