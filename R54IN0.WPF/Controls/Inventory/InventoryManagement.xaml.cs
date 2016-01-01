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
    /// InventoryDataGridControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InventoryManagement : UserControl
    {
        public InventoryManagement()
        {
            InitializeComponent();

            MeasureCheckBox.Click += MeasureCheckBox_Click;
            ProductCheckBox.Click += ProductCheckBox_Click;
            MakerCheckBox.Click += MakerCheckBox_Click;

            MakerCheckBox.IsChecked = true;
            ProductCheckBox.IsChecked = true;
            MeasureCheckBox.IsChecked = true;
        }

        private void MakerCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            MakerColumn.Visibility = (cb.IsChecked != true ? Visibility.Collapsed : Visibility.Visible);
        }

        private void ProductCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            ProductColumn.Visibility = (cb.IsChecked != true ? Visibility.Collapsed : Visibility.Visible);
        }

        private void MeasureCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            MeasureColumn.Visibility = (cb.IsChecked != true ? Visibility.Collapsed : Visibility.Visible);
        }
    }
}
