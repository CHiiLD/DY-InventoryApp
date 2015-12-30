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
        SortedDictionary<string, InventoryWrapper> _IDDic;
        SortedDictionary<string, InventoryWrapper> _specificationKeyDic;
        MultiSortedDictionary<string, InventoryWrapper> _itemKeyDic;

        InventoryWrapperDirector()
        {
            InitCollection();
        }

        void InitCollection()
        {
            _list = new List<InventoryWrapper>();
            _IDDic = new SortedDictionary<string, InventoryWrapper>();
            _specificationKeyDic = new SortedDictionary<string, InventoryWrapper>();
            _itemKeyDic = new MultiSortedDictionary<string, InventoryWrapper>();

            Inventory[] invens = null;
            using (var db = LexDb.GetDbInstance())
            {
                invens = db.LoadAll<Inventory>();
            }
#if DEBUG
            //아래 3조건은 올바른 입력 아래에서 절대 일어날 수 없는 케이스이다.
            foreach (var inven in invens)
            {
                Debug.Assert(inven.ID != null);
                Debug.Assert(inven.ItemID != null);
                Debug.Assert(inven.SpecificationID != null);
            }
#endif
            _list.AddRange(invens.Select(x => new InventoryWrapper(x)));
            foreach (var inventoryWrapper in _list)
            {
                _IDDic.Add(inventoryWrapper.ID, inventoryWrapper);
                _specificationKeyDic.Add(inventoryWrapper.Specification.ID, inventoryWrapper);
                _itemKeyDic.Add(inventoryWrapper.Item.ID, inventoryWrapper);
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
                _thiz._IDDic = null;
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
                _IDDic.Add(item.ID, item);
                _specificationKeyDic.Add(item.Specification.ID, item);
                _itemKeyDic.Add(item.Item.ID, item);
            }
        }

        public bool Contains(InventoryWrapper item)
        {
#if DEBUG
            Debug.Assert(_list.Contains(item) == _IDDic.ContainsKey(item.ID));
#endif
            return _list.Contains(item);
        }

        public bool Remove(InventoryWrapper item)
        {
            item.Product.Delete<Inventory>();
            _IDDic.Remove(item.ID);
            _specificationKeyDic.Remove(item.Specification.ID);
            _itemKeyDic.Remove(item.Item.ID, item);
            return _list.Remove(item);
        }

        public int Count()
        {
            return _list.Count;
        }

        public InventoryWrapper SearchAsSpecificationKey(string ID)
        {
            if (ID == null)
                return null;
            if (!_specificationKeyDic.ContainsKey(ID))
                return null;
            return _specificationKeyDic[ID];
        }

        public List<InventoryWrapper> SearchAsItemKey(string ID)
        {
            if (ID == null)
                return null;
            if (!_itemKeyDic.ContainsKey(ID))
                return null;
            return _itemKeyDic[ID];
        }

        public InventoryWrapper BinSearch(string ID)
        {
            if (ID == null)
                return null;

            if (!_IDDic.ContainsKey(ID))
            {
#if DEBUG
                Debug.Assert(false);
#endif
                return null;
            }
            return _IDDic[ID];
        }
    }
}