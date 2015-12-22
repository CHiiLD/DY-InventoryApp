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
    /// InventoryRegisterWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InventoryEditorWindow : MetroWindow
    {
        InventoryWrapperEditorViewModel _viewModel;

        public InventoryWrapperEditorViewModel Helper
        {
            get
            {
                return _viewModel;
            }
            set
            {
                _viewModel = value;
                DataContext = _viewModel;
            }
        }

        public double? Count
        {
            get
            {
                return _viewModel != null ? _viewModel.ItemCount : 0; 
            }
            set
            {
                _viewModel.ItemCount = (int)(value ?? 0);
            }
        }

        public InventoryEditorWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Update();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
