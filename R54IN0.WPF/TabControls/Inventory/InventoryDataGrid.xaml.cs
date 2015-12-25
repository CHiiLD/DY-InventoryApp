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
    public partial class InventoryDataGrid : UserControl
    {
        InventoryWrapperViewModel _viewModel;

        public InventoryWrapperViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        public InventoryDataGrid()
        {
            InitializeComponent();
            CollectionViewModelObserverSubject subject = CollectionViewModelObserverSubject.GetInstance();
            _viewModel = new InventoryWrapperViewModel(subject);
            _viewModel.NewItemAddHandler += OnNewItemAdditionHandlerCallback;
            _viewModel.SelectedItemModifyHandler += OnSelectedItemModifyHandlerCallback;
            DataContext = _viewModel;
        }

        void OnSelectedItemModifyHandlerCallback(object sender, EventArgs e)
        {
            InventoryWrapperEditorViewModel helper = new InventoryWrapperEditorViewModel(_viewModel, _viewModel.SelectedItem);
            InventoryEditorWindow irw = new InventoryEditorWindow();
            irw.OkButton.Content = "수정";
            irw.Editor = helper;
            irw.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            irw.ShowDialog();
        }

        void OnNewItemAdditionHandlerCallback(object sender, EventArgs e)
        {
            InventoryWrapperEditorViewModel helper = new InventoryWrapperEditorViewModel(_viewModel);
            InventoryEditorWindow irw = new InventoryEditorWindow();
            irw.OkButton.Content = "추가";
            irw.Editor = helper;
            irw.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            irw.ShowDialog();
        }
    }
}