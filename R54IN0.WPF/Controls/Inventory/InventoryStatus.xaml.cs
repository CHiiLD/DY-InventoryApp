using System.Windows.Controls;

namespace R54IN0.WPF
{
    /// <summary>
    /// InventoryDataGridControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InventoryStatus : UserControl
    {
        public InventoryStatus()
        {
            InitializeComponent();

            InventoryStatusViewModel viewmodel = new InventoryStatusViewModel();
            DataContext = viewmodel;

            //datagrid binding
            DataGridL.DataContext = viewmodel.DataGridViewModel1;
            DataGridR.DataContext = viewmodel.DataGridViewModel2;
        }
    }
}