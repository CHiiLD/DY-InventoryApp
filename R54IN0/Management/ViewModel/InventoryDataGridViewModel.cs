using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class InventoryDataGridViewModel
    {
        public ObservableCollection<InventoryPipe> Items { get; set; }
        public InventoryPipe SelectedItem { get; set; }

        public InventoryDataGridViewModel()
        {
            IEnumerable<Inventory> items = null;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                items = db.LoadAll<Inventory>();
            }
            Items = new ObservableCollection<InventoryPipe>(items.Select(x => new InventoryPipe(x)));
            SelectedItem = Items.FirstOrDefault();
        }

        public void ChangeInventoryItems(IEnumerable<InventoryPipe> items)
        {
            Items.Clear();
            foreach (var i in items)
                Items.Add(i);
            SelectedItem = Items.FirstOrDefault();
        }

        public void RemoveSelectedItem()
        {
            if (SelectedItem != null)
            {
                SelectedItem.Inven.Delete<Inventory>();
                Items.Remove(SelectedItem);
                SelectedItem = Items.FirstOrDefault();
            }
        }

        public void Add(InventoryPipe inventoryPipe)
        {
            inventoryPipe.Inven.Save<Inventory>();
            Items.Add(inventoryPipe);
            SelectedItem = inventoryPipe;
        }
    }
}