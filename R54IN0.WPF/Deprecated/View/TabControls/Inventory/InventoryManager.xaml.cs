using System.Windows.Controls;

namespace R54IN0.WPF
{
    public partial class InventoryManager : UserControl
    {
        public InventoryManager()
        {
            InitializeComponent();
            Finder.ViewModel = InvenDataGrid.ViewModel.CreateFinderViewModel(Finder.FinderTreeView) as ItemFinderViewModel;
        }
    }
}