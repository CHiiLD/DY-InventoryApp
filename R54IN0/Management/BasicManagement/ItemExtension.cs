using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.Inven
{
    public static class ItemExtension
    {
        public static List<ItemStandard> GetItemStandardList(this Item item)
        {
            using (var db = DatabaseDirector.GetBase().GetDbInstance())
            {
                return db.Table<ItemStandard>().IndexQueryByKey("ItemUUID", item.UUID).ToList();
            }
        }
    }
}