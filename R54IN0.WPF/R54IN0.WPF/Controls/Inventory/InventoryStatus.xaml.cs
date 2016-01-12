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
            DataGridL.DataGridControl.PreviewTextInput += viewmodel.DataGridViewModel1.OnPreviewTextInputted;
            DataGridR.DataGridControl.PreviewTextInput += viewmodel.DataGridViewModel2.OnPreviewTextInputted;

            ProductSelector.DataContext = viewmodel.TreeViewViewModel;
            ProductSelector.MultiSelectTreeView.TreeView.OnSelecting += viewmodel.TreeViewViewModel.OnNodeSelected;
        }
    }
}