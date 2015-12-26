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
        SortedDictionary<string, InventoryWrapper> _uuidDic;
        SortedDictionary<string, InventoryWrapper> _specificationKeyDic;
        MultiSortedDictionary<string, InventoryWrapper> _itemKeyDic;

        InventoryWrapperDirector()
        {
            InitCollection();
        }

        void InitCollection()
        {
            _list = new List<InventoryWrapper>();
            _uuidDic = new SortedDictionary<string, InventoryWrapper>();
            _specificationKeyDic = new SortedDictionary<string, InventoryWrapper>();
            _itemKeyDic = new MultiSortedDictionary<string, InventoryWrapper>();

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
            foreach (var inventoryWrapper in _list)
            {
                _uuidDic.Add(inventoryWrapper.UUID, inventoryWrapper);
                _specificationKeyDic.Add(inventoryWrapper.Specification.UUID, inventoryWrapper);
                _itemKeyDic.Add(inventoryWrapper.Item.UUID, inventoryWrapper);
            }
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
                _thiz._uuidDic = null;
                _thiz._itemKeyDic = null;
                _thiz._specificationKeyDic = null;
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
                item.Product.Save<Inventory>();
                _list.Add(item);
                _uuidDic.Add(item.UUID, item);
                _specificationKeyDic.Add(item.Specification.UUID, item);
                _itemKeyDic.Add(item.Item.UUID, item);
            }
        }

        public bool Contains(InventoryWrapper item)
        {
#if DEBUG
            Debug.Assert(_list.Contains(item) == _uuidDic.ContainsKey(item.UUID));
#endif
            return _list.Contains(item);
        }

        public bool Remove(InventoryWrapper item)
        {
            item.Product.Delete<Inventory>();
            _uuidDic.Remove(item.UUID);
            _specificationKeyDic.Remove(item.Specification.UUID);
            _itemKeyDic.Remove(item.Item.UUID, item);
            return _list.Remove(item);
        }

        public int Count()
        {
            return _list.Count;
        }

        public InventoryWrapper SearchAsSpecificationKey(string uuid)
        {
            if (uuid == null)
                return null;
            if (!_specificationKeyDic.ContainsKey(uuid))
                return null;
            return _specificationKeyDic[uuid];
        }

        public List<InventoryWrapper> SearchAsItemKey(string uuid)
        {
            if (uuid == null)
                return null;
            if (!_itemKeyDic.ContainsKey(uuid))
                return null;
            return _itemKeyDic[uuid];
        }

        public InventoryWrapper BinSearch(string uuid)
        {
            if (uuid == null)
                return null;

            if (!_uuidDic.ContainsKey(uuid))
            {
#if DEBUG
                Debug.Assert(false);
                return null;
#endif
            }
            return _uuidDic[uuid];
        }
    }
}