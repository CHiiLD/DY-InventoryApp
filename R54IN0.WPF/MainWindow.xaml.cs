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
using R54IN0.Test;

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
#if DEBUG
            Menu.Items.Add(new Separator() { Width = 5} );

            MenuItem debugMenu = new MenuItem() { Header = "DEBUG" };
            MenuItem allDbDataPurgeMenu = new MenuItem() { Header = "PURGE DB DATA" };
            allDbDataPurgeMenu.Click += AllDbDataPurgeMenu_Click;

            debugMenu.Items.Add(allDbDataPurgeMenu);
            Menu.Items.Add(debugMenu);

            MenuItem createSampleMenu = new MenuItem() { Header = "CREATE DY SAMPLE" };
            createSampleMenu.Click += CreateSampleMenu_Click;
            debugMenu.Items.Add(createSampleMenu);

            MenuItem createStressSampleMenu = new MenuItem() { Header = "CREATE STRESS SAMPLE" };
            debugMenu.Items.Add(createStressSampleMenu);
#endif
        }

        private void CreateSampleMenu_Click(object sender, RoutedEventArgs e)
        {
            new DYDummyDbData().Create();
        }

        void AllDbDataPurgeMenu_Click(object sender, RoutedEventArgs e)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                db.Purge();
            }
            FinderDirector.GetInstance().Collection.Clear();
        }

        void InventoryTile_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = InventoryTab;
        }

        void InStockTile_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = InStockTab;
        }

        void OutStockTile_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = OutStockTab;
        }

        void InOutStockTile_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = InOutStockTab;
        }

        void ItemTile_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = ItemTab;
        }

        void ClientTile_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = ClientTab;
        }

        void ExitTile_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(110);
        }

        void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(110);
        }

        void ItemMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = ItemTab;
        }

        void ClientMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = ClientTab;
        }

        void MakerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = MakerTab;
        }

        void EmployeeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = EmployeeTab;
        }

        void WarehouseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = WarehouseTab;
        }

        void CurrencyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = CurrencyTab;
        }

        void MeasureMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = MeasureTab;
        }

        void InvenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = InventoryTab;
        }

        void InStockMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = InStockTab;
        }

        void OutStockMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = OutStockTab;
        }

        void InOutStockMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = InOutStockTab;
        }
    }
}