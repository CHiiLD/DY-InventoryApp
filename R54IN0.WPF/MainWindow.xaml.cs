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

using Lex.Db;

namespace R54IN0.WPF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if false
            var eep = new Employee() { Name = "천두관" }.Save<Employee>();
            var seller = new Seller() { Name = "(주) 비즈메디코리아", Delegator = "김성남", MobileNumber = "", PhoneNumber = "" }.Save<Seller>();
            var meas = new Measure() { Name = "개" }.Save<Measure>();
            var ware = new Warehouse() { Name = "엠플래닝" }.Save<Warehouse>();
            var curr = new Currency() { Name = "홍콩달러" }.Save<Currency>();
            var item = new Item() { Name = "냅킨", CurrencyUUID = curr.UUID, MeasureUUID = meas.UUID }.Save<Item>();
            var spec = new Specification() { Name = "독일 직수입", ItemUUID = item.UUID, PurchaseUnitPrice = 2212, SalesUnitPrice = 3212 }.Save<Specification>();

            //재고관리헬퍼로 재고관리 생성
            var cur_stock = new InventoryRRRHelper().Save(spec, ware, 10, "remark");

            var viewModel = InvenDataGrid.DataContext as InventoryDataGridViewModel;
            viewModel.ChangeInventoryItems(new InventoryRecord[] { new InventoryRecord(cur_stock) });
#endif
            Test.DummyDbData dummy = new Test.DummyDbData();
            dummy.Create();

            var items = DatabaseDirector.GetDbInstance().LoadAll<Item>();

            DirectoryNode root = new DirectoryNode("ROOT");
            DirectoryNode node1 = new DirectoryNode("ROOT_NODE1");
            DirectoryNode node2 = new DirectoryNode("ROOT_NODE2");
            ItemNode itemNode0 = new ItemNode(items[0].UUID);
            ItemNode itemNode1 = new ItemNode(items[1].UUID);
            node1.Nodes.Add(itemNode0);
            node1.Nodes.Add(itemNode1);
            root.Nodes.Add(node1);
            root.Nodes.Add(node2);

            var viewModel = Finder.DataContext as InventoryFinderViewModel;
            viewModel.Nodes.Add(root);
        }
    }
}
