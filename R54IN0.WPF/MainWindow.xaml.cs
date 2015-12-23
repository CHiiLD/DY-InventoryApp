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
using System.Collections.ObjectModel;
using MahApps.Metro.Controls;
using MahApps.Metro;

namespace R54IN0.WPF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InventoryTile_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = InventoryTab;
        }

        private void InStockTile_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = InStockTab;
        }

        private void OutStockTile_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = OutStockTab;
        }

        private void InOutStockTile_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = InOutStockTab;
        }

        private void ItemTile_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = ItemTab;
        }

        private void AccountTile_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = AccountTab;
        }

        private void ExitTile_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(110);
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(110);
        }

        private void ItemMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = ItemTab;
        }

        private void AccountMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = AccountTab;
        }

        private void MakerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = MakerTab;
        }

        private void EmployeeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = EmployeeTab;
        }

        private void WarehouseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = WarehouseTab;
        }

        private void CurrencyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = CurrencyTab;
        }

        private void MeasureMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = MeasureTab;
        }

        private void InvenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = InventoryTab;
        }

        private void InStockMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = InStockTab;
        }

        private void OutStockMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = OutStockTab;
        }

        private void InOutStockMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = InOutStockTab;
        }
    }
}