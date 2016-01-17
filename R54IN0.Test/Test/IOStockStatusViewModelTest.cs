﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace R54IN0.Test
{
    [TestClass]
    public class IOStockStatusViewModelTest
    {
        [TestMethod]
        public async Task CanCreate()
        {
            await new Dummy().Create();
            new IOStockStatusViewModel();
        }

        /// <summary>
        /// 뷰에서 날짜별로 그룹화를 선택해서 오늘날짜를 클릭한다.
        /// </summary>
        [TestMethod]
        public async Task SelectDatePicker()
        {
            await new Dummy().Create();
            var viewmodel = new IOStockStatusViewModel();
            Assert.AreEqual(0, viewmodel.DataGridViewModel.Items.Count);
            //날짜를 선택
            viewmodel.SelectedGroupItem = IOStockStatusViewModel.GROUPITEM_DATE;
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
        public async Task SelectProject()
        {
            await new Dummy().Create();
            var viewmodel = new IOStockStatusViewModel();
            Assert.AreEqual(0, viewmodel.DataGridViewModel.Items.Count);
            //프로젝트 선택
            viewmodel.SelectedGroupItem = IOStockStatusViewModel.GROUPITEM_PROJECT;
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
        public async Task SelectProduct()
        {
            await new Dummy().Create();
            var viewmodel = new IOStockStatusViewModel();
            Assert.AreEqual(0, viewmodel.DataGridViewModel.Items.Count);
            viewmodel.SelectedGroupItem = IOStockStatusViewModel.GROUPITEM_PRODUCT;
            var node = viewmodel.TreeViewViewModel.Root.SelectMany(root => root.Descendants().Where(x => x.Type == NodeType.PRODUCT)).Random();
            viewmodel.TreeViewViewModel.ExecuteNodesSelectedEventCommand(
                new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node }, new List<TreeViewNode>()));
            viewmodel.OnTreeViewNodesSelected(viewmodel.TreeViewViewModel, new PropertyChangedEventArgs("SelectedNodes"));
            Assert.AreNotEqual(0, viewmodel.DataGridViewModel.Items.Count);
            foreach (var item in viewmodel.DataGridViewModel.Items)
            {
                Assert.IsTrue(item.Inventory.Product.ID == node.ProductID);
            }
        }

        /// <summary>
        /// 입고와 출고 체크박스를 선택하였을 때 그에 맞는 데이터가 데이터 그리드에 업데이트 되어야 한다.
        /// </summary>
        [TestMethod]
        public async Task ControlInoutStockCheckBox()
        {
            await new Dummy().Create();
            var viewmodel = new IOStockStatusViewModel();
            viewmodel.IsCheckedInComing = true;
            viewmodel.IsCheckedOutGoing = true;
            //제품 하나 선택
            viewmodel.SelectedGroupItem = IOStockStatusViewModel.GROUPITEM_PRODUCT;
            var node = viewmodel.TreeViewViewModel.Root.SelectMany(root => root.Descendants().Where(x => x.Type == NodeType.PRODUCT)).Random();
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
        public async Task CheckDataGridRowCell()
        {
            var viewmodel = await CreateViewModelThenSelectedTreeViewNodeRandomly();
            var item = viewmodel.DataGridViewModel.Items.Random();
            item.IsChecked = true;
        }

        /// <summary>
        /// 하나의 데이터그리드 아이템을 체크
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CopyCheckedDataGridRowCell()
        {
            var viewmodel = await CreateViewModelThenSelectedTreeViewNodeRandomly();
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
        public async Task CopyCheckedDataGridRowCells()
        {
            var viewmodel = await CreateViewModelThenSelectedTreeViewNodeRandomly();
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
        public async Task CopyRowCellThenCheckThatQuantityHaveToCalc()
        {
            var viewmodel = await CreateViewModelThenSelectedTreeViewNodeRandomly();
            int itemsCount = viewmodel.DataGridViewModel.Items.Count();
            var item = viewmodel.DataGridViewModel.Items.Random();
            item.IsChecked = true;

            Console.WriteLine("선택된 아이템의 IOSQty: {0}, RemainQty: {1}, InvQty: {2}",item.Quantity, item.RemainingQuantity, item.Inventory.Quantity);
            viewmodel.DataGridViewModel.CheckedIOStockFormatsCopyCommand.Execute(null);

            AssertQuantityChecking(viewmodel.DataGridViewModel.Items);
        }

        /// <summary>
        /// 데이터그리드를 복사하여 여러 데이터를 추가한 뒤 Quantity가 제대로 계산되었는지 확인한다.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CopyRowCellThenCheckThatQuantityHaveToCalc2()
        {
            var viewmodel = await CreateViewModelThenSelectedTreeViewNodeRandomly();
            int itemsCount = viewmodel.DataGridViewModel.Items.Count();
            for (int i = 0; i < 10; i++)
            {
                int idx = new Random().Next(0, itemsCount);
                var item = viewmodel.DataGridViewModel.Items.ElementAt(idx);
                if(item.IsChecked != true)
                    item.IsChecked = true;
            }

            viewmodel.DataGridViewModel.CheckedIOStockFormatsCopyCommand.Execute(null);

            AssertQuantityChecking(viewmodel.DataGridViewModel.Items);
        }

        /// <summary>
        /// 데이터그리드의 아이템을 하나 삭제하고 데이터그리드를 최신화 시킨다.
        /// </summary>
        [TestMethod]
        public async Task DeleteItemThenSyncDataGridItems()
        {
            var viewmodel = await CreateViewModelThenSelectedTreeViewNodeRandomly();
            var item = viewmodel.DataGridViewModel.SelectedItem = viewmodel.DataGridViewModel.Items.Random();

            viewmodel.DataGridViewModel.IOStockFormatDeletionCommand.Execute(null);

            Assert.IsTrue(!viewmodel.DataGridViewModel.Items.Contains(item));
        }

        /// <summary>
        /// 삭제 후 데이터베이스에서 관련 자료가 모두 삭제되어야 한다.
        /// </summary>
        [TestMethod]
        public async Task DeleteItemThenSyncDb()
        {
            var viewmodel = await CreateViewModelThenSelectedTreeViewNodeRandomly();
            var item = viewmodel.DataGridViewModel.SelectedItem = viewmodel.DataGridViewModel.Items.Random();
            string iosID = item.ID;

            viewmodel.DataGridViewModel.IOStockFormatDeletionCommand.Execute(null);

            var iofmts = await DbAdapter.GetInstance().SelectAsync<IOStockFormat>(iosID);
            Assert.IsNull(iofmts);
        }

        /// <summary>
        /// 삭제 후 잔여수량과 재고수량을 업데이트 한다.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DeleteItemThenSyncQty()
        {
            var viewmodel = await CreateViewModelThenSelectedTreeViewNodeRandomly();
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
        public async Task DeleteItemThenSyncInventoryQty()
        {
            var viewmodel = await CreateViewModelThenSelectedTreeViewNodeRandomly();
            var item = viewmodel.DataGridViewModel.SelectedItem = viewmodel.DataGridViewModel.Items.Random();
            int inQty = item.Inventory.Quantity;
            string iosID = item.ID;

            viewmodel.DataGridViewModel.IOStockFormatDeletionCommand.Execute(null);
            int inQty2 = ObservableInventoryDirector.GetInstance().Search(item.Inventory.ID).Quantity;

            Assert.AreNotEqual(inQty, inQty2);
        }
        
        [TestMethod]
        public async Task DeleteCheckedItemThenSyncDataGridItems()
        {
            var viewmodel = await CreateViewModelThenSelectedTreeViewNodeRandomly();
            var item = viewmodel.DataGridViewModel.SelectedItem = viewmodel.DataGridViewModel.Items.Random();
            item.IsChecked = true;

            viewmodel.DataGridViewModel.ChekcedIOStockFormatsDeletionCommand.Execute(null);

            Assert.IsTrue(!viewmodel.DataGridViewModel.Items.Contains(item));
        }


        /// <summary>
        /// 삭제 후 데이터베이스에서 관련 자료가 모두 삭제되어야 한다.
        /// </summary>
        [TestMethod]
        public async Task DeleteCheckedItemThenSyncDb()
        {
            var viewmodel = await CreateViewModelThenSelectedTreeViewNodeRandomly();
            var item = viewmodel.DataGridViewModel.SelectedItem = viewmodel.DataGridViewModel.Items.Random();
            item.IsChecked = true;
            string iosID = item.ID;

            viewmodel.DataGridViewModel.ChekcedIOStockFormatsDeletionCommand.Execute(null);

            var iofmts = await DbAdapter.GetInstance().SelectAsync<IOStockFormat>(iosID);
            Assert.IsNull(iofmts);
        }

        /// <summary>
        /// 삭제 후 잔여수량과 재고수량을 업데이트 한다.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DeleteCheckedItemThenSyncQty()
        {
            var viewmodel = await CreateViewModelThenSelectedTreeViewNodeRandomly();
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
        public async Task DeleteCheckedItemThenSyncInventoryQty()
        {
            var viewmodel = await CreateViewModelThenSelectedTreeViewNodeRandomly();
            var item = viewmodel.DataGridViewModel.SelectedItem = viewmodel.DataGridViewModel.Items.Random();
            item.IsChecked = true;
            int inQty = item.Inventory.Quantity;
            string iosID = item.ID;

            viewmodel.DataGridViewModel.ChekcedIOStockFormatsDeletionCommand.Execute(null);
            int inQty2 = ObservableInventoryDirector.GetInstance().Search(item.Inventory.ID).Quantity;

            Assert.AreNotEqual(inQty, inQty2);
        }


        [TestMethod]
        public async Task DeleteCheckedItemsThenSyncDataGridItems()
        {
            var viewmodel = await CreateViewModelThenSelectedTreeViewNodeRandomly();
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
        public async Task DeleteCheckedItemsThenSyncDb()
        {
            var viewmodel = await CreateViewModelThenSelectedTreeViewNodeRandomly();
            for (int i = 0; i < 10; i++)
            {
                int idx = new Random().Next(0, viewmodel.DataGridViewModel.Items.Count());
                var item = viewmodel.DataGridViewModel.Items.ElementAt(idx);
                if (item.IsChecked != true)
                    item.IsChecked = true;
            }
            var checkedItems = viewmodel.DataGridViewModel.Items.Where(x => x.IsChecked == true).ToList();

            viewmodel.DataGridViewModel.ChekcedIOStockFormatsDeletionCommand.Execute(null);

            foreach(var item in checkedItems)
            {
                var iofmts = await DbAdapter.GetInstance().SelectAsync<IOStockFormat>(item.ID);
                Assert.IsNull(iofmts);
            }
        }

        [TestMethod]
        public async Task DeleteCheckedItemsThenSyncQty()
        {
            var viewmodel = await CreateViewModelThenSelectedTreeViewNodeRandomly();
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

        public void AssertQuantityChecking(IEnumerable<IOStockDataGridItem> items)
        {
            var lookup = items.ToLookup(x => x.Inventory);
            foreach(var item in lookup)
            {
                var orderedItem = item.OrderBy(x => x.Date);
                var lastItem = orderedItem.Last();
                Assert.AreEqual(lastItem.RemainingQuantity, item.Key.Quantity);
                IOStockFormat near = null;
                int remainQty, iosQty, exp;
                foreach (var i in orderedItem)
                {
                    remainQty = 0;
                    iosQty = i.Quantity;
                    if (i.StockType == IOStockType.OUTGOING)
                        iosQty = -iosQty;
                    if (near != null)
                        remainQty = near.RemainingQuantity;
                    exp = remainQty + iosQty;
                    Assert.AreEqual(i.RemainingQuantity, exp);
                    near = i.Format;
                }
            }
        }
        public async Task<IOStockStatusViewModel> CreateViewModelThenSelectedTreeViewNodeRandomly()
        {
            await new Dummy().Create();
            var viewmodel = new IOStockStatusViewModel();
            viewmodel.SelectedGroupItem = IOStockStatusViewModel.GROUPITEM_PRODUCT;
            TreeViewNode node = viewmodel.TreeViewViewModel.Root.SelectMany(root => root.Descendants().Where(x => x.Type == NodeType.PRODUCT)).Random();
            viewmodel.TreeViewViewModel.ExecuteNodesSelectedEventCommand(new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node }, null));
            return viewmodel;
        }
    }
}