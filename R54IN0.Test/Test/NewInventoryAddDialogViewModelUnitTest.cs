using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System.Threading.Tasks;
using System.Linq;

namespace R54IN0.Test
{
    [TestClass]
    public class NewInventoryAddDialogViewModelUnitTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var product = InventoryDataCommander.GetInstance().CopyObservableFields<Product>().Random();
            new NewInventoryAddDialogViewModel(product);
        }

        [TestMethod]
        public async Task TestRegister()
        {
            new Dummy().Create();
            var product = InventoryDataCommander.GetInstance().CopyObservableFields<Product>().Random();
            var viewmodel = new NewInventoryAddDialogViewModel(product);
            var name = viewmodel.Specification = "some specification";
            var memo = viewmodel.Memo = "spec";
            var maker = viewmodel.Maker = viewmodel.MakerList.Random();
            var measure = viewmodel.Measure = viewmodel.MeasureList.Random();

            var inventory = await viewmodel.Register();

            Assert.IsNotNull(inventory);
            Assert.AreEqual(inventory.Product, product);
            Assert.AreEqual(inventory.Specification, name);
            Assert.AreEqual(inventory.Memo, memo);
            Assert.AreEqual(inventory.Maker, maker);
            Assert.AreEqual(inventory.Measure, measure);
        }

        [TestMethod]
        public async Task TestRegister2()
        {
            new Dummy().Create();
            var product = InventoryDataCommander.GetInstance().CopyObservableFields<Product>().Random();
            var viewmodel = new NewInventoryAddDialogViewModel(product);
            var name = viewmodel.Specification = "some specification";
            var memo = viewmodel.Memo = "spec";
            var maker = viewmodel.MakerText = "some maker";
            var measure = viewmodel.MeasureText = "some measure";

            var inventory = await viewmodel.Register();

            Assert.IsNotNull(inventory);
            Assert.AreEqual(inventory.Product, product);
            Assert.AreEqual(inventory.Specification, name);
            Assert.AreEqual(inventory.Memo, memo);
            Assert.AreEqual(inventory.Maker.Name, maker);
            Assert.AreEqual(inventory.Measure.Name, measure);
            Assert.IsNotNull(InventoryDataCommander.GetInstance().SearchObservableField<Maker>(inventory.Maker.ID));
            Assert.IsNotNull(InventoryDataCommander.GetInstance().SearchObservableField<Measure>(inventory.Measure.ID));
        }

        [TestMethod]
        public async Task SyncTreeView()
        {
            new Dummy().Create();
            var inventoryStatusViewModel = new InventoryStatusViewModel();
            var node = inventoryStatusViewModel.TreeViewViewModel.Root.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).Random();
            var product = InventoryDataCommander.GetInstance().SearchObservableField<Product>(node.ObservableObjectID);
            var viewmodel = new NewInventoryAddDialogViewModel(product);
            var name = viewmodel.Specification = "some specification";

            var inventory = await viewmodel.Register();

            Assert.IsNotNull(InventoryDataCommander.GetInstance().SearchObservableInventory(inventory.ID));
            Assert.IsTrue(node.Root.Any(x => x.ObservableObjectID == inventory.ID));
        }

        [TestMethod]
        public async Task SyncInventoryStatusViewModelDataGrid()
        {
            new Dummy().Create();
            var inventoryStatusViewModel = new InventoryStatusViewModel();
            var node = inventoryStatusViewModel.TreeViewViewModel.Root.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).Random();
            var product = InventoryDataCommander.GetInstance().SearchObservableField<Product>(node.ObservableObjectID);
            var viewmodel = new NewInventoryAddDialogViewModel(product);
            var name = viewmodel.Specification = "some specification";

            var inventory = await viewmodel.Register();

            Assert.IsTrue(inventoryStatusViewModel.GetDataGridItems().Any(x => x.ID == inventory.ID));
        }
    }
}