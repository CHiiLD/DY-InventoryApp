﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using MySQL.Test;
using R54IN0.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace R54IN0.Test
{
    [TestClass]
    public class IOStockStatusViewModelUnitTest
    {
        private static MySqlConnection _conn;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Console.WriteLine(nameof(ClassInitialize));
            Console.WriteLine(context.TestName);

            _conn = new MySqlConnection(ConnectingString.KEY);
            _conn.Open();

            Dummy dummy = new Dummy(_conn);
            dummy.Create();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Console.WriteLine(nameof(ClassCleanup));
            _conn.Close();
            _conn = null;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            MySqlConnection conn = DataDirector.GetInstance().DB.Connection;
            using (MySqlCommand cmd = new MySqlCommand("begin work;", conn))
                cmd.ExecuteNonQuery();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            MySqlConnection conn = DataDirector.GetInstance().DB.Connection;
            using (MySqlCommand cmd = new MySqlCommand("rollback;", conn))
                cmd.ExecuteNonQuery();

            CollectionViewModelObserverSubject.Destory();
            TreeViewNodeDirector.Destroy(true);
            DataDirector.Destroy();
        }

        [TestMethod]
        public void CanCreate()
        {
            new IOStockStatusViewModel();
        }

        /// <summary>
        /// 뷰에서 날짜별로 그룹화를 선택해서 오늘날짜를 클릭한다.
        /// </summary>
        [TestMethod]
        public void SelectDatePicker()
        {
            var viewmodel = new IOStockStatusViewModel();
            Assert.AreEqual(0, viewmodel.DataGridViewModel.Items.Count);
            //날짜를 선택
            viewmodel.SelectedDataGridGroupOption = IOStockStatusViewModel.DATAGRID_OPTION_DATE;
            viewmodel.DatePickerViewModel.LastYearCommand.Execute(null); //올해 버튼을 클릭

            Assert.AreNotEqual(0, viewmodel.DataGridViewModel.Items.Count); //올해에 입력된 입출고 데이터를 데이터그리드에 추가
            foreach (var item in viewmodel.DataGridViewModel.Items)
            {
                Assert.IsTrue(new DateTime(DateTime.Now.Year - 1, 1, 1).Ticks < item.Date.Ticks);
            }
        }

        /// <summary>
        /// 뷰에서 프로젝트별 그룹화를 선택한 후 프로젝트를 클릭한다.
        /// </summary>
        [TestMethod]
        public void SelectProject()
        {
            var viewmodel = new IOStockStatusViewModel();
            Assert.AreEqual(0, viewmodel.DataGridViewModel.Items.Count);
            //프로젝트 선택
            viewmodel.SelectedDataGridGroupOption = IOStockStatusViewModel.DATAGRID_OPTION_PROJECT;
            var project = viewmodel.ProjectListBoxViewModel.SelectedItem = viewmodel.ProjectListBoxViewModel.Items.Random();

            Assert.AreNotEqual(0, viewmodel.DataGridViewModel.Items.Count);
            foreach (var item in viewmodel.DataGridViewModel.Items)
            {
                Assert.IsTrue(item.Project == project);
            }
        }

        /// <summary>
        /// 뷰에서 제품별 그룹화를 선택한 후 탐색기에서 제품셀을 선택
        /// </summary>
        [TestMethod]
        public void SelectProduct()
        {
            var viewmodel = new IOStockStatusViewModel();
            Assert.AreEqual(0, viewmodel.DataGridViewModel.Items.Count);
            viewmodel.SelectedDataGridGroupOption = IOStockStatusViewModel.DATAGRID_OPTION_PRODUCT;
            var node = viewmodel.TreeViewViewModel.SearchNodeInRoot(NodeType.PRODUCT).Random();
            viewmodel.TreeViewViewModel.ExecuteNodesSelectedEventCommand(
                new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node }, new List<TreeViewNode>()));
            viewmodel.OnTreeViewNodesSelected(viewmodel.TreeViewViewModel, new PropertyChangedEventArgs("SelectedNodes"));
            Assert.AreNotEqual(0, viewmodel.DataGridViewModel.Items.Count);
            foreach (var item in viewmodel.DataGridViewModel.Items)
            {
                Assert.IsTrue(item.Inventory.Product.ID == node.ObservableObjectID);
            }
        }

        /// <summary>
        /// 입고와 출고 체크박스를 선택하였을 때 그에 맞는 데이터가 데이터 그리드에 업데이트 되어야 한다.
        /// </summary>
        [TestMethod]
        public void ControlInoutStockCheckBox()
        {
            var viewmodel = new IOStockStatusViewModel();
            viewmodel.IsCheckedInComing = true;
            viewmodel.IsCheckedOutGoing = true;
            //제품 하나 선택
            viewmodel.SelectedDataGridGroupOption = IOStockStatusViewModel.DATAGRID_OPTION_PRODUCT;
            var node = viewmodel.TreeViewViewModel.SearchNodeInRoot(NodeType.PRODUCT).Random();
            viewmodel.TreeViewViewModel.ExecuteNodesSelectedEventCommand(
                new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node }, new List<TreeViewNode>()));
            Assert.IsTrue(viewmodel.DataGridViewModel.Items.All(i => IOStockType.ALL.HasFlag(i.StockType)));

            viewmodel.IsCheckedInComing = false;
            Assert.IsTrue(viewmodel.DataGridViewModel.Items.All(i => i.StockType == IOStockType.OUTGOING));

            viewmodel.IsCheckedOutGoing = false;
            Assert.IsTrue(viewmodel.DataGridViewModel.Items.Count == 0);

            viewmodel.IsCheckedInComing = true;
            Assert.IsTrue(viewmodel.DataGridViewModel.Items.All(i => i.StockType == IOStockType.INCOMING));

            viewmodel.IsCheckedOutGoing = true;
            Assert.IsTrue(viewmodel.DataGridViewModel.Items.All(i => IOStockType.ALL.HasFlag(i.StockType)));
        }

        /// <summary>
        /// IOStockDataGridItem의 IsChecked체크박스 활성화
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [Ignore]
        public void CheckDataGridRowCell()
        {
            var viewmodel = SelectSomeTreeViewNode();
            var item = viewmodel.DataGridViewModel.Items.Random();
            item.IsChecked = true;
        }

        /// <summary>
        /// 하나의 데이터그리드 아이템을 체크
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [Ignore]
        public void CopyCheckedDataGridRowCell()
        {
            var viewmodel = SelectSomeTreeViewNode();
            int itemsCount = viewmodel.DataGridViewModel.Items.Count();
            var item = viewmodel.DataGridViewModel.Items.Random();
            item.IsChecked = true;

            Console.WriteLine("입출고 데이터를 복사하기 전 데이터그리드 아이템소스 개수: " + itemsCount);
            viewmodel.DataGridViewModel.CheckedIOStockFormatsCopyCommand.Execute(null);

            int itemsCount2 = viewmodel.DataGridViewModel.Items.Count();
            Console.WriteLine("입출고 데이터를 복사한 후 데이터그리드 아이템소스 개수: " + itemsCount2);
            Assert.AreEqual(itemsCount + 1, itemsCount2);
        }

        /// <summary>
        /// 다수의 데이터그리드 아이템을 체크
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [Ignore]
        public void CopyCheckedDataGridRowCells()
        {
            var viewmodel = SelectSomeTreeViewNode();
            int itemsCount = viewmodel.DataGridViewModel.Items.Count();
            for (int i = 0; i < 10; i++)
            {
                int idx = new Random().Next(0, itemsCount);
                var item = viewmodel.DataGridViewModel.Items.ElementAt(idx);
                item.IsChecked = !item.IsChecked;
            }
            int checkedItemsCount = viewmodel.DataGridViewModel.Items.Where(x => x.IsChecked == true).Count();

            Console.WriteLine("입출고 데이터를 복사하기 전 데이터그리드 아이템소스 개수: " + itemsCount);
            viewmodel.DataGridViewModel.CheckedIOStockFormatsCopyCommand.Execute(null);

            int itemsCount2 = viewmodel.DataGridViewModel.Items.Count();
            Console.WriteLine("입출고 데이터를 복사한 후 데이터그리드 아이템소스 개수: " + itemsCount2);
            Assert.AreEqual(itemsCount + checkedItemsCount, itemsCount2);
        }

        /// <summary>
        /// 데이터그리드를 복사하여 추가한 뒤 Quantity가 제대로 계산되었는지 확인한다.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [Ignore]
        public void CopyRowCellThenCheckThatQuantityHaveToCalc()
        {
            var viewmodel = SelectSomeTreeViewNode();
            int itemsCount = viewmodel.DataGridViewModel.Items.Count();
            var item = viewmodel.DataGridViewModel.Items.Random();
            item.IsChecked = true;

            Console.WriteLine("선택된 아이템의 IOSQty: {0}, RemainQty: {1}, InvQty: {2}", item.Quantity, item.RemainingQuantity, item.Inventory.Quantity);
            viewmodel.DataGridViewModel.CheckedIOStockFormatsCopyCommand.Execute(null);

            AssertQuantityChecking(viewmodel.DataGridViewModel.Items);
        }

        /// <summary>
        /// 데이터그리드를 복사하여 여러 데이터를 추가한 뒤 Quantity가 제대로 계산되었는지 확인한다.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [Ignore]
        public void CopyRowCellThenCheckThatQuantityHaveToCalc2()
        {
            var viewmodel = SelectSomeTreeViewNode();
            int itemsCount = viewmodel.DataGridViewModel.Items.Count();
            for (int i = 0; i < 10; i++)
            {
                int idx = new Random().Next(0, itemsCount);
                var item = viewmodel.DataGridViewModel.Items.ElementAt(idx);
                if (item.IsChecked != true)
                    item.IsChecked = true;
            }

            viewmodel.DataGridViewModel.CheckedIOStockFormatsCopyCommand.Execute(null);

            AssertQuantityChecking(viewmodel.DataGridViewModel.Items);
        }

        /// <summary>
        /// 데이터그리드의 아이템을 하나 삭제하고 데이터그리드를 최신화 시킨다.
        /// </summary>
        [TestMethod]
        public void DeleteItemThenSyncDataGridItems()
        {
            var viewmodel = SelectSomeTreeViewNode();
            var item = viewmodel.DataGridViewModel.SelectedItem = viewmodel.DataGridViewModel.Items.Random();

            viewmodel.DataGridViewModel.IOStockFormatDeletionCommand.Execute(null);

            Assert.IsTrue(!viewmodel.DataGridViewModel.Items.Contains(item));
        }

        /// <summary>
        /// 삭제 후 데이터베이스에서 관련 자료가 모두 삭제되어야 한다.
        /// </summary>
        [TestMethod]
        public void DeleteItemThenSyncDb()
        {
            var viewmodel = SelectSomeTreeViewNode();
            var item = viewmodel.DataGridViewModel.SelectedItem = viewmodel.DataGridViewModel.Items.Random();
            string iosID = item.ID;

            viewmodel.DataGridViewModel.IOStockFormatDeletionCommand.Execute(null);

            var iofmts = DataDirector.GetInstance().DB.Select<IOStockFormat>(iosID);
            Assert.IsNull(iofmts);
        }

        /// <summary>
        /// 삭제 후 잔여수량과 재고수량을 업데이트 한다.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void DeleteItemThenSyncQty()
        {
            var viewmodel = SelectSomeTreeViewNode();
            var item = viewmodel.DataGridViewModel.SelectedItem = viewmodel.DataGridViewModel.Items.Random();
            string iosID = item.ID;

            viewmodel.DataGridViewModel.IOStockFormatDeletionCommand.Execute(null);

            AssertQuantityChecking(viewmodel.DataGridViewModel.Items);
        }

        /// <summary>
        /// 삭제 후 잔여수량과 재고수량을 업데이트 한다.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void DeleteItemThenSyncInventoryQty()
        {
            IOStockStatusViewModel viewmodel = SelectSomeTreeViewNode();
            IOStockDataGridItem item = viewmodel.DataGridViewModel.SelectedItem = viewmodel.DataGridViewModel.Items.Random();
            int invQty = item.Inventory.Quantity;

            viewmodel.DataGridViewModel.IOStockFormatDeletionCommand.Execute(null);
            int inQty2 = DataDirector.GetInstance().SearchInventory(item.Inventory.ID).Quantity;

            Assert.AreNotEqual(invQty, inQty2);
        }

        [TestMethod]
        public void DeleteCheckedItemThenSyncDataGridItems()
        {
            var viewmodel = SelectSomeTreeViewNode();
            var item = viewmodel.DataGridViewModel.SelectedItem = viewmodel.DataGridViewModel.Items.Random();
            item.IsChecked = true;

            viewmodel.DataGridViewModel.ChekcedIOStockFormatsDeletionCommand.Execute(null);

            Assert.IsTrue(!viewmodel.DataGridViewModel.Items.Contains(item));
        }

        /// <summary>
        /// 삭제 후 데이터베이스에서 관련 자료가 모두 삭제되어야 한다.
        /// </summary>
        [TestMethod]
        public void DeleteCheckedItemThenSyncDb()
        {
            var viewmodel = SelectSomeTreeViewNode();
            var item = viewmodel.DataGridViewModel.SelectedItem = viewmodel.DataGridViewModel.Items.Random();
            item.IsChecked = true;
            string iosID = item.ID;

            viewmodel.DataGridViewModel.ChekcedIOStockFormatsDeletionCommand.Execute(null);

            var iofmts = DataDirector.GetInstance().DB.Select<IOStockFormat>(iosID);
            Assert.IsNull(iofmts);
        }

        /// <summary>
        /// 삭제 후 잔여수량과 재고수량을 업데이트 한다.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [Ignore]
        public void DeleteCheckedItemThenSyncQty()
        {
            var viewmodel = SelectSomeTreeViewNode();
            var item = viewmodel.DataGridViewModel.SelectedItem = viewmodel.DataGridViewModel.Items.Random();
            item.IsChecked = true;
            string iosID = item.ID;

            viewmodel.DataGridViewModel.ChekcedIOStockFormatsDeletionCommand.Execute(null);

            AssertQuantityChecking(viewmodel.DataGridViewModel.Items);
        }

        /// <summary>
        /// 삭제 후 잔여수량과 재고수량을 업데이트 한다.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [Ignore]
        public void DeleteCheckedItemThenSyncInventoryQty()
        {
            var viewmodel = SelectSomeTreeViewNode();
            var item = viewmodel.DataGridViewModel.SelectedItem = viewmodel.DataGridViewModel.Items.Random();
            item.IsChecked = true;
            int inQty = item.Inventory.Quantity;
            string iosID = item.ID;

            viewmodel.DataGridViewModel.ChekcedIOStockFormatsDeletionCommand.Execute(null);
            int inQty2 = DataDirector.GetInstance().SearchInventory(item.Inventory.ID).Quantity;

            Assert.AreNotEqual(inQty, inQty2);
        }

        [TestMethod]
        public void DeleteCheckedItemsThenSyncDataGridItems()
        {
            var viewmodel = SelectSomeTreeViewNode();
            for (int i = 0; i < 10; i++)
            {
                int idx = new Random().Next(0, viewmodel.DataGridViewModel.Items.Count());
                var item = viewmodel.DataGridViewModel.Items.ElementAt(idx);
                if (item.IsChecked != true)
                    item.IsChecked = true;
            }

            viewmodel.DataGridViewModel.ChekcedIOStockFormatsDeletionCommand.Execute(null);

            Assert.IsTrue(viewmodel.DataGridViewModel.Items.All(x => x.IsChecked != true));
        }

        [TestMethod]
        public void DeleteCheckedItemsThenSyncDb()
        {
            var viewmodel = SelectSomeTreeViewNode();
            for (int i = 0; i < 10; i++)
            {
                int idx = new Random().Next(0, viewmodel.DataGridViewModel.Items.Count());
                var item = viewmodel.DataGridViewModel.Items.ElementAt(idx);
                if (item.IsChecked != true)
                    item.IsChecked = true;
            }
            var checkedItems = viewmodel.DataGridViewModel.Items.Where(x => x.IsChecked == true).ToList();

            viewmodel.DataGridViewModel.ChekcedIOStockFormatsDeletionCommand.Execute(null);

            foreach (var item in checkedItems)
            {
                var iofmts = DataDirector.GetInstance().DB.Select<IOStockFormat>(item.ID);
                Assert.IsNull(iofmts);
            }
        }

        [Ignore]
        [TestMethod]
        public void DeleteCheckedItemsThenSyncQty()
        {
            var viewmodel = SelectSomeTreeViewNode();
            for (int i = 0; i < 10; i++)
            {
                int idx = new Random().Next(0, viewmodel.DataGridViewModel.Items.Count());
                var item = viewmodel.DataGridViewModel.Items.ElementAt(idx);
                if (item.IsChecked != true)
                    item.IsChecked = true;
            }

            viewmodel.DataGridViewModel.ChekcedIOStockFormatsDeletionCommand.Execute(null);

            AssertQuantityChecking(viewmodel.DataGridViewModel.Items);
        }

        /// <summary>
        /// 제품을 검색하기
        /// </summary>
        [TestMethod]
        [Ignore]
        public void SearchProductName()
        {
            var viewmodel = new IOStockStatusViewModel();
            var oid = DataDirector.GetInstance();
            var text = viewmodel.SearchViewModel.Text = oid.CopyInventories().Select(x => x.Product).Distinct().Random().Name;

            viewmodel.SearchViewModel.SearchCommand.Execute(null);

            var items = viewmodel.DataGridViewModel.Items;
            Assert.AreNotEqual(0, items.Count());
        }

        /// <summary>
        /// 규격을 검색하기
        /// </summary>
        [Ignore]
        [TestMethod]
        public void SearchSpecificationName()
        {
            var viewmodel = new IOStockStatusViewModel();
            var oid = DataDirector.GetInstance();
            var text = viewmodel.SearchViewModel.Text = oid.CopyInventories().Random().Specification;

            viewmodel.SearchViewModel.SearchCommand.Execute(null);

            var items = viewmodel.DataGridViewModel.Items;
            Assert.AreNotEqual(0, items.Count());
        }

        /// <summary>
        /// 제품과 규격 검색하기
        /// </summary>
        [Ignore]
        [TestMethod]
        public void SearchCommand()
        {
            var viewmodel = new IOStockStatusViewModel();
            var oid = DataDirector.GetInstance();
            var text1 = oid.CopyInventories().Random().Specification;
            var text2 = oid.CopyInventories().Select(x => x.Product).Distinct().Random().Name;

            viewmodel.SearchViewModel.Text = text1 + " " + text2;
            viewmodel.SearchViewModel.SearchCommand.Execute(null);

            var items = viewmodel.DataGridViewModel.Items;
            Assert.AreNotEqual(0, items.Count());
        }

        public void AssertQuantityChecking(IEnumerable<IOStockDataGridItem> items)
        {
            var lookup = items.ToLookup(x => x.Inventory);
            foreach (var item in lookup)
            {
                var orderedItem = item.OrderBy(x => x.Date);
                var lastItem = orderedItem.Last();
                Assert.AreEqual(lastItem.RemainingQuantity, item.Key.Quantity);
                int? reQty = null;
                int iosQty, exp;
                foreach (var i in orderedItem)
                {
                    iosQty = i.Quantity;
                    if (i.StockType == IOStockType.OUTGOING)
                        iosQty = -iosQty;
                    if (reQty == null)
                        reQty = 0;
                    exp = (int)reQty + iosQty;
                    Assert.AreEqual(i.RemainingQuantity, exp);
                    reQty = exp;
                }
            }
        }

        public IOStockStatusViewModel SelectSomeTreeViewNode()
        {
            var viewmodel = new IOStockStatusViewModel();
            viewmodel.SelectedDataGridGroupOption = IOStockStatusViewModel.DATAGRID_OPTION_PRODUCT;
            TreeViewNode node = viewmodel.TreeViewViewModel.SearchNodeInRoot(NodeType.PRODUCT).Random();
            viewmodel.TreeViewViewModel.AddSelectedNodes(node);
            return viewmodel;
        }

        [TestMethod]
        public void TestProductSearch()
        {
            var viewmodel = new IOStockStatusViewModel();
            var searchvm = viewmodel.SearchViewModel;
            searchvm.SelectedItem = FilterSearchTextBoxViewModel.FILTER_PRODUCT;
            searchvm.Text = "PBL";

            searchvm.SearchCommand.Execute(null);

            Assert.IsTrue(viewmodel.DataGridViewModel.Items.All(x => x.Inventory.Product.Name.Contains(searchvm.Text)));
        }

        [TestMethod]
        public void TestSpecificationSearch()
        {
            var viewmodel = new IOStockStatusViewModel();
            var searchvm = viewmodel.SearchViewModel;
            searchvm.SelectedItem = FilterSearchTextBoxViewModel.FILTER_SPECIFICATION;
            searchvm.Text = "AW";

            searchvm.SearchCommand.Execute(null);

            Assert.IsTrue(viewmodel.DataGridViewModel.Items.All(x => x.Inventory.Specification.Contains(searchvm.Text)));
        }

        [TestMethod]
        public void TestMakerSearch()
        {
            var viewmodel = new IOStockStatusViewModel();
            var searchvm = viewmodel.SearchViewModel;
            searchvm.SelectedItem = FilterSearchTextBoxViewModel.FILTER_MAKER;
            searchvm.Text = "LG";

            searchvm.SearchCommand.Execute(null);

            Assert.IsTrue(viewmodel.DataGridViewModel.Items.All(x => x.Inventory.Maker.Name.Contains(searchvm.Text)));
        }

        [TestMethod]
        public void TestSuppilerSearch()
        {
            var viewmodel = new IOStockStatusViewModel();
            var searchvm = viewmodel.SearchViewModel;
            searchvm.SelectedItem = FilterSearchTextBoxViewModel.FILTER_SUPPLIER;
            searchvm.Text = "테크";

            searchvm.SearchCommand.Execute(null);

            Assert.IsTrue(viewmodel.DataGridViewModel.Items.All(x => x.Supplier.Name.Contains(searchvm.Text)));
        }

        [TestMethod]
        public void TestWarehouseSearch()
        {
            var viewmodel = new IOStockStatusViewModel();
            var searchvm = viewmodel.SearchViewModel;
            searchvm.SelectedItem = FilterSearchTextBoxViewModel.FILTER_WAREHOUSE;
            searchvm.Text = "연구";

            searchvm.SearchCommand.Execute(null);

            Assert.IsTrue(viewmodel.DataGridViewModel.Items.All(x => x.Warehouse.Name.Contains(searchvm.Text)));
        }

        [TestMethod]
        public void TestCustomerSearch()
        {
            var viewmodel = new IOStockStatusViewModel();
            var searchvm = viewmodel.SearchViewModel;
            searchvm.SelectedItem = FilterSearchTextBoxViewModel.FILTER_CUSTOMER;
            searchvm.Text = "테크";

            searchvm.SearchCommand.Execute(null);

            Assert.IsTrue(viewmodel.DataGridViewModel.Items.All(x => x.Customer.Name.Contains(searchvm.Text)));
        }

        [TestMethod]
        public void ProjectListItemDeleteTest()
        {
            IOStockStatusViewModel viewmodel = new IOStockStatusViewModel();
            viewmodel.SelectedDataGridGroupOption = IOStockStatusViewModel.DATAGRID_OPTION_PROJECT;
            Observable<Project> project = viewmodel.ProjectListBoxViewModel.SelectedItem = viewmodel.ProjectListBoxViewModel.Items.Random();
            viewmodel.ProjectListBoxViewModel.ProjectDeletionCommand.Execute(null);

            Observable<Project> result = DataDirector.GetInstance().SearchField<Project>(project.ID);
            Assert.IsNull(result);
            Assert.IsFalse(viewmodel.ProjectListBoxViewModel.Items.Contains(project));
            //Assert.AreEqual(0, viewmodel.DataGridViewModel.Items.Count());
            Assert.IsNull(viewmodel.DataGridViewModel.SelectedItem);
        }

        /// <summary>
        /// 인벤토리 노드 하나를 클릭할 경우 관련 데이터를 데이터그리드에 업데이트
        /// </summary>
        [TestMethod]
        public void TestTreeViewSelect()
        {
            var viewmodel = new IOStockStatusViewModel();
            var node = viewmodel.TreeViewViewModel.Root.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.INVENTORY)).Random();
            viewmodel.TreeViewViewModel.AddSelectedNodes(node);

            Assert.IsTrue(viewmodel.DataGridViewModel.Items.All(x => x.Inventory.ID == node.ObservableObjectID));
        }

        /// <summary>
        /// 제품 노드를 하나 클릭한 경우 관련 데이터를 데이터그리드에 업데이트
        /// </summary>
        [TestMethod]
        public void TestTreeViewSelect2()
        {
            var viewmodel = new IOStockStatusViewModel();
            var node = viewmodel.TreeViewViewModel.SearchNodeInRoot(NodeType.PRODUCT).Random();
            viewmodel.TreeViewViewModel.AddSelectedNodes(node);

            var inventories = DataDirector.GetInstance().SearchInventories(node.ObservableObjectID);
            var inventoryIds = inventories.Select(x => x.ID);

            Assert.IsTrue(viewmodel.DataGridViewModel.Items.All(x => x.Inventory.Product.ID == node.ObservableObjectID));
        }

        /// <summary>
        /// 제품노드와 그 하위 노드를 클릭한 경우 제품노드와 관련된 데이터를 데이터그리드에 업데이트
        /// </summary>
        [TestMethod]
        public void TestTreeViewSelect3()
        {
            var viewmodel = new IOStockStatusViewModel();
            var productNode = viewmodel.TreeViewViewModel.SearchNodeInRoot(NodeType.PRODUCT).Random();
            var inventoryNode = productNode.Root.Random();
            viewmodel.TreeViewViewModel.AddSelectedNodes(productNode);
            viewmodel.TreeViewViewModel.AddSelectedNodes(inventoryNode);

            var inventories = DataDirector.GetInstance().SearchInventories(productNode.ObservableObjectID);
            var inventoryIds = inventories.Select(x => x.ID);

            Assert.IsTrue(viewmodel.DataGridViewModel.Items.All(x => x.Inventory.Product.ID == productNode.ObservableObjectID));
        }

        [TestMethod]
        public void DeleteInventoryNodeThenSyncTreeView()
        {
            var viewmodel = new IOStockStatusViewModel();
            var productNode = viewmodel.TreeViewViewModel.SearchNodeInRoot(NodeType.PRODUCT).Random();
            var inventoryNode = productNode.Root.Random();
            var treeview = viewmodel.TreeViewViewModel;
            treeview.NodesSelectedEventCommand.Execute(new SelectionChangedCancelEventArgs(new TreeViewNode[] { inventoryNode }, null));
            Assert.IsTrue(viewmodel.DataGridViewModel.Items.All(x => x.Inventory.ID == inventoryNode.ObservableObjectID));
            //삭제가 가능함
            Assert.IsTrue(treeview.CanDeleteNode());
            //삭제 명령
            treeview.SelectedNodeDeletionCommand.Execute(null);
            //inventory 리스트에서도 삭제 확인
            var inven = DataDirector.GetInstance().SearchInventory(inventoryNode.ObservableObjectID);
            Assert.IsNull(inven);
            //treeview에서도 삭제 확인
            Assert.IsFalse(TreeViewNodeDirector.GetInstance().Contains(inventoryNode));
            //datagrid에서 삭제 확인
            Assert.IsFalse(viewmodel.DataGridViewModel.Items.Any(x => x.Inventory.ID == inventoryNode.ObservableObjectID));
        }

        [TestMethod]
        [Ignore]
        public void CalcQuantity()
        {
            var viewmodel = new IOStockStatusViewModel();
            var node = viewmodel.TreeViewViewModel.SearchNodeInRoot(NodeType.INVENTORY).Random();
            viewmodel.TreeViewViewModel.AddSelectedNodes(node);
            IEnumerable<IOStockDataGridItem> items = viewmodel.DataGridViewModel.Items;
            IEnumerable<IOStockDataGridItem> orderedItems = items.OrderBy(x => x.Date);

            foreach (var i in orderedItems)
            {
                int remainQty = 0;
                string sql = string.Format(@"select
                                            (select sum(Quantity) from IOStockFormat where InventoryID = '{0}' and StockType = '{1}' and Date <= '{3}') -
                                            (select sum(Quantity) from IOStockFormat where InventoryID = '{0}' and StockType = '{2}' and Date <= '{3}');",
                                             i.InventoryID, (int)IOStockType.INCOMING, (int)IOStockType.OUTGOING, i.Date.ToString(MySQLClient.DATETIME));
                using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        remainQty = reader.GetInt32(0);
                }
                Assert.AreEqual(remainQty, i.RemainingQuantity);
            }
        }

        /// <summary>
        /// buffix 프로젝트를 삭제할 시 에러 발생
        /// </summary>
        [TestMethod]
        [Ignore]
        public void DeleteSelectedProject()
        {
            var viewmodel = new IOStockStatusViewModel();
            viewmodel.SelectedDataGridGroupOption = IOStockStatusViewModel.DATAGRID_OPTION_PROJECT;
            viewmodel.ProjectListBoxViewModel.SelectedItem = viewmodel.ProjectListBoxViewModel.Items.Random();

            viewmodel.ProjectListBoxViewModel.ExecuteProjectDeletionCommand();

            var items = viewmodel.DataGridViewModel.Items;
            Assert.AreEqual(0, items.Count());
        }

        [TestMethod]
        public void ChangeQuantityProperty()
        {
            var viewmodel = SelectSomeTreeViewNode();

            var item = viewmodel.DataGridViewModel.Items.Random();
            var mvm = new IOStockManagerViewModel(viewmodel, item);
            mvm.Quantity = 100;
            mvm.Update();

            AssertQuantityChecking(viewmodel.DataGridViewModel.Items);
        }

        [TestMethod]
        public void ChangeDateTimeProperty()
        {
            var viewmodel = SelectSomeTreeViewNode();

            var item = viewmodel.DataGridViewModel.Items.Random();
            var mvm = new IOStockManagerViewModel(viewmodel, item);
            mvm.SelectedDate = DateTime.Now;
            mvm.Update();

            AssertQuantityChecking(viewmodel.DataGridViewModel.Items);
        }

        [TestMethod]
        public void ChangeStockTypeProperty()
        {
            var viewmodel = SelectSomeTreeViewNode();

            var inc = viewmodel.DataGridViewModel.Items.Where(x => x.StockType == IOStockType.INCOMING);
            var item = viewmodel.DataGridViewModel.Items.Where(x => x.StockType == IOStockType.INCOMING).Random();
            var mvm = new IOStockManagerViewModel(viewmodel, item);
            mvm.StockType = IOStockType.OUTGOING;
            mvm.Update();
            AssertQuantityChecking(viewmodel.DataGridViewModel.Items);
        }

        /// <summary>
        /// bugfix io status 에서 project list가 선택되어 있을 때 새로운 iostock 등록 시 에러 발생
        /// </summary>
        [TestMethod]
        public void AddNewStockData()
        {
            IOStockStatusViewModel viewmodel = new IOStockStatusViewModel();
            viewmodel.SelectedDataGridGroupOption = IOStockStatusViewModel.DATAGRID_OPTION_PROJECT;
            Observable<Project> project = viewmodel.ProjectListBoxViewModel.SelectedItem = viewmodel.ProjectListBoxViewModel.Items.Random();

            var mvm = new IOStockManagerViewModel(DataDirector.GetInstance().CopyInventories().Random());
            mvm.Insert();
        }
    }
}