using System.Linq;
using System.Windows.Controls;

namespace R54IN0.WPF
{
    /// <summary>
    /// InoutStockDataGrid.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class IOStockStatus : UserControl
    {
        public IOStockStatus()
        {
            InitializeComponent();

            IOStockStatusViewModel viewmodel = new IOStockStatusViewModel();
            DataContext = viewmodel;
            DataGrid.DataContext = viewmodel.DataGridViewModel;
            DataGrid.DataGridControl.PreviewTextInput += viewmodel.DataGridViewModel.OnPreviewTextInputted;

            DatePicker.DataContext = viewmodel.DatePickerViewModel;
            ProjectListBox.DataContext = viewmodel.ProjectListBoxViewModel;
            MSTreeView.DataContext = viewmodel.TreeViewViewModel;
            MSTreeView.TreeView.OnSelecting += viewmodel.TreeViewViewModel.OnNodeSelected;

            viewmodel.SelectedGroupItem = viewmodel.GroupItems.First();
        }
    }
}