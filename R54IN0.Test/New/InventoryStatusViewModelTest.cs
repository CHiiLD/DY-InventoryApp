using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;

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

        [TestMethod]
        public void DataGridInitializationTest()
        {
            var viewmodel = new InventoryStatusViewModel();
            Assert.IsNotNull(viewmodel.DataGridViewModel1.Items);
            Assert.IsNotNull(viewmodel.DataGridViewModel2.Items);
            Assert.IsTrue(viewmodel.DataGridViewModel1.Items.Count >= viewmodel.DataGridViewModel2.Items.Count);
        }

        [TestMethod]
        public void ColumnCheckBoxTest()
        {
            var viewmodel = new InventoryStatusViewModel();

            viewmodel.ShowMakerColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.MakerVisibility);

            viewmodel.ShowProductColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.ProductVisibility);

            viewmodel.ShowMeasureColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.MeasureVisibility);

            viewmodel.ShowMakerColumn = false;
            Assert.AreEqual(Visibility.Collapsed, viewmodel.DataGridViewModel1.MakerVisibility);

            viewmodel.ShowProductColumn = false;
            Assert.AreEqual(Visibility.Collapsed, viewmodel.DataGridViewModel1.ProductVisibility);

            viewmodel.ShowMeasureColumn = false;
            Assert.AreEqual(Visibility.Collapsed, viewmodel.DataGridViewModel1.MeasureVisibility);

            viewmodel.ShowMakerColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.MakerVisibility);

            viewmodel.ShowProductColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.ProductVisibility);

            viewmodel.ShowMeasureColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.MeasureVisibility);
        }

        [TestMethod]
        public void TreeViewNodeNameChanged()
        {
            new Dummy2().Create();
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

        [TestMethod]
        public void WhenSelectNodeThenUpdateDataGrid()
        {
            new Dummy2().Create();
            var viewmodel = new InventoryStatusViewModel();
            //하나 선택
            var node = viewmodel.TreeViewViewModel.Root.Where(x => x.Type == NodeType.PRODUCT).Random();
            viewmodel.TreeViewViewModel.OnNodeSelected(null, new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node }, new List<TreeViewNode>()));

            Assert.IsTrue(viewmodel.GetDataGridItems().All(x => x.Product.ID == node.ProductID));

            //다른 하나를 선택
            var node2 = viewmodel.TreeViewViewModel.Root.Where(x => x.Type == NodeType.PRODUCT).Random();
            viewmodel.TreeViewViewModel.OnNodeSelected(null, new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node2 }, new List<TreeViewNode>() { node }));

            Assert.IsTrue(viewmodel.GetDataGridItems().All(x => x.Product.ID == node2.ProductID));

            //2개를 선택
            viewmodel.TreeViewViewModel.OnNodeSelected(null, new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node }, new List<TreeViewNode>() { node }));
            Assert.IsTrue(viewmodel.GetDataGridItems().All(x => x.Product.ID == node2.ProductID || x.Product.ID == node.ProductID));
        }
    }
}