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
        //InventoryEditorViewModel _editorViewModel;

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
            ViewModelObserverSubject subject = ViewModelObserverSubject.GetInstance();
            _viewModel = new InventoryWrapperViewModel(subject);
            DataContext = _viewModel;
        }

        void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
           // _viewModel.RemoveSelectedItem();
        }

        void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //_editorViewModel = new InventoryEditorViewModel();
            //InventoryItemEditorWindow inventoryItemEditorWindow = new InventoryItemEditorWindow(_editorViewModel);
            //inventoryItemEditorWindow.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            //inventoryItemEditorWindow.Closed += OnEditorWindowClosed;
            //inventoryItemEditorWindow.ShowDialog();
        }

        void EditButton_Click(object sender, RoutedEventArgs e)
        {
            //if (_viewModel.SelectedItem != null)
            //{
            //    _editorViewModel = new InventoryEditorViewModel(_viewModel.SelectedItem.Inven);
            //    InventoryItemEditorWindow inventoryItemEditorWindow = new InventoryItemEditorWindow(_editorViewModel);
            //    inventoryItemEditorWindow.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            //    inventoryItemEditorWindow.Closed += OnEditorWindowClosed;
            //    inventoryItemEditorWindow.ShowDialog();
            //}
        }

        public void OnEditorWindowClosed(object sender, EventArgs e)
        {
            //InventoryItemEditorWindow editWin = sender as InventoryItemEditorWindow;
            //if (editWin.IsApply)
            //{
            //    if (_editorViewModel.Action == EditorModelViewAction.ADD)
            //        _viewModel.AddNewItem(_editorViewModel.Inventory);
            //    else if (_editorViewModel.Action == EditorModelViewAction.EDIT)
            //        _viewModel.ReplaceItem(_editorViewModel.Inventory);
            //}
        }
    }
}