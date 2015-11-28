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

namespace R54IN0.WPF
{
    /// <summary>
    /// InventoryItemEditorWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InventoryItemEditorWindow : Window
    {
        public bool IsApply { get; set; }

        public InventoryItemEditorWindow(InventoryEditorViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            IsApply = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsApply = false;
            Close();
        }
    }
}