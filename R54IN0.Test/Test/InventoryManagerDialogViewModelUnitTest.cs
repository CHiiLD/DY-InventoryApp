using MySql.Data.MySqlClient;
using MySQL.Test;
using NUnit.Framework;
using R54IN0.Server;
using R54IN0.WPF;
using System;
using System.Linq;

namespace R54IN0.WPF.Test
{
    [TestFixture]
    public class InventoryManagerDialogViewModelUnitTest
    {
        MySqlConnection _conn;

        [TestFixtureSetUp]
        public void TestSetUp()
        {
            _conn = new MySqlConnection(MySqlJsonFormat.ConnectionString("mysql_connection_string.json"));
            _conn.Open();
            Dummy dummy = new Dummy(_conn);
            dummy.Create();
        }

        [TestFixtureTearDown]
        public void TestTearDown()
        {
            _conn.Close();
        }

        [SetUp]
        public void Setup()
        {
            IDbAction dbAction = new FakeDbAction(_conn);
            DataDirector.IntializeInstance(dbAction);
        }

        [TearDown]
        public void TearDown()
        {
            CollectionViewModelObserverSubject.Destory();
            TreeViewNodeDirector.Destroy(true);
            DataDirector.Destroy();
        }

        [Test]
        public void CanCreate()
        {
            var product = DataDirector.GetInstance().CopyFields<Product>().Random();
            new InventoryManagerViewModel(product);
        }

        [Test]
        public void TestRegister()
        {
            var product = DataDirector.GetInstance().CopyFields<Product>().Random();
            var viewmodel = new InventoryManagerViewModel(product);
            var name = viewmodel.Specification = "some specification";
            var memo = viewmodel.Memo = "spec";
            var maker = viewmodel.Maker = viewmodel.MakerList.Random();
            var measure = viewmodel.Measure = viewmodel.MeasureList.Random();

            var invID = viewmodel.Insert();
            var inventory = DataDirector.GetInstance().SearchInventory(invID);

            Assert.IsNotNull(inventory);
            Assert.AreEqual(inventory.Product, product);
            Assert.AreEqual(inventory.Specification, name);
            Assert.AreEqual(inventory.Memo, memo);
            Assert.AreEqual(inventory.Maker, maker);
            Assert.AreEqual(inventory.Measure, measure);
        }

        [Test]
        public void TestInsert2()
        {
            var product = DataDirector.GetInstance().CopyFields<Product>().Random();
            var viewmodel = new InventoryManagerViewModel(product);
            var name = viewmodel.Specification = "some specification";
            var memo = viewmodel.Memo = "spec";
            var maker = viewmodel.MakerText = "some maker";
            var measure = viewmodel.MeasureText = "some measure";

            var invID = viewmodel.Insert();
            var inventory = DataDirector.GetInstance().SearchInventory(invID);

            Assert.IsNotNull(inventory);
            Assert.AreEqual(inventory.Product, product);
            Assert.AreEqual(inventory.Specification, name);
            Assert.AreEqual(inventory.Memo, memo);
            Assert.AreEqual(inventory.Maker.Name, maker);
            Assert.AreEqual(inventory.Measure.Name, measure);
            Assert.IsNotNull(DataDirector.GetInstance().SearchField<Maker>(inventory.Maker.ID));
            Assert.IsNotNull(DataDirector.GetInstance().SearchField<Measure>(inventory.Measure.ID));
        }

        [Test]
        public void WhenNewInventoryDataInsertThenSyncTreeView()
        {
            var inventoryStatusViewModel = new InventoryStatusViewModel();
            var node = inventoryStatusViewModel.TreeViewViewModel.SearchNodesInRoot(NodeType.PRODUCT).Random();
            inventoryStatusViewModel.TreeViewViewModel.AddSelectedNodes(node);

            var product = DataDirector.GetInstance().SearchField<Product>(node.ObservableObjectID);
            var viewmodel = new InventoryManagerViewModel(product);
            var name = viewmodel.Specification = "some specification";

            var invID = viewmodel.Insert();
            var inventory = DataDirector.GetInstance().SearchInventory(invID);

            Assert.IsNotNull(DataDirector.GetInstance().SearchInventory(inventory.ID));
            Assert.IsTrue(node.Root.Any(x => x.ObservableObjectID == inventory.ID));
        }

        [Test]
        public void WhenNewInventoryDataInsertThenSyncDataGridViewMdoel()
        {
            var inventoryStatusViewModel = new InventoryStatusViewModel();
            var node = inventoryStatusViewModel.TreeViewViewModel.SearchNodesInRoot(NodeType.PRODUCT).Random();
            inventoryStatusViewModel.TreeViewViewModel.AddSelectedNodes(node);

            var product = DataDirector.GetInstance().SearchField<Product>(node.ObservableObjectID);
            var viewmodel = new InventoryManagerViewModel(product);
            var name = viewmodel.Specification = "some specification";

            var invID = viewmodel.Insert();
            var inventory = DataDirector.GetInstance().SearchInventory(invID);

            Assert.IsTrue(inventoryStatusViewModel.GetDataGridItems().Any(x => x.ID == inventory.ID));
        }

        [Test]
        public void CanCreateForModify()
        {
            ObservableInventory inv = DataDirector.GetInstance().CopyInventories().Random();
            InventoryManagerViewModel vm = new InventoryManagerViewModel(inv);
        }

        [Test]
        public void TestPropertyInitCheck()
        {
            ObservableInventory inv = DataDirector.GetInstance().CopyInventories().Random();
            InventoryManagerViewModel vm = new InventoryManagerViewModel(inv);

            Assert.AreEqual(inv.Product.Name, vm.ProductName);
            Assert.AreEqual(inv.Specification, vm.Specification);
            Assert.AreEqual(inv.Memo, vm.Memo);
            Assert.AreEqual(inv.Maker, vm.Maker);
            Assert.AreEqual(inv.Measure, vm.Measure);
        }

        [Test]
        public void PropertyChange()
        {
            ObservableInventory inv = DataDirector.GetInstance().CopyInventories().Random();
            InventoryManagerViewModel vm = new InventoryManagerViewModel(inv);

            string spec = vm.Specification = "some specification";
            string memo = vm.Memo = "memo";
            vm.Maker = null;
            string maker = vm.MakerText = "new";
            vm.Measure = null;
            string measure = vm.MeasureText = "new";

            string invID = vm.Update();
            inv = DataDirector.GetInstance().SearchInventory(invID);

            Assert.AreEqual(spec, inv.Specification);
            Assert.AreEqual(memo, inv.Memo);
            Assert.AreEqual(maker, inv.Maker.Name);
            Assert.AreEqual(measure, inv.Measure.Name);
        }

        [Test]
        public void PropertyChange2()
        {
            ObservableInventory inv = DataDirector.GetInstance().CopyInventories().Random();
            InventoryManagerViewModel vm = new InventoryManagerViewModel(inv);

            string spec = vm.Specification = "some specification";
            string memo = vm.Memo = "memo";
            var maker = vm.Maker = vm.MakerList.Random();
            var measure = vm.Measure = vm.MeasureList.Random();

            string invID = vm.Update();
            inv = DataDirector.GetInstance().SearchInventory(invID);

            Assert.AreEqual(spec, inv.Specification);
            Assert.AreEqual(memo, inv.Memo);
            Assert.AreEqual(maker, inv.Maker);
            Assert.AreEqual(measure, inv.Measure);
        }
    }
}