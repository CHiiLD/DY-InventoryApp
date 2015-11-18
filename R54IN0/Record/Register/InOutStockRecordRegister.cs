using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;

namespace DY.Inven
{
    public class InOutStockRecordRegister
    {
        IInOutStock _InOutStock;

        IInOutStock InOutStock
        {
            get
            {
                return _InOutStock;
            }
        }

        public InOutStockRecordRegister(bool isInStock)
        {
            _InOutStock = isInStock ? (IInOutStock)new InStock() : new OutStock();
        }

        public InOutStockRecordRegister(IInOutStock inOutStock)
        {
            _InOutStock = inOutStock;
        }

        /// <summary>
        /// 입출고재고현황기록을 데이터베이스에 저장하고 재고현황수량에서 가감하여 재고현황을 반영한다.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="istd"></param>
        /// <param name="count"></param>
        /// <param name="seller"></param>
        /// <param name="eep"></param>
        /// <param name="warehouse"></param>
        /// <param name="remark"></param>
        public void SaveInOutStock(DateTime date, ItemStandard istd, int count, Seller seller, Employee eep, Warehouse warehouse, string remark)
        {
            _InOutStock.ItemStandardUUID = istd.UUID;
            _InOutStock.Date = date;
            _InOutStock.Count = count;
            _InOutStock.EnterpriseUUID = seller.UUID;
            _InOutStock.EmployeeUUID = eep.UUID;
            _InOutStock.WarehouseUUID = warehouse.UUID;
            _InOutStock.Remark = remark;

            var queryResult = DatabaseDirector.GetStock().GetDbInstance().Table<StockItem>().IndexQueryByKey("ItemStandardUUID", istd.UUID);
            StockItem stockItem = queryResult == null ? new StockItem() { ItemStandardUUID = istd.UUID, Warehouse = new Dictionary<string, int>() } : queryResult.ToList().First();
            if (_InOutStock is InStock)
            {
                stockItem.Warehouse[_InOutStock.WarehouseUUID] += count;
                _InOutStock.Save<InStock>();
            }
            else
            {
                stockItem.Warehouse[_InOutStock.WarehouseUUID] -= count;
                _InOutStock.Save<OutStock>();
            }
            stockItem.Save<StockItem>();
        }

        private DbInstance GetDbInstance()
        {
            return DatabaseDirector.GetStock().GetDbInstance();
        }

        /// <summary>
        /// 자재 리스트 얻기
        /// </summary>
        /// <returns></returns>
        Item[] GetItems()
        {
            using (var db = GetDbInstance())
            {
                return db.LoadAll<Item>();
            }
        }

        /// <summary>
        /// 자재 규격 종류 알기
        /// </summary>
        /// <param name="itemUUID"></param>
        /// <returns></returns>
        List<ItemStandard> GetItemStandards(string itemUUID)
        {
            using (var db = GetDbInstance())
            {
                Item item = db.LoadByKey<Item>(itemUUID);
                List<ItemStandard> stdList = db.Table<ItemStandard>().IndexQueryByKey("ItemUUID", item).ToList();
                return stdList;
            }
        }

        /// <summary>
        /// 단위 알기
        /// </summary>
        public string GetMeasure()
        {
            Measure measure = _InOutStock.TraceMeasure();
            if (measure == null)
                return "";
            return measure.Name;
        }

        public Seller[] GetSellers()
        {
            using (var db = GetDbInstance())
            {
                return db.LoadAll<Seller>();
            }
        }

        public Employee[] GetEmployees()
        {
            using (var db = GetDbInstance())
            {
                return db.LoadAll<Employee>();
            }
        }

        public Warehouse[] GetWarehouseList()
        {
            using (var db = GetDbInstance())
            {
                return db.LoadAll<Warehouse>();
            }
        }

        public string GetRemark()
        {
            return _InOutStock.Remark;
        }

        public DateTime GetDate()
        {
            return _InOutStock.Date;
        }
    }
}