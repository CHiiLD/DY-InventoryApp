using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;

namespace R54IN0.Test
{
    [TestClass]
    public class InoutStockStatusViewModelTest
    {
        [TestMethod]
        public void CanCreate()
        {
            new InoutStockStatusViewModel();
        }

        /// <summary>
        /// 뷰에서 날짜별로 그룹화를 선택해서 오늘날짜를 클릭한다.
        /// </summary>
        [TestMethod]
        public void SelectDatePicker()
        {
            new Dummy2().Create();
            var viewmodel = new InoutStockStatusViewModel();
            Assert.AreEqual(0, viewmodel.DataGridViewModel.Items.Count);
            //날짜를 선택
            viewmodel.SelectedGroupItem = InoutStockStatusViewModel.GROUPITEM_DATE;
            viewmodel.DatePickerViewModel.ThisWeekCommand.Execute(null); //올해 버튼을 클릭

            Assert.AreNotEqual(0, viewmodel.DataGridViewModel.Items.Count); //올해에 입력된 입출고 데이터를 데이터그리드에 추가
            foreach (var item in viewmodel.DataGridViewModel.Items)
            {
                Assert.IsTrue(new DateTime(DateTime.Now.Year, 1, 1).Ticks < item.Date.Ticks);
            }
        }

        /// <summary>
        /// 뷰에서 프로젝트별 그룹화를 선택한 후 프로젝트를 클릭한다.
        /// </summary>
        [TestMethod]
        public void SelectProject()
        {
            new Dummy2().Create();
            var viewmodel = new InoutStockStatusViewModel();
            Assert.AreEqual(0, viewmodel.DataGridViewModel.Items.Count);
            //프로젝트 선택
            viewmodel.SelectedGroupItem = InoutStockStatusViewModel.GROUPITEM_PROJECT;
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
            new Dummy2().Create();
            var viewmodel = new InoutStockStatusViewModel();
            Assert.AreEqual(0, viewmodel.DataGridViewModel.Items.Count);
            viewmodel.SelectedGroupItem = InoutStockStatusViewModel.GROUPITEM_PRODUCT;
            var node = viewmodel.TreeViewViewModel.Root.SelectMany(root => root.Descendants().Where(x => x.Type == NodeType.PRODUCT)).Random();
            viewmodel.TreeViewViewModel.OnNodeSelected(viewmodel.TreeViewViewModel,
                new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node }, new List<TreeViewNode>()));

            Assert.AreNotEqual(0, viewmodel.DataGridViewModel.Items.Count); 
            foreach (var item in viewmodel.DataGridViewModel.Items)
            {
                Assert.IsTrue(item.Inventory.Product.ID == node.ProductID);
            }
        }
    }
}