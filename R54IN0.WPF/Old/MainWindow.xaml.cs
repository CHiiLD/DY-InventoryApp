using MahApps.Metro.Controls;
using System.Windows;

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
#if false
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
            createStressSampleMenu.Click += CreateStressSampleMenu_Click;
            debugMenu.Items.Add(createStressSampleMenu);
#endif
        }

        private void CreateStressSampleMenu_Click(object sender, RoutedEventArgs e)
        {
            //new DYDummyDbData().Create(50, 60);
        }

        private void CreateSampleMenu_Click(object sender, RoutedEventArgs e)
        {
            //new DYDummyDbData().Create();
        }

        private void AllDbDataPurgeMenu_Click(object sender, RoutedEventArgs e)
        {
            using (var db = LexDb.GetDbInstance())
            {
                db.Purge();
            }
            TreeViewNodeDirector.GetInstance().Collection.Clear();
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

        private void ClientTile_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = ClientTab;
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

        private void ClientMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = ClientTab;
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