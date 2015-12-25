using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace R54IN0
{
    public class InventoryWrapperDirector
    {
        static InventoryWrapperDirector _thiz;
        List<InventoryWrapper> _list;
        SortedDictionary<string, InventoryWrapper> _sortDic;

        InventoryWrapperDirector()
        {
            InitCollection();
        }

        void InitCollection()
        {
            _list = new List<InventoryWrapper>();
            _sortDic = new SortedDictionary<string, InventoryWrapper>();

            Inventory[] invens = null;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                invens = db.LoadAll<Inventory>();
            }
#if DEBUG
            foreach (var inven in invens)
            {
                Debug.Assert(inven.UUID != null);
                Debug.Assert(inven.ItemUUID != null);
                Debug.Assert(inven.SpecificationUUID != null);
            }
#endif
            _list.AddRange(invens.Select(x => new InventoryWrapper(x)));
            foreach (var item in _list)
                _sortDic.Add(item.UUID, item);
        }

        public static InventoryWrapperDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new InventoryWrapperDirector();
            return _thiz;
        }

        public static void Distory()
        {
            if (_thiz != null)
            {
                _thiz._list = null;
                _thiz._sortDic = null;
                _thiz = null;
            }
        }

        public ObservableCollection<InventoryWrapper> CreateCollection()
        {
            return new ObservableCollection<InventoryWrapper>(_list);
        }

        public void Add(InventoryWrapper item)
        {
            if (!_list.Contains(item))
            {
                item.Record.Save<Inventory>();
                _sortDic.Add(item.UUID, item);
                _list.Add(item);
            }
        }

        public bool Contains(InventoryWrapper item)
        {
#if DEBUG
            Debug.Assert(_list.Contains(item) == _sortDic.ContainsKey(item.UUID));
#endif
            return _list.Contains(item);
        }

        public bool Remove(InventoryWrapper item)
        {
            item.Record.Delete<Inventory>();
            _sortDic.Remove(item.UUID);
            return _list.Remove(item);
        }

        public int Count()
        {
            return _list.Count;
        }

        public InventoryWrapper BinSearch(string uuid)
        {
            if (uuid == null)
                return null;

            if (!_sortDic.ContainsKey(uuid))
            {
#if DEBUG
                Debug.Assert(false);
                return null;
#endif
            }
            return _sortDic[uuid];
        }
    }
}