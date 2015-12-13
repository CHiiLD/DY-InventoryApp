using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace R54IN0.Test
{
    [TestClass]
    public class InventoryDataGridViewModelTest
    {
        [TestMethod]
        public void CanCreateInventoryDataGridViewModel()
        {
            InventoryDataGridViewModel viewModel = new InventoryDataGridViewModel();
        }

        [TestMethod]
        public void RemoveSelectedItemTest()
        {
            InventoryDataGridViewModel viewModel = new InventoryDataGridViewModel();
            if (viewModel.Items.Count != 0)
            {
                int cnt = viewModel.Items.Count;
                viewModel.RemoveSelectedItem();
                Assert.AreEqual(cnt - 1, viewModel.Items.Count);
            }
        }
    }
}