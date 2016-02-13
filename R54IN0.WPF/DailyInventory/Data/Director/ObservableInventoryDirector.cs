using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R54IN0
{
    internal class ObservableInventoryDirector
    {
        private IDictionary<string, ObservableInventory> _idKey;
        private SQLiteServer _db;

        internal ObservableInventoryDirector(SQLiteServer _db)
        {
            this._db = _db;
        }

        public void Load()
        {
            _idKey = new Dictionary<string, ObservableInventory>();
            var formats = _db.Select<InventoryFormat>();
            _idKey = formats.Select(x => new ObservableInventory(x)).ToDictionary(x => x.ID);
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