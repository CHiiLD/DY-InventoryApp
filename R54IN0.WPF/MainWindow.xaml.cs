﻿using System;
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
#if DEBUG
            MenuItem DebugMenu = new MenuItem() { Header = "debug" };
            MenuItem AllDbDataPurgeMenu = new MenuItem() { Header = "purge db data" };
            AllDbDataPurgeMenu.Click += AllDbDataPurgeMenu_Click;

            DebugMenu.Items.Add(AllDbDataPurgeMenu);
            Menu.Items.Add(DebugMenu);
#endif
        }

        void AllDbDataPurgeMenu_Click(object sender, RoutedEventArgs e)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                db.Purge();
            }
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

        void AccountTile_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = AccountTab;
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

        void AccountMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabCenter.SelectedItem = AccountTab;
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