using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace R54IN0.WPF
{
    public partial class InOutStockDataGrid : UserControl
    {
        private SearchStockWrapperViewModel _viewModel;
        private IOStockType _stockType;

        public SearchStockWrapperViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        public InOutStockDataGrid()
        {
            InitializeComponent();
        }

        public IOStockType StockType
        {
            set
            {
                _stockType = value;
                CollectionViewModelObserverSubject subject = CollectionViewModelObserverSubject.GetInstance();
                _viewModel = new SearchStockWrapperViewModel(_stockType, subject);
                _viewModel.NewItemAddHandler += OnNewItemAdditionHandlerCallback;
                _viewModel.SelectedItemModifyHandler += OnSelectedItemModifyHandlerCallback;
                DataContext = _viewModel;
            }
            get
            {
                return _stockType;
            }
        }

        private void OnSelectedItemModifyHandlerCallback(object sender, EventArgs e)
        {
            StockWrapperEditorViewModel evm = new StockWrapperEditorViewModel(_viewModel, _viewModel.SelectedItem);
            var editor = new IOStockEditorWindow();
            editor.ItemAddButton.Content = "수정";
            editor.Editor = evm;
            editor.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            editor.ShowDialog();
        }

        private void OnNewItemAdditionHandlerCallback(object sender, EventArgs e)
        {
            StockWrapperEditorViewModel evm = new StockWrapperEditorViewModel(_viewModel);
            var editor = new IOStockEditorWindow();
            editor.ItemAddButton.Content = "추가";
            editor.Editor = evm;
            editor.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            editor.ShowDialog();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}