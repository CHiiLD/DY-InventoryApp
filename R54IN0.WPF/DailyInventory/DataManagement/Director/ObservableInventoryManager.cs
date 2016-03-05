﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    internal class ObservableInventoryManager
    {
        private IDictionary<string, ObservableInventory> _inventories;

        internal ObservableInventoryManager()
        {
        }

        public async Task InitializeAsync(IDbAction db)
        {
            _inventories = new Dictionary<string, ObservableInventory>();
            IEnumerable<InventoryFormat> formats = await db.SelectAsync<InventoryFormat>();
            _inventories = formats.Select(x => new ObservableInventory(x)).ToDictionary(x => x.ID);
        }

        public ObservableInventory Search(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            return _inventories.ContainsKey(id) ? _inventories[id] as ObservableInventory : null;
        }

        public List<ObservableInventory> SearchAsProductID(string id)
        {
            return _inventories.Values.Where(x => x.Product.ID == id).ToList();
        }

        public List<ObservableInventory> CopyObservableInventories()
        {
            return _inventories.Values.ToList();
        }

        public void Add(ObservableInventory observableInventory)
        {
            if (_inventories.ContainsKey(observableInventory.ID))
                return;
            _inventories.Add(observableInventory.ID, observableInventory);
        }

        public bool Remove(string id)
        {
            if (!_inventories.ContainsKey(id))
                return false;
            return _inventories.Remove(id);
        }
    }
}