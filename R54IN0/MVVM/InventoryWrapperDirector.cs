using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class InventoryWrapperDirector
    {
        static InventoryWrapperDirector _thiz;
        List<InventoryWrapper> _list;

        InventoryWrapperDirector()
        {

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
                _thiz = null;
            }
        }

        public ObservableCollection<InventoryWrapper> CreateCollection()
        {
            if (_list == null)
            {
                _list = new List<InventoryWrapper>();
                using (var db = DatabaseDirector.GetDbInstance())
                {
                    Inventory[] invens = db.LoadAll<Inventory>();
                    foreach (var inven in invens)
                    {
                        var invenWrapper = new InventoryWrapper(inven);
                        _list.Add(invenWrapper);
                    }
                }
            }
            return new ObservableCollection<InventoryWrapper>(_list);
        }

        public void Add(InventoryWrapper item)
        {
            if (!_list.Contains(item))
            {
                item.Record.Save<Inventory>();
                _list.Add(item);
            }
        }

        public bool Contains(InventoryWrapper item)
        {
            return _list.Contains(item);
        }

        public bool Remove(InventoryWrapper item)
        {
            item.Record.Delete<Inventory>();
            return _list.Remove(item);
        }

        public int Count()
        {
            return _list.Count;
        }

        public int IndexOf(InventoryWrapper item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, InventoryWrapper item)
        {
            _list.Insert(index, item);
        }
    }
}