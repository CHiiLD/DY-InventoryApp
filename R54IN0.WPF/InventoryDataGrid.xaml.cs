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
    public partial class InventoryDataGrid : UserControl
    {
        InventoryDataGridViewModel _viewModel;
        InventoryEditorViewModel _editorViewModel;

        public InventoryDataGrid()
        {
            InitializeComponent();
            DataContext = _viewModel = new InventoryDataGridViewModel();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RemoveSelectedItem();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _editorViewModel = new InventoryEditorViewModel();
            InventoryItemEditorWindow editWindow = new InventoryItemEditorWindow(_editorViewModel);
            editWindow.Closed += OnEditorWindowClosed;
            editWindow.Show();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedItem != null)
            {
                _editorViewModel = new InventoryEditorViewModel(_viewModel.SelectedItem.Inven);
                InventoryItemEditorWindow editWindow = new InventoryItemEditorWindow(_editorViewModel);
                editWindow.Closed += OnEditorWindowClosed;
                editWindow.Show();
            }
        }

        public void OnEditorWindowClosed(object sender, EventArgs e)
        {
            InventoryItemEditorWindow editWin = sender as InventoryItemEditorWindow;
            if (editWin.IsApply)
            {
                if (!_editorViewModel.IsEditMode)
                    _viewModel.Add(_editorViewModel.Inventory);
                else
                    _viewModel.Replace(_editorViewModel.Inventory);
            }
        }
    }
}