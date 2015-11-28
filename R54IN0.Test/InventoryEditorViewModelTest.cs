using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace R54IN0.Test
{
    [TestClass]
    public class InventoryEditorViewModelTest
    {
        [TestMethod]
        public void CanCreateInventoryEditorViewModel()
        {
            new InventoryEditorViewModel();
        }

        [TestMethod]
        public void CanCopyInventoryEditorViewModel()
        {
            new DummyDbData().Create();
            using (var db = DatabaseDirector.GetDbInstance())
            {
                var invens = db.LoadAll<Inventory>();
                new InventoryEditorViewModel(new InventoryPipe(invens.FirstOrDefault()));
            }
        }

        [TestMethod]
        public void AddTest()
        {
            new DummyDbData().Create();
            var viewModel = new InventoryEditorViewModel();
            viewModel.SelectedItem = viewModel.AllItem.FirstOrDefault();
            viewModel.SelectedSpecification = viewModel.AllSpecification.FirstOrDefault();
            viewModel.ItemCount = 10;
            viewModel.SelectedWarehouse = viewModel.AllWarehouse.FirstOrDefault();

            var dgViewModel = new InventoryDataGridViewModel();
            int count = dgViewModel.Items.Count;
            dgViewModel.Add(viewModel.InventoryPipe);

            Assert.AreEqual(count + 1, dgViewModel.Items.Count);

            dgViewModel = new InventoryDataGridViewModel();
            Assert.AreEqual(count + 1, dgViewModel.Items.Count);
        }
    }
}
