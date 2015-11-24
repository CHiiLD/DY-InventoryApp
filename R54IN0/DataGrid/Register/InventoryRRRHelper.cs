using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;
using System.Diagnostics;

namespace R54IN0.Lib
{
    /// <summary>
    /// inventory report record register/edit helper
    /// </summary>
    public class InventoryRRRHelper
    {
        private CurrentStock _stockItem;

        public CurrentStock StockItem
        {
            get
            {
                return _stockItem;
            }
        }

        /// <summary>
        /// for 새로운 기록 추가
        /// </summary>
        public InventoryRRRHelper()
        {
            _stockItem = new CurrentStock();
        }

        /// <summary>
        /// for 기존 기록 수정
        /// </summary>
        /// <param name="stockItem"></param>
        public InventoryRRRHelper(CurrentStock stockItem)
        {
            _stockItem = stockItem;
        }

        public CurrentStock Save(Specification istd, Warehouse warehouse, int itemCount, string remark)
        {
            _stockItem.SpecificationUUID = istd.UUID;
            _stockItem.WarehouseUUID = warehouse.UUID;
            _stockItem.ItemCount = itemCount;
            _stockItem.Remark = remark;
            return _stockItem.Save<CurrentStock>();
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
        List<Specification> GetItemStandards(string itemUUID)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Item item = db.LoadByKey<Item>(itemUUID);
                List<Specification> stdList = db.Table<Specification>().IndexQueryByKey("ItemUUID", item).ToList();
                return stdList;
            }
        }

        /// <summary>
        /// 단위 알기
        /// </summary>
        public string GetMeasure()
        {
            Measure measure = _stockItem.TraceMeasure();
            if (measure == null)
                return "";
            return measure.Name;
        }

        /// <summary>
        /// 창고 이름 및 수량 알기
        /// </summary>
        /// <returns>창고이름, 개수, UUID </returns>
        public Warehouse GetWarehouse()
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                return db.LoadByKey<Warehouse>(_stockItem.WarehouseUUID);
            }
        }
    }
}