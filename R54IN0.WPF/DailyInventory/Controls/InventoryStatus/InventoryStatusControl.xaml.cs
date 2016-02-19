using System.Windows.Controls;

namespace R54IN0.WPF
{
    /// <summary>
    /// InventoryDataGridControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InventoryStatusControl : UserControl
    {
        public InventoryStatusControl()
        {
            InventoryStatusViewModel viewmodel = new InventoryStatusViewModel();
            DataContext = viewmodel;
            InitializeComponent();
        }
    }
}