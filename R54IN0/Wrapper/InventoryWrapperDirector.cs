using System;
using System.Collections.ObjectModel;

namespace R54IN0.Test
{
    public class InventoryWrapperDirector
    {
        static InventoryWrapperDirector _thiz;
        ObservableCollection<InventoryWrapper> _items;

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
                _thiz._items = null;
                _thiz = null;
            }
        }

        public ObservableCollection<InventoryWrapper> CreateInventoryWrapperCollection()
        {
            if (_items == null)
            {
                _items = new ObservableCollection<InventoryWrapper>();
                using (var db = DatabaseDirector.GetDbInstance())
                {
                    Inventory[] invens = db.LoadAll<Inventory>();
                    foreach (var inven in invens)
                    {
                        var invenWrapper = new InventoryWrapper(inven);
                        _items.Add(invenWrapper);
                    }
                }
            }
            return new ObservableCollection<InventoryWrapper>(_items);
        }

        public void Add(InventoryWrapper item)
        {
            if (!_items.Contains(item))
            {
                item.Inven.Save<Inventory>();
                _items.Add(item);
            }
        }

        public bool Contains(InventoryWrapper item)
        {
            return _items.Contains(item);
        }

        public bool Remove(InventoryWrapper item)
        {
            item.Inven.Delete<Inventory>();
            return _items.Remove(item);
        }

        public int Count()
        {
            return _items.Count;
        }
    }
}