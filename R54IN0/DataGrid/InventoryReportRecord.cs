using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;

namespace R54IN0.Lib
{
    /// <summary>
    /// 재고 현황
    /// </summary>
    public class InventoryReportRecord
    {
        private CurrentStock _stockItem;

        public InventoryReportRecord(CurrentStock stockItem)
        {
            _stockItem = stockItem;
        }

        public string Code
        {
            get
            {
                return _stockItem.SpecificationUUID.Substring(0, 6).ToUpper();
            }
        }

        public string Name
        {
            get
            {
                return _stockItem.TraceItem().Name;
            }
        }

        public string Spec
        {
            get
            {
                return _stockItem.TraceItemStandard().Name;
            }
        }

        public int Count
        {
            get
            {
                return _stockItem.ItemCount;
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
                return _stockItem.TraceWarehouse().Name;
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
                return _stockItem.TraceItemStandard().SalesUnitPrice * Count;
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
                return _stockItem.TraceItemStandard().PurchaseUnitPrice * Count;
            }
        }

        public string Currency
        {
            get
            {
                return _stockItem.TraceCurrency().Name;
            }
        }

        public string Remark
        {
            get
            {
                return _stockItem.Remark;
            }
        }
    }
}