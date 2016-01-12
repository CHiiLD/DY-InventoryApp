using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace R54IN0.Test.New
{
    [TestClass]
    public class InventoryStatusViewModelTest
    {
        [TestMethod]
        public void CanCreate()
        {
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
            viewmodel.TreeViewViewModel.OnNodeSelected(null, new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node }, new List<TreeViewNode>()));
            Assert.IsTrue(viewmodel.GetDataGridItems().All(x => x.Product.ID == node.ProductID));

            //다른 NODE하나를 선택
            var node2 = viewmodel.TreeViewViewModel.Root.Where(x => x.Type == NodeType.PRODUCT).Random();
            viewmodel.TreeViewViewModel.OnNodeSelected(null, new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node2 }, new List<TreeViewNode>() { node }));
            Assert.IsTrue(viewmodel.GetDataGridItems().All(x => x.Product.ID == node2.ProductID));

            //NODE 2개를 선택
            viewmodel.TreeViewViewModel.OnNodeSelected(null, new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node }, new List<TreeViewNode>() { node }));
            Assert.IsTrue(viewmodel.GetDataGridItems().All(x => x.Product.ID == node2.ProductID || x.Product.ID == node.ProductID));
        }
    }
}