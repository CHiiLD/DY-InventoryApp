using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;
using System.Diagnostics;

namespace R54IN0.Lib
{
    public class InOutStockReportRecord
    {
        private InOutStock _inOutStock;

        public InOutStockReportRecord(InOutStock stock)
        {
            _inOutStock = stock;
        }

        public DateTime Date
        {
            get
            {
                return _inOutStock.Date;
            }
        }

        public string ItemName
        {
            get
            {
                return _inOutStock.TraceItem().Name;
            }
        }

        public string ItemStandardName
        {
            get
            {
                return _inOutStock.TraceItemStandard().Name;
            }
        }

        public int Count
        {
            get
            {
                return _inOutStock.ItemCount;
            }
        }

        public string Measure
        {
            get
            {
                return _inOutStock.TraceMeasure().Name;
            }
        }

        public string Enterprise
        {
            get
            {
                return _inOutStock.TraceSeller().Name;
            }
        }

        public string Employee
        {
            get
            {
                return _inOutStock.TraceEmployee().Name;
            }
        }

        public string Warehouse
        {
            get
            {
                return _inOutStock.TraceWarehouse().Name;
            }
        }

        public string Remark
        {
            get
            {
                return _inOutStock.Remark;
            }
        }
    }
}