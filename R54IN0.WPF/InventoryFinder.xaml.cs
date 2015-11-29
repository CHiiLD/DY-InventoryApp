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
    /// InventoryFinder.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InventoryFinder : UserControl
    {
        InventoryFinderViewModel _viewModel;

        public InventoryFinderViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        public InventoryFinder()
        {
            InitializeComponent();
            DataContext = _viewModel = InventoryFinderViewModel.CreateInventoryFinderViewModel();
            FinderTreeView.OnSelecting += _viewModel.OnSelectNodes;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewDirectoryInSelectedDirectory();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DeleteSelectedDirectories();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Refresh();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveTree();
        }
    }
}
