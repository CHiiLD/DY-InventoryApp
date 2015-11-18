using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;
using System.Diagnostics;

namespace DY.Inven
{
    public class InOutStockConditionRecord
    {
        private IInOutStock _inOutStock;

        public InOutStockConditionRecord(IInOutStock stock)
        {
            _inOutStock = stock;
        }

        public IInOutStock InOutStock
        {
            get
            {
                return _inOutStock;
            }
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
                return _inOutStock.Count;
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