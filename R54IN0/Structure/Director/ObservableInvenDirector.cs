using System;
using System.Collections.Generic;

namespace R54IN0
{
    public class ObservableInventoryDirector
    {
        private static ObservableInventoryDirector _thiz;
        private SortedDictionary<string, ObservableInventory> _dic;
        private MultiSortedDictionary<string, ObservableInventory> _productDic; //key -> 제품키

        private ObservableInventoryDirector()
        {
            _dic = new SortedDictionary<string, ObservableInventory>();
            _productDic = new MultiSortedDictionary<string, ObservableInventory>();

            InventoryFormat[] formats = null;
            using (var db = LexDb.GetDbInstance())
                formats = db.LoadAll<InventoryFormat>();

            foreach (var fmt in formats)
            {
                ObservableInventory observableInventory = new ObservableInventory(fmt);
                _dic.Add(fmt.ID, observableInventory);
                _productDic.Add(fmt.ProductID, observableInventory);
            }
        }

        public static ObservableInventoryDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new ObservableInventoryDirector();
            return _thiz;
        }

        public ObservableInventory Search(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            return _dic.ContainsKey(id) ? _dic[id] as ObservableInventory : null;
        }

        public List<ObservableInventory> SearchAsProductID(string id)
        {
            if (!_productDic.ContainsKey(id))
                return null;
            return new List<ObservableInventory>(_productDic[id]);
        }

        public List<ObservableInventory> CreateList()
        {
            return new List<ObservableInventory>(_dic.Values);
        }

        public static void Distory()
        {
            if (_thiz != null)
            {
                _thiz._dic = null;
                _thiz._productDic = null;
            }
            _thiz = null;
        }

        public void Add(ObservableInventory inventory)
        {
            if (_dic.ContainsKey(inventory.ID))
                return;
            _dic.Add(inventory.ID, inventory);
            _productDic.Add(inventory.Product.ID, inventory);
        }
    }
}