using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace R54IN0.WPF
{
    public partial class InventoryDataGrid : UserControl
    {
        private InventoryWrapperViewModel _viewModel;

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

        private void OnSelectedItemModifyHandlerCallback(object sender, EventArgs e)
        {
            InventoryWrapperEditorViewModel helper = new InventoryWrapperEditorViewModel(_viewModel, _viewModel.SelectedItem);
            InventoryEditorWindow irw = new InventoryEditorWindow();
            irw.OkButton.Content = "수정";
            irw.Editor = helper;
            irw.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            irw.ShowDialog();
        }

        private void OnNewItemAdditionHandlerCallback(object sender, EventArgs e)
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