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
using Lex.Db;

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

        }

        private void InStockTile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OutStockTile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void InOutStockTile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ItemTile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AccountTile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExitTile_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(110);
        }
    }
}