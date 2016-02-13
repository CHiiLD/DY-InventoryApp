﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace R54IN0.Test.New
{
    [TestClass]
    public class InventoryStatusViewModelUnitTest
    {
        [TestMethod]
        public void CanCreate()
        {
            new Dummy().Create();
            new InventoryStatusViewModel();
        }

        /// <summary>
        /// 재고현황의 데이터그리드들과 데이터그리드의 아이템 개수를 체크한다. (항상 좌측이 많아야 한다)
        /// </summary>
        [TestMethod]
        public void DataGridInitializationTest()
        {
            new Dummy().Create();
            var viewmodel = new InventoryStatusViewModel();
            Assert.IsNotNull(viewmodel.DataGridViewModel1.Items);
            Assert.IsNotNull(viewmodel.DataGridViewModel2.Items);
            Assert.IsTrue(viewmodel.DataGridViewModel1.Items.Count >= viewmodel.DataGridViewModel2.Items.Count);
        }

        /// <summary>
        /// 객체의 체크박스와 컨트롤의 Visibility 옵션의 연동을 체크
        /// </summary>
        [TestMethod]
        public void ColumnCheckBoxTest()
        {
            var viewmodel = new InventoryStatusViewModel();

            viewmodel.ShowMakerColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.MakerColumnVisibility);

            viewmodel.ShowProductColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.ProductColumnVisibility);

            viewmodel.ShowMeasureColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.MeasureColumnVisibility);

            viewmodel.ShowMakerColumn = false;
            Assert.AreEqual(Visibility.Collapsed, viewmodel.DataGridViewModel1.MakerColumnVisibility);

            viewmodel.ShowProductColumn = false;
            Assert.AreEqual(Visibility.Collapsed, viewmodel.DataGridViewModel1.ProductColumnVisibility);

            viewmodel.ShowMeasureColumn = false;
            Assert.AreEqual(Visibility.Collapsed, viewmodel.DataGridViewModel1.MeasureColumnVisibility);

            viewmodel.ShowMakerColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.MakerColumnVisibility);

            viewmodel.ShowProductColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.ProductColumnVisibility);

            viewmodel.ShowMeasureColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.MeasureColumnVisibility);
        }

        /// <summary>
        /// TreeViewNode의 이름 변경을 검사
        /// </summary>
        [TestMethod]
        public void ReNameTreeViewNode()
        {
            new Dummy().Create();
            var viewmodel = new InventoryStatusViewModel();
            var node = viewmodel.TreeViewViewModel.Root.Where(x => x.Type == NodeType.PRODUCT).Random();
            node.IsNameEditable = false;
            string name = node.Name = "플라리아";
            Assert.IsFalse(viewmodel.DataGridViewModel1.Items.Any(item => item.Product.Name == name));
            Assert.IsFalse(viewmodel.DataGridViewModel2.Items.Any(item => item.Product.Name == name));

            node.IsNameEditable = true;
            name = node.Name = "플라리아";
            Assert.IsTrue(viewmodel.DataGridViewModel1.Items.Any(item => item.Product.Name == name) || viewmodel.DataGridViewModel2.Items.Any(item => item.Product.Name == name));
        }

        /// <summary>
        /// TreeView에서 Node를 여러개 선택했을 경우 관련 재고 데이터를 데이터그리드에 업데이트한다.
        /// </summary>
        [TestMethod]
        public void WhenSelectNodesThenUpdateDataGrid()
        {
            new Dummy().Create();
            var viewmodel = new InventoryStatusViewModel();
            //NODE 하나 선택
            var node = viewmodel.TreeViewViewModel.Root.Where(x => x.Type == NodeType.PRODUCT).Random();
            viewmodel.TreeViewViewModel.ExecuteNodesSelectedEventCommand(new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node }, new List<TreeViewNode>()));
            Assert.IsTrue(viewmodel.GetDataGridItems().All(x => x.Product.ID == node.ObservableObjectID));

            //다른 NODE하나를 선택
            var node2 = viewmodel.TreeViewViewModel.Root.Where(x => x.Type == NodeType.PRODUCT).Random();
            viewmodel.TreeViewViewModel.ExecuteNodesSelectedEventCommand(new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node2 }, new List<TreeViewNode>() { node }));
            Assert.IsTrue(viewmodel.GetDataGridItems().All(x => x.Product.ID == node2.ObservableObjectID));

            //NODE 2개를 선택
            viewmodel.TreeViewViewModel.ExecuteNodesSelectedEventCommand(new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node }, new List<TreeViewNode>() { node }));
            Assert.IsTrue(viewmodel.GetDataGridItems().All(x => x.Product.ID == node2.ObservableObjectID || x.Product.ID == node.ObservableObjectID));
        }

        /// <summary>
        /// 데이터그리드의 아이템을 하나 선택 후 SelectedNodeDeletionCommand을 작동시킬 경우
        /// 데이터그리드에서 해당하는 아이템을 담당하는 셀이 없어져야 한다.
        /// </summary>
        [TestMethod]
        public void DeleteItemThenSyncDataGridItems()
        {
            new Dummy().Create();
            var viewmodel = new InventoryStatusViewModel();
            var item = viewmodel.DataGridViewModel1.SelectedItem = viewmodel.DataGridViewModel1.Items.Random();

            viewmodel.DataGridViewModel1.InventoryDataDeletionCommand.Execute(null);

            Assert.IsTrue(viewmodel.GetDataGridItems().All(x => x.ID != item.ID));
        }

        /// <summary>
        /// 삭제 시 IOStockStatusViewModel 역시 동기화를 하여야 한다.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void DeleteItemThenSyncIOStockViewModel()
        {
            new Dummy().Create();
            var viewmodel = new InventoryStatusViewModel();
            var iosvm = new IOStockStatusViewModel();
            iosvm.SelectedGroupItem = IOStockStatusViewModel.GROUPITEM_PRODUCT;
            var item = viewmodel.DataGridViewModel1.SelectedItem = viewmodel.DataGridViewModel1.Items.Random();
            string inventoryID = item.ID;
            Console.WriteLine("삭제할 Inventory ID: " + inventoryID);
            var node = viewmodel.TreeViewViewModel.Root.SelectMany(x => x.Descendants().Where(y => y.ObservableObjectID == item.Product.ID)).Single();
            iosvm.TreeViewViewModel.ExecuteNodesSelectedEventCommand(new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node }, null));

            Console.WriteLine("삭제 전 입출고 현황 데이터그리드 아이템 Inventory ID 목록");
            iosvm.DataGridViewModel.Items.ToList().ForEach(x => Console.WriteLine(x.Inventory.ID));

            viewmodel.DataGridViewModel1.InventoryDataDeletionCommand.Execute(null);

            Console.WriteLine("삭제 후 입출고 현황 데이터그리드 아이템 Inventory ID 목록");
            iosvm.DataGridViewModel.Items.ToList().ForEach(x => Console.WriteLine(x.Inventory.ID));
            Assert.IsTrue(iosvm.DataGridViewModel.Items.All(x => x.Inventory.ID != inventoryID));
        }

        /// <summary>
        /// 삭제 후 데이터베이스에서 관련 자료가 모두 삭제되어야 한다. (인벤토리, 인벤토리와 관련된 입출고 데이터)
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void DeleteItemThenSyncDb()
        {
            new Dummy().Create();
            var viewmodel = new InventoryStatusViewModel();
            var item = viewmodel.DataGridViewModel1.SelectedItem = viewmodel.DataGridViewModel1.Items.Random();
            string inventoryID = item.ID;

            viewmodel.DataGridViewModel1.InventoryDataDeletionCommand.Execute(null);

            var infmt = InventoryDataCommander.GetInstance().DB.Select<InventoryFormat>("ID", inventoryID);
            Assert.IsNull(infmt);

            var iofmts = InventoryDataCommander.GetInstance().DB.Query<IOStockFormat>("select * from {0} where {1} = '{2}';", typeof(IOStockFormat).Name, "InventoryID", inventoryID);
            Assert.AreEqual(0, iofmts.Count());
        }

        /// <summary>
        /// 삭제 후 데이터 관리자의 데이터도 동기화 되어야 한다.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void DeleteItemThenSyncDirector()
        {
            new Dummy().Create();
            var viewmodel = new InventoryStatusViewModel();
            var item = viewmodel.DataGridViewModel1.SelectedItem = viewmodel.DataGridViewModel1.Items.Random();
            string inventoryID = item.ID;

            viewmodel.DataGridViewModel1.InventoryDataDeletionCommand.Execute(null);

            var result = InventoryDataCommander.GetInstance().SearchInventory(inventoryID);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void WhenDeleteMakerSyncMakers()
        {
            new Dummy().Create();
            var viewmodel = new InventoryStatusViewModel();

            var someMaker = InventoryDataCommander.GetInstance().CopyFields<Maker>().Random();
            Assert.IsTrue(viewmodel.DataGridViewModel1.Makers.Contains(someMaker));
            InventoryDataCommander.GetInstance().RemoveObservableField(someMaker);
            Assert.IsFalse(viewmodel.DataGridViewModel1.Makers.Contains(someMaker));
        }

        [TestMethod]
        public void WhenDeleteMeasureSyncMeasures()
        {
            new Dummy().Create();
            var viewmodel = new InventoryStatusViewModel();

            var someMeasure = InventoryDataCommander.GetInstance().CopyFields<Measure>().Random();
            Assert.IsTrue(viewmodel.DataGridViewModel1.Measures.Contains(someMeasure));
            InventoryDataCommander.GetInstance().RemoveObservableField(someMeasure);
            Assert.IsFalse(viewmodel.DataGridViewModel1.Measures.Contains(someMeasure));
        }

        [TestMethod]
        public void WhenAddNewMakerSyncMakers()
        {
            new Dummy().Create();
            var viewmodel = new InventoryStatusViewModel();

            var someMaker = new Observable<Maker>("some maker");
            Assert.IsFalse(viewmodel.DataGridViewModel1.Makers.Contains(someMaker));
            InventoryDataCommander.GetInstance().AddObservableField(someMaker);
            Assert.IsTrue(viewmodel.DataGridViewModel1.Makers.Contains(someMaker));
        }

        [TestMethod]
        public void WhenAddNewMeasureSyncMeasures()
        {
            new Dummy().Create();
            var viewmodel = new InventoryStatusViewModel();

            var someMeasure = new Observable<Measure>("some measure");
            Assert.IsFalse(viewmodel.DataGridViewModel1.Measures.Contains(someMeasure));
            InventoryDataCommander.GetInstance().AddObservableField(someMeasure);
            Assert.IsTrue(viewmodel.DataGridViewModel1.Measures.Contains(someMeasure));
        }

        /// <summary>
        /// 인벤토리 노드 하나를 클릭할 경우 관련 데이터를 데이터그리드에 업데이트
        /// </summary>
        [TestMethod]
        public void TestTreeViewSelect()
        {
            new Dummy().Create();
            var viewmodel = new InventoryStatusViewModel();
            var node = viewmodel.TreeViewViewModel.Root.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.INVENTORY)).Random();
            viewmodel.TreeViewViewModel.NodesSelectedEventCommand.Execute(new SelectionChangedCancelEventArgs(new TreeViewNode[] { node }, null));

            Assert.AreEqual(viewmodel.GetDataGridItems().Single().ID, node.ObservableObjectID);
        }

        /// <summary>
        /// 제품 노드를 하나 클릭한 경우 관련 데이터를 데이터그리드에 업데이트
        /// </summary>
        [TestMethod]
        public void TestTreeViewSelect2()
        {
            new Dummy().Create();
            var viewmodel = new InventoryStatusViewModel();
            var node = viewmodel.TreeViewViewModel.Root.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).Random();
            viewmodel.TreeViewViewModel.NodesSelectedEventCommand.Execute(new SelectionChangedCancelEventArgs(new TreeViewNode[] { node }, null));

            var inventories = InventoryDataCommander.GetInstance().SearchInventoryAsProductID(node.ObservableObjectID);
            var inventoryIds = inventories.Select(x => x.ID);
            Assert.IsTrue(viewmodel.GetDataGridItems().All(x => inventoryIds.Contains(x.ID)));
        }

        /// <summary>
        /// 제품노드와 그 하위 노드를 클릭한 경우 제품노드와 관련된 데이터를 데이터그리드에 업데이트
        /// </summary>
        [TestMethod]
        public void TestTreeViewSelect3()
        {
            new Dummy().Create();
            var viewmodel = new InventoryStatusViewModel();
            var productNode = viewmodel.TreeViewViewModel.Root.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).Random();
            var inventoryNode = productNode.Root.Random();
            viewmodel.TreeViewViewModel.NodesSelectedEventCommand.Execute(new SelectionChangedCancelEventArgs(new TreeViewNode[] { productNode, inventoryNode }, null));

            var inventories = InventoryDataCommander.GetInstance().SearchInventoryAsProductID(productNode.ObservableObjectID);
            var inventoryIds = inventories.Select(x => x.ID);
            Assert.IsTrue(viewmodel.GetDataGridItems().All(x => inventoryIds.Contains(x.ID)));
        }

        [TestMethod]
        public void DeleteInventoryNodeThenSyncTreeView()
        {
            new Dummy().Create();
            var viewmodel = new InventoryStatusViewModel();
            var productNode = viewmodel.TreeViewViewModel.Root.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).Random();
            var inventoryNode = productNode.Root.Random();
            var treeview = viewmodel.TreeViewViewModel;
            treeview.NodesSelectedEventCommand.Execute(new SelectionChangedCancelEventArgs(new TreeViewNode[] { inventoryNode }, null));
            Assert.IsTrue(viewmodel.GetDataGridItems().All(x => x.ID == inventoryNode.ObservableObjectID));
            //삭제가 가능함
            Assert.IsTrue(treeview.CanDeleteNode());
            //삭제 명령
            treeview.SelectedNodeDeletionCommand.Execute(null);
            //inventory 리스트에서도 삭제 확인
            var inven = InventoryDataCommander.GetInstance().SearchInventory(inventoryNode.ObservableObjectID);
            Assert.IsNull(inven);
            //treeview에서도 삭제 확인
            Assert.IsFalse(TreeViewNodeDirector.GetInstance().Contains(inventoryNode));
            //datagrid에서 삭제 확인
            Assert.IsFalse(viewmodel.GetDataGridItems().Any(x => x.ID == inventoryNode.ObservableObjectID));
        }
    }
}