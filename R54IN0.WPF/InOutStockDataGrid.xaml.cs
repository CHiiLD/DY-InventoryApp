using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace R54IN0.WPF
{
    /// <summary>
    /// InventoryDataGrid.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InOutStockDataGrid : UserControl
    {
        InOutStockDataGridViewModel _viewModel;
        InOutStockEditorViewModel _editorViewModel;
        StockType _stockType;

        public InOutStockDataGrid()
        {
            InitializeComponent();
            //_stockType = StockType.ALL;
            //DataContext = _viewModel = new InOutStockDataGridViewModel(_stockType);
        }

        public StockType StockType
        {
            set
            {
                _stockType = value;
                DataContext = _viewModel = new InOutStockDataGridViewModel(_stockType);
            }
            get
            {
                return _stockType;
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RemoveSelectedItem();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _editorViewModel = new InOutStockEditorViewModel();
            InOutStockItemEditorWindow editWindow = new InOutStockItemEditorWindow(_editorViewModel);
            editWindow.Closed += OnEditorWindowClosed;
            editWindow.Show();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedItem != null)
            {
                _editorViewModel = new InOutStockEditorViewModel(_viewModel.SelectedItem.Inven);
                InOutStockItemEditorWindow editWindow = new InOutStockItemEditorWindow(_editorViewModel);
                editWindow.Closed += OnEditorWindowClosed;
                editWindow.Show();
            }
        }

        public void OnEditorWindowClosed(object sender, EventArgs e)
        {
            InOutStockItemEditorWindow editWin = sender as InOutStockItemEditorWindow;
            if (editWin.IsApply)
            {
                if (!_editorViewModel.IsEdit)
                    _viewModel.Add(_editorViewModel.Inventory);
                else
                    _viewModel.Replace(_editorViewModel.Inventory);
            }
        }
    }
}