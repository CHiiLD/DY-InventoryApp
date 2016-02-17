using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace R54IN0.Test
{
    [TestClass]
    public class TreeViewViewModelUnitTest
    {
        public TreeViewNode GetProductNode(MultiSelectTreeViewModelView viewmodel)
        {
            return viewmodel.Root.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).Random();
        }

        public bool Has(MultiSelectTreeViewModelView viewmodel, TreeViewNode node)
        {
            return viewmodel.Root.Any(x => x.Descendants().Any(y => y == node));
        }

        /// <summary>
        /// 트리뷰에서 제품 노드를 삭제하고 트리뷰에서 삭제됨을 확인함
        /// </summary>
        [TestMethod]
        public void DeleteProductNodeThenSyncTreeView()
        {
            new Dummy().Create();
            var treeview = new MultiSelectTreeViewModelView();
            var node = GetProductNode(treeview);
            treeview.SelectedNodes.Add(node);

            treeview.SelectedNodeDeletionCommand.Execute(null);

            Assert.IsFalse(Has(treeview, node));
        }

        /// <summary>
        /// 트리뷰에서 제품 노드를 삭제하고 재고, 입출고 현황 뷰모델의 데이터그리드 또한 동기화한다.
        /// </summary>
        [TestMethod]
        public void DeleteProductNodeThenSyncDataGrid()
        {
            new Dummy().Create();
            var treeview = new MultiSelectTreeViewModelView();
            var ivm = new InventoryStatusViewModel();
            var iovm = new IOStockStatusViewModel();
            var node = GetProductNode(treeview);
            treeview.SelectedNodes.Add(node);
            iovm.SelectedGroupItem = IOStockStatusViewModel.GROUPITEM_PRODUCT;
            iovm.TreeViewViewModel.ExecuteNodesSelectedEventCommand(new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node }, null));
            Console.WriteLine("IOStock Status View Model 데이터 그리드 아이템들의 제품 범위식별자 리스트, 삭제할 ID: " + node.ObservableObjectID);
            iovm.DataGridViewModel.Items.ToList().ForEach(x => Console.WriteLine(x.Inventory.Product.ID));

            treeview.SelectedNodeDeletionCommand.Execute(null);

            Assert.IsTrue(ivm.GetDataGridItems().Any(x => x.Product.ID != node.ObservableObjectID));
            Assert.IsTrue(iovm.DataGridViewModel.Items.Count() == 0 || iovm.DataGridViewModel.Items.Any(x => x.Inventory.Product.ID != node.ObservableObjectID));
            Console.WriteLine("삭제 후 업데이트 된 데이터 그리드 리스트");
            iovm.DataGridViewModel.Items.ToList().ForEach(x => Console.WriteLine(x.Inventory.Product.ID));
        }

        /// <summary>
        /// 트리뷰에서 제품 노드를 삭제하고 디렉터에서 관련 컬렉션 또한 동기화한다.
        /// </summary>
        [TestMethod]
        public void DeleteProductNodeThenSyncDirector()
        {
            new Dummy().Create();
            var treeview = new MultiSelectTreeViewModelView();
            var node = GetProductNode(treeview);
            treeview.SelectedNodes.Add(node);

            treeview.SelectedNodeDeletionCommand.Execute(null);

            Assert.IsNull(DataDirector.GetInstance().SearchField<Product>(node.ObservableObjectID));
            Assert.AreEqual(0, DataDirector.GetInstance().SearchInventories(node.ObservableObjectID).Count());
        }

        /// <summary>
        /// 트리뷰에서 제품 노드를 삭제하고 데이터베이스에서도 관련 자료를 삭제한다.
        /// </summary>
        [TestMethod]
        public void DeleteProductNodeThenSyncDb()
        {
            new Dummy().Create();
            var treeview = new MultiSelectTreeViewModelView();
            var node = GetProductNode(treeview);
            treeview.SelectedNodes.Add(node);

            var product = DataDirector.GetInstance().SearchField<Product>(node.ObservableObjectID);
            if (product != null)
                CollectionViewModelObserverSubject.GetInstance().NotifyItemDeleted(product);
            var oid = DataDirector.GetInstance();
            var invens = oid.SearchInventories(product.ID).ToList();

            treeview.SelectedNodeDeletionCommand.Execute(null);

            foreach (var inven in invens)
            {
                var iosfmts = DataDirector.GetInstance().DB.Query<IOStockFormat>(
                    "select * from IOStockFormat where {0} = '{1}';",
                    "InventoryID", inven.ID);
                Assert.AreEqual(0, iosfmts.Count());
            }
            var infmts = DataDirector.GetInstance().DB.Query<InventoryFormat>(
                "select * from InventoryFormat where {0} = '{1}';",
                "ProductID", product.ID);
            Assert.AreEqual(0, infmts.Count());
            Assert.IsNull(DataDirector.GetInstance().DB.Select<Product>(product.ID));
        }

        [TestMethod]
        public void WhenCreateNewProjectTypeTreeViewNodeThenNameInitializedAsProductName()
        {
            new Dummy().Create();
            var someProduct = DataDirector.GetInstance().CopyFields<Product>().Random();
            var treeview = new TreeViewNode(someProduct);

            Assert.AreEqual(treeview.Name, someProduct.Name);
        }
    }
}