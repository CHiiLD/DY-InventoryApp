using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Linq;

namespace R54IN0.WPF
{
    /// <summary>
    /// InoutStockDataGrid.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InoutStockStatus : UserControl
    {
        public InoutStockStatus()
        {
            InitializeComponent();

            InoutStockStatusViewModel viewmodel = new InoutStockStatusViewModel();
            DataContext = viewmodel;
            this.DataGrid.DataContext = viewmodel.DataGridViewModel;

            DatePicker.DataContext = viewmodel.DatePickerViewModel;
            ProjectListBox.DataContext = viewmodel.ProjectListBoxViewModel;
            ProductManager.DataContext = viewmodel.TreeViewViewModel;

            viewmodel.SelectedGroupItem = viewmodel.GroupItems.First();
        }
    }
}