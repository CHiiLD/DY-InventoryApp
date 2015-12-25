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
using System.Windows.Shapes;

namespace R54IN0.WPF
{
    public partial class InOutStockManager : UserControl
    {
        public StockType StockType
        {
            set
            {
                IOStockDataGrid.StockType = value;
                Finder.ViewModel.SelectItemsChanged += IOStockDataGrid.ViewModel.OnFinderViewSelectItemChanged;
            }
            get
            {
                return IOStockDataGrid.StockType;
            }
        }

        public InOutStockManager()
        {
            InitializeComponent();
        }
    }
}