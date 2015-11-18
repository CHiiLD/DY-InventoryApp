using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;

namespace DY.Inven.Info
{
    public class StockConditionRecord
    {
        private StockItem _stockItem;

        public StockConditionRecord(StockItem stockItem)
        {
            _stockItem = stockItem;
        }

        public string Code
        {
            get
            {
                if (string.IsNullOrEmpty(_stockItem.ItemStandardUUID))
                    return _stockItem.ItemStandardUUID.Substring(0, 6);
                else
                    return "";
            }
        }

        public string ItemName
        {
            get
            {
                using (var db = DatabaseDirector.GetBase().GetDbInstance())
                {
                    ItemStandard istd = db.LoadByKey<ItemStandard>(_stockItem.ItemStandardUUID);
                    Item item = db.LoadByKey<Item>(istd.ItemUUID);
                    if (item == null)
                        Debug.Assert(false);
                    return item.Name;
                }
            }
        }

        public string ItemStandardName
        {
            get
            {
                using (var db = DatabaseDirector.GetBase().GetDbInstance())
                {
                    ItemStandard istd = db.LoadByKey<ItemStandard>(_stockItem.ItemStandardUUID);
                    if (istd == null)
                        Debug.Assert(false);
                    return istd.Name;
                }
            }
        }

        public int Balance
        {
            get
            {
                return _stockItem.Warehouse.Sum(x => x.Value);
            }
        }

        public string Measure
        {
            get
            {
                using (var db = DatabaseDirector.GetBase().GetDbInstance())
                {
                    ItemStandard istd = db.LoadByKey<ItemStandard>(_stockItem.ItemStandardUUID);
                    Item item = db.LoadByKey<Item>(istd.ItemUUID);
                    Measure measure = db.LoadByKey<Measure>(item.MeasureUUID);
                    if (measure == null)
                        Debug.Assert(false);
                    return measure.Name;
                }
            }
        }

        public string Warehousing
        {
            get
            {
                using (var db = DatabaseDirector.GetBase().GetDbInstance())
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var i in _stockItem.Warehouse)
                    {
                        Warehouse warehouse = db.LoadByKey<Warehouse>(i.Key);
                        if (warehouse == null)
                            Debug.Assert(false);
                        sb.Append(warehouse.Name);
                        sb.Append("(");
                        sb.Append(i.Value);
                        sb.Append("), ");
                    }
                    return sb.ToString().Substring(0, sb.Length - 2);
                }
            }
        }

        public decimal SalesPrice
        {
            get
            {
                using (var db = DatabaseDirector.GetBase().GetDbInstance())
                {
                    ItemStandard istd = db.LoadByKey<ItemStandard>(_stockItem.ItemStandardUUID);
                    if (istd == null)
                        Debug.Assert(false);
                    return istd.SalesUnitPrice;
                }
            }
        }

        public decimal SelesPriceAmount
        {
            get
            {
                using (var db = DatabaseDirector.GetBase().GetDbInstance())
                {
                    ItemStandard istd = db.LoadByKey<ItemStandard>(_stockItem.ItemStandardUUID);
                    if (istd == null)
                        Debug.Assert(false);
                    return istd.SalesUnitPrice * Balance;
                }
            }
        }

        public decimal PurchasePrice
        {
            get
            {
                using (var db = DatabaseDirector.GetBase().GetDbInstance())
                {
                    ItemStandard istd = db.LoadByKey<ItemStandard>(_stockItem.ItemStandardUUID);
                    if (istd == null)
                        Debug.Assert(false);
                    return istd.PurchaseUnitPrice;
                }
            }
        }

        public decimal PurchasePriceAmount
        {
            get
            {
                using (var db = DatabaseDirector.GetBase().GetDbInstance())
                {
                    ItemStandard istd = db.LoadByKey<ItemStandard>(_stockItem.ItemStandardUUID);
                    if (istd == null)
                        Debug.Assert(false);
                    return istd.PurchaseUnitPrice * Balance;
                }
            }
        }

        public string Currency
        {
            get
            {
                using (var db = DatabaseDirector.GetBase().GetDbInstance())
                {
                    ItemStandard istd = db.LoadByKey<ItemStandard>(_stockItem.ItemStandardUUID);
                    Item item = db.LoadByKey<Item>(istd.ItemUUID);
                    Currency currency = db.LoadByKey<Currency>(item.CurrencyUUID);
                    if (currency == null)
                        Debug.Assert(false);
                    return currency.Name;
                }
            }
        }
    }
}