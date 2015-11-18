using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;
using System.Diagnostics;

namespace DY.Inven
{
    public class StockItemRecordRegister
    {
        private StockItem _stockItem;

        public StockItem StockItem
        {
            get
            {
                return _stockItem;
            }
        }

        /// <summary>
        /// for 새로운 기록 추가
        /// </summary>
        public StockItemRecordRegister()
        {
            _stockItem = new StockItem();
        }

        /// <summary>
        /// for 기존 기록 수정
        /// </summary>
        /// <param name="stockItem"></param>
        public StockItemRecordRegister(StockItem stockItem)
        {
            _stockItem = stockItem;
        }

        public void SaveStockItem(ItemStandard iistd, List<Tuple<Warehouse, int>> warehouse)
        {
            _stockItem.ItemStandardUUID = iistd.UUID;

            if (_stockItem.Warehouse == null)
                _stockItem.Warehouse = new Dictionary<string, int>();
            _stockItem.Warehouse.Clear();

            foreach (var i in warehouse)
                _stockItem.Warehouse.Add(i.Item1.UUID, i.Item2);
            _stockItem.Save<StockItem>();
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
                Item item = db.LoadByKey<Item>(itemUUID); //db.Table<Item>().IndexQueryByKey("Name", itemName).ToList();
                List<ItemStandard> stdList = db.Table<ItemStandard>().IndexQueryByKey("ItemUUID", item).ToList();
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
        public List<Tuple<Warehouse, int>> GetWarehouse()
        {
            if (_stockItem.Warehouse == null)
                return null;
            List<Tuple<Warehouse, int>> tuples = new List<Tuple<Warehouse, int>>();
            using (var db = GetDbInstance())
            {
                foreach (var i in _stockItem.Warehouse)
                {
                    Warehouse wharhouse = db.LoadByKey<Warehouse>(i.Key);
                    if (wharhouse == null)
                        Debug.Assert(false);
                    tuples.Add(new Tuple<Warehouse, int>(wharhouse, i.Value));
                }
            }
            return tuples;
        }

      
    }
}