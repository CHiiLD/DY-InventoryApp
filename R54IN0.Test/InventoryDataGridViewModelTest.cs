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
        public void DummyTest()
        {
            new DummyDbData().Create();
            var items = DatabaseDirector.GetDbInstance().LoadAll<Item>();
            Assert.AreEqual(5, items.Length);
        }
    }
}