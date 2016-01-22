using System.Collections.Generic;

namespace R54IN0
{
    public interface IObservableInventoryDirector
    {
        void AddObservableInventory(ObservableInventory observableInventory);
        List<ObservableInventory> CopyObservableInventories();
        bool RemoveObservableInventory(ObservableInventory observableInventory);
        ObservableInventory SearchObservableInventory(string id);
        IEnumerable<ObservableInventory> SearchObservableInventoryAsProductID(string id);
    }
}