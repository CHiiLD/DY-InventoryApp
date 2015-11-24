using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.Inven
{
    //public static class ItemExtension
    //{
    //    public static List<Specification> GetItemStandardList(this Item item)
    //    {
    //        using (var db = DatabaseDirector.GetDbInstance())
    //        {
    //            return db.Table<Specification>().IndexQueryByKey("ItemUUID", item.UUID).ToList();
    //        }
    //    }

    //    public static Measure GetMeasure(this Item item)
    //    {
    //        using (var db = DatabaseDirector.GetDbInstance())
    //        {
    //            return db.LoadByKey<Measure>(item.MeasureUUID);
    //        }
    //    }

    //    public static Currency GetCurrency(this Item item)
    //    {
    //        using (var db = DatabaseDirector.GetDbInstance())
    //        {
    //            return db.LoadByKey<Currency>(item.CurrencyUUID);
    //        }
    //    }
    //}
}