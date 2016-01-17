using System.ComponentModel;

namespace R54IN0.WPF
{
    public partial class IOStockDataAmenderViewModel
    {
        private enum Mode
        {
            ADD,
            MODIFY
        }

        public class NonSaveObservableInventory : ObservableInventory
        {
            public NonSaveObservableInventory() : base()
            {
            }

            public NonSaveObservableInventory(InventoryFormat inventory) : base(inventory)
            {
            }

            public override void NotifyPropertyChanged(string propertyName)
            {
                if (propertyChanged != null)
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}