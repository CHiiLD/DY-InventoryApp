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
    public partial class InOutStockDataGrid : UserControl
    {
        IOStockWrapperViewModel _viewModel;
        StockType _stockType;

        public IOStockWrapperViewModel ViewModel
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

        public StockType StockType
        {
            set
            {
                _stockType = value;
                ViewModelObserverSubject subject = ViewModelObserverSubject.GetInstance();
                _viewModel = new IOStockWrapperViewModel(_stockType, subject);
                _viewModel.NewItemAddHandler += OnNewItemAdditionHandlerCallback;
                _viewModel.SelectedItemModifyHandler += OnSelectedItemModifyHandlerCallback;
                DataContext = _viewModel;
            }
            get
            {
                return _stockType;
            }
        }

        void OnSelectedItemModifyHandlerCallback(object sender, EventArgs e)
        {
            IOStockWrapperEditorViewModel evm = new IOStockWrapperEditorViewModel(_viewModel, _viewModel.SelectedItem);
            OpenEditor(evm);
        }

        void OnNewItemAdditionHandlerCallback(object sender, EventArgs e)
        {
            IOStockWrapperEditorViewModel evm = new IOStockWrapperEditorViewModel(_viewModel);
            OpenEditor(evm);
        }

        void OpenEditor(IOStockWrapperEditorViewModel evm)
        {
            var editor = new IOStockEditorWindow();
            editor.Editor = evm;
            editor.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            editor.ShowDialog();
        }
    }
}