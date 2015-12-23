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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro;

namespace R54IN0.WPF
{
    /// <summary>
    /// IOStockRegisterWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class IOStockEditorWindow : MetroWindow
    {
        IOStockWrapperEditorViewModel _viewModel;

        public FinderViewModel Finder
        {
            get; set;
        }

        public IOStockWrapperEditorViewModel Editor
        {
            get
            {
                return _viewModel;
            }
            set
            {
                _viewModel = value;
                Finder = _viewModel.CreateFinderViewModel(FinderTreeView);
                FinderTreeView.GetBindingExpression(TreeViewEx.ItemsSourceProperty).UpdateTarget();
                FinderTreeView.GetBindingExpression(TreeViewEx.SelectedItemsProperty).UpdateTarget();
                DataContext = _viewModel;
            }
        }

        public IOStockEditorWindow()
        {
            InitializeComponent();
        }

        void ItemAddButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Update();
            Close();
        }

        void ItemRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
