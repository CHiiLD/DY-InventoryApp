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
    }
}