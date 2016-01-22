using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R54IN0
{
    public class ObservableInventoryDirector : IObservableInventoryDirector
    {
        private static ObservableInventoryDirector _thiz;
        private IDictionary<string, ObservableInventory> _idKey;

        private ObservableInventoryDirector()
        {
        }

        internal async Task LoadDataFromServerAsync()
        {
            _idKey = new Dictionary<string, ObservableInventory>();
            var formats = await DbAdapter.GetInstance().SelectAllAsync<InventoryFormat>();
            _idKey = formats.Select(x => new ObservableInventory(x)).ToDictionary(x => x.ID);
        }

        public static ObservableInventoryDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new ObservableInventoryDirector();
            return _thiz;
        }

        public ObservableInventory SearchObservableInventory(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            return _idKey.ContainsKey(id) ? _idKey[id] as ObservableInventory : null;
        }

        public IEnumerable<ObservableInventory> SearchObservableInventoryAsProductID(string id)
        {
            return _idKey.Values.Where(x => x.Product.ID == id).ToList();
        }

        public List<ObservableInventory> CopyObservableInventories()
        {
            return _idKey.Values.ToList();
        }

        public static void Destory()
        {
            if (_thiz != null)
                _thiz._idKey = null;
            _thiz = null;
        }

        public void AddObservableInventory(ObservableInventory observableInventory)
        {
            if (_idKey.ContainsKey(observableInventory.ID))
                return;
            _idKey.Add(observableInventory.ID, observableInventory);
        }

        public bool RemoveObservableInventory(ObservableInventory observableInventory)
        {
            if (!_idKey.ContainsKey(observableInventory.ID))
                return false;
            return _idKey.Remove(observableInventory.ID);
        }
    }
}