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

        public StockItem StockItem
        {
            get
            {
                return _stockItem;
            }
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
                return _stockItem.TraceItem().Name;
            }
        }

        public string ItemStandardName
        {
            get
            {
                return _stockItem.TraceItemStandard().Name;
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
                return _stockItem.TraceMeasure().Name;
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
                return _stockItem.TraceItemStandard().SalesUnitPrice;
            }
        }

        public decimal SelesPriceAmount
        {
            get
            {
                return _stockItem.TraceItemStandard().SalesUnitPrice * Balance;
            }
        }

        public decimal PurchasePrice
        {
            get
            {
                return _stockItem.TraceItemStandard().PurchaseUnitPrice;
            }
        }

        public decimal PurchasePriceAmount
        {
            get
            {
                return _stockItem.TraceItemStandard().PurchaseUnitPrice * Balance;
            }
        }

        public string Currency
        {
            get
            {
                return _stockItem.TraceCurrency().Name;
            }
        }
    }
}