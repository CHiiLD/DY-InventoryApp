using System.Windows.Controls;

namespace R54IN0.WPF
{
    /// <summary>
    /// InoutStockDataGrid.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class IOStockStatusControl : UserControl
    {
        public IOStockStatusControl()
        {
            IOStockStatusViewModel viewmodel = new IOStockStatusViewModel();
            DataContext = viewmodel;
            InitializeComponent();
        }
    }
}