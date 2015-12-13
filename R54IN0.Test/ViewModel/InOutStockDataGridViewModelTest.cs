using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

namespace R54IN0.Test
{
    [TestClass]
    public class InOutStockDataGridViewModelTest
    {
        [TestMethod]
        public void CanCreateInOutStockDataGridViewModel()
        {
            new InOutStockDataGridViewModel(StockType.IN);
            new InOutStockDataGridViewModel(StockType.OUT);
        }

        [TestMethod]
        public void PrintInOutStock()
        {
            new DummyDbData().Create();
            var viewModel = new InOutStockDataGridViewModel(StockType.IN | StockType.OUT);
            foreach(var item in viewModel.Items)
                Console.WriteLine("{0}- {1}: {2}, EA: {3}", item.Inven.StockType, item.Item.Name, item.Specification.Name, item.ItemCount);
            Console.WriteLine("======");
            foreach (var item in viewModel.Items.Where(x => x.Inven.StockType == StockType.IN))
                Console.WriteLine("{0}- {1}: {2}, EA: {3}", item.Inven.StockType, item.Item.Name, item.Specification.Name, item.ItemCount);
            Console.WriteLine("======");
            foreach (var item in viewModel.Items.Where(x => x.Inven.StockType == StockType.OUT))
                Console.WriteLine("{0}- {1}: {2}, EA: {3}", item.Inven.StockType, item.Item.Name, item.Specification.Name, item.ItemCount);
        }

        [TestMethod]
        public void RemoveSelectedItemTest()
        {
            var viewModel = new InOutStockDataGridViewModel(StockType.IN | StockType.OUT);
            var item = viewModel.Items.FirstOrDefault();
            viewModel.SelectedItem = item;
            viewModel.RemoveSelectedItem();

            Assert.IsFalse(viewModel.Items.Any(x=> x == item));
        }

        [TestMethod]
        public void AddTest()
        {
            var viewModel = new InOutStockDataGridViewModel(StockType.IN | StockType.OUT);
            var item = viewModel.Items.FirstOrDefault();
            int count = viewModel.Items.Count;
            var newItem = new InOutStock(item.Inven);
            newItem.UUID = null;
            viewModel.AddNewItem(newItem);

            Assert.AreEqual(count + 1, viewModel.Items.Count);
            Assert.IsTrue(viewModel.Items.Any(x => x.Inven == newItem));
        }

        [TestMethod]
        public void ReplaceTest()
        {
            var viewModel = new InOutStockDataGridViewModel(StockType.IN | StockType.OUT);
            var item = viewModel.Items.FirstOrDefault();
            int count = viewModel.Items.Count;
            var newItem = new InOutStock(item.Inven);
            newItem.ItemCount = 12;
            viewModel.ReplaceItem(newItem);

            Assert.AreEqual(count, viewModel.Items.Count);
            Assert.IsTrue(viewModel.Items.Any(x => x.Inven == newItem));
            Assert.IsFalse(viewModel.Items.Any(x => x == item));
        }
    }
}