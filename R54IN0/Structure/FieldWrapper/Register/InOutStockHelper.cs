using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;

namespace R54IN0
{
    /// <summary>
    /// in out stock report record register/edit helper
    /// </summary>
    public class InOutStockHelper
    {
        InOutStock _InOutStock;

        /// <summary>
        /// for register
        /// </summary>
        public InOutStockHelper()
        {
            _InOutStock = new InOutStock();
        }

        /// <summary>
        /// for edit
        /// </summary>
        /// <param name="inOutStock"></param>
        public InOutStockHelper(InOutStock inOutStock)
        {
            _InOutStock = inOutStock;
        }

        /// <summary>
        /// 입출고재고현황기록을 데이터베이스에 저장하고 재고현황수량에서 가감하여 재고현황을 반영한다.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="spec"></param>
        /// <param name="count"></param>
        /// <param name="seller"></param>
        /// <param name="eep"></param>
        /// <param name="warehouse"></param>
        /// <param name="remark"></param>
        public InOutStock Save(StockType type, DateTime date, Specification spec, int count, Seller seller, Employee eep, Warehouse warehouse, string remark)
        {
            _InOutStock.StockType = type;
            _InOutStock.SpecificationUUID = spec.UUID;
            _InOutStock.Date = date;
            _InOutStock.ItemCount = count;
            _InOutStock.EnterpriseUUID = seller.UUID;
            _InOutStock.EmployeeUUID = eep.UUID;
            _InOutStock.WarehouseUUID = warehouse.UUID;
            _InOutStock.Remark = remark;

            IIndexQuery<Inventory, string> queryResult = null;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                queryResult = db.Table<Inventory>().IndexQueryByKey("SpecificationUUID", spec.UUID);
            }
            Inventory stockItem = queryResult.Count() == 0 ? 
                new Inventory() { SpecificationUUID = spec.UUID, WarehouseUUID = warehouse.UUID } : queryResult.ToList().First();
            switch (_InOutStock.StockType)
            {
                case StockType.IN: stockItem.ItemCount += count; break;
                case StockType.OUT: stockItem.ItemCount -= count; break;
            }
            stockItem.Save<Inventory>();
            return _InOutStock.Save<InOutStock>();
        }

        /// <summary>
        /// 단위 알기
        /// </summary>
        public string GetMeasure()
        {
            Measure measure = _InOutStock.TraceMeasure();
            if (measure == null)
                return string.Empty;
            return measure.Name;
        }

        public string GetRemark()
        {
            return _InOutStock.Remark;
        }

        public DateTime GetDate()
        {
            return _InOutStock.Date;
        }

        /// <summary>
        /// 자재 리스트 얻기
        /// </summary>
        /// <returns></returns>
        Item[] GetItems()
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                return db.LoadAll<Item>();
            }
        }

        /// <summary>
        /// 자재 규격 종류 알기
        /// </summary>
        /// <param name="itemUUID"></param>
        /// <returns></returns>
        List<Specification> GetSpecficationList(Item item)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                return db.Table<Specification>().IndexQueryByKey("ItemUUID", item).ToList();
            }
        }

        public Seller[] GetSellers()
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                return db.LoadAll<Seller>();
            }
        }

        public Employee[] GetEmployees()
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                return db.LoadAll<Employee>();
            }
        }

        public Warehouse[] GetWarehouse()
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                return db.LoadAll<Warehouse>();
            }
        }
    }
}