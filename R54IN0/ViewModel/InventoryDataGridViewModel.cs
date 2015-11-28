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
        public ObservableCollection<InventoryPipe> InventoryItems { get; set; }

        public InventoryDataGridViewModel()
        {
            InventoryItems = new ObservableCollection<InventoryPipe>();
        }

        public void ChangeInventoryItems(IEnumerable<InventoryPipe> items)
        {
            InventoryItems.Clear();
            foreach (var i in items)
                InventoryItems.Add(i);
        }
    }
}