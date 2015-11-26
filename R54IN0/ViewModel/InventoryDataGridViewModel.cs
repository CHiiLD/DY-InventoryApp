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
        public ObservableCollection<InventoryRecord> InventoryItems { get; set; }

        public InventoryDataGridViewModel()
        {
            InventoryItems = new ObservableCollection<InventoryRecord>();
        }

        public void ChangeInventoryItems(IEnumerable<InventoryRecord> items)
        {
            InventoryItems.Clear();
            foreach (var i in items)
                InventoryItems.Add(i);
        }
    }
}