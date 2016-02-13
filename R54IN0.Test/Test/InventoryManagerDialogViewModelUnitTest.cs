using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System.Threading.Tasks;
using System.Linq;

namespace R54IN0.Test
{
    [TestClass]
    public class InventoryManagerDialogViewModelUnitTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var product = InventoryDataCommander.GetInstance().CopyFields<Product>().Random();
            new InventoryManagerViewModel(product);
        }

        [TestMethod]
        public void TestRegister()
        {
            new Dummy().Create();
            var product = InventoryDataCommander.GetInstance().CopyFields<Product>().Random();
            var viewmodel = new InventoryManagerViewModel(product);
            var name = viewmodel.Specification = "some specification";
            var memo = viewmodel.Memo = "spec";
            var maker = viewmodel.Maker = viewmodel.MakerList.Random();
            var measure = viewmodel.Measure = viewmodel.MeasureList.Random();

            var inventory = viewmodel.Register();

            Assert.IsNotNull(inventory);
            Assert.AreEqual(inventory.Product, product);
            Assert.AreEqual(inventory.Specification, name);
            Assert.AreEqual(inventory.Memo, memo);
            Assert.AreEqual(inventory.Maker, maker);
            Assert.AreEqual(inventory.Measure, measure);
        }

        [TestMethod]
        public void TestRegister2()
        {
            new Dummy().Create();
            var product = InventoryDataCommander.GetInstance().CopyFields<Product>().Random();
            var viewmodel = new InventoryManagerViewModel(product);
            var name = viewmodel.Specification = "some specification";
            var memo = viewmodel.Memo = "spec";
            var maker = viewmodel.MakerText = "some maker";
            var measure = viewmodel.MeasureText = "some measure";

            var inventory = viewmodel.Register();

            Assert.IsNotNull(inventory);
            Assert.AreEqual(inventory.Product, product);
            Assert.AreEqual(inventory.Specification, name);
            Assert.AreEqual(inventory.Memo, memo);
            Assert.AreEqual(inventory.Maker.Name, maker);
            Assert.AreEqual(inventory.Measure.Name, measure);
            Assert.IsNotNull(InventoryDataCommander.GetInstance().SearchField<Maker>(inventory.Maker.ID));
            Assert.IsNotNull(InventoryDataCommander.GetInstance().SearchField<Measure>(inventory.Measure.ID));
        }

        [TestMethod]
        public void SyncTreeView()
        {
            new Dummy().Create();
            var inventoryStatusViewModel = new InventoryStatusViewModel();
            var node = inventoryStatusViewModel.TreeViewViewModel.Root.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).Random();
            var product = InventoryDataCommander.GetInstance().SearchField<Product>(node.ObservableObjectID);
            var viewmodel = new InventoryManagerViewModel(product);
            var name = viewmodel.Specification = "some specification";

            var inventory = viewmodel.Register();

            Assert.IsNotNull(InventoryDataCommander.GetInstance().SearchInventory(inventory.ID));
            Assert.IsTrue(node.Root.Any(x => x.ObservableObjectID == inventory.ID));
        }

        [TestMethod]
        public void SyncInventoryStatusViewModelDataGrid()
        {
            new Dummy().Create();
            var inventoryStatusViewModel = new InventoryStatusViewModel();
            var node = inventoryStatusViewModel.TreeViewViewModel.Root.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).Random();
            var product = InventoryDataCommander.GetInstance().SearchField<Product>(node.ObservableObjectID);
            var viewmodel = new InventoryManagerViewModel(product);
            var name = viewmodel.Specification = "some specification";

            var inventory = viewmodel.Register();

            Assert.IsTrue(inventoryStatusViewModel.GetDataGridItems().Any(x => x.ID == inventory.ID));
        }
    }
}