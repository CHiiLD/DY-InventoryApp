using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace R54IN0
{
    public class InventoryEditorViewModel : EditorViewModel<Inventory>
    {
        public InventoryEditorViewModel()
            : base()
        {

        }

        public InventoryEditorViewModel(Inventory inventory)
            : base(inventory)
        {

        }
    }
}