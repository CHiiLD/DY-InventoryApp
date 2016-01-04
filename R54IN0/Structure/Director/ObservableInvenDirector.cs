using System;
using System.Collections.Generic;

namespace R54IN0
{
    public class ObservableInvenDirector
    {
        private static ObservableInvenDirector _thiz;
        private SortedDictionary<string, ObservableInventory> _dic;

        private ObservableInvenDirector()
        {
            _dic = new SortedDictionary<string, ObservableInventory>();
            InventoryFormat[] formats = null;
            using (var db = LexDb.GetDbInstance())
                formats = db.LoadAll<InventoryFormat>();
            foreach (var t in formats)
                _dic.Add(t.ID, new ObservableInventory(t));
        }

        public static ObservableInvenDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new ObservableInvenDirector();
            return _thiz;
        }

        public ObservableInventory Search(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            return _dic.ContainsKey(id) ? _dic[id] as ObservableInventory : null;
        }

        public List<ObservableInventory> CreateList()
        {
            return new List<ObservableInventory>(_dic.Values);
        }

        public static void Distory()
        {
            if (_thiz != null)
                _thiz._dic = null;
            _thiz = null;
        }
    }
}