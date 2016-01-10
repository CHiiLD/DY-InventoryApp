using System.Windows.Controls;

namespace R54IN0.WPF
{
    public partial class InOutStockManager : UserControl
    {
        public IOStockType StockType
        {
            set
            {
                IOStockDataGrid.StockType = value;
                Finder.ViewModel = IOStockDataGrid.ViewModel.CreateFinderViewModel(Finder.FinderTreeView) as ItemFinderViewModel;
            }
            get
            {
                return IOStockDataGrid.StockType;
            }
        }

        public InOutStockManager()
        {
            InitializeComponent();
        }
    }
}