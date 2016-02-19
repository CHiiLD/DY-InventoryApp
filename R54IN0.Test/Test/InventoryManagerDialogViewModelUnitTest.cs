using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using MySQL.Test;
using R54IN0.WPF;
using System;
using System.Linq;

namespace R54IN0.Test
{
    [TestClass]
    public class InventoryManagerDialogViewModelUnitTest
    {
        private static MySqlConnection _conn;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Console.WriteLine(nameof(ClassInitialize));
            Console.WriteLine(context.TestName);

            _conn = new MySqlConnection(ConnectingString.KEY);
            _conn.Open();

            Dummy dummy = new Dummy(_conn);
            dummy.Create();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Console.WriteLine(nameof(ClassCleanup));
            _conn.Close();
            _conn = null;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            MySqlConnection conn = DataDirector.GetInstance().DB.Connection;
            using (MySqlCommand cmd = new MySqlCommand("begin work;", conn))
                cmd.ExecuteNonQuery();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            MySqlConnection conn = DataDirector.GetInstance().DB.Connection;
            using (MySqlCommand cmd = new MySqlCommand("rollback;", conn))
                cmd.ExecuteNonQuery();

            CollectionViewModelObserverSubject.Destory();
            TreeViewNodeDirector.Destroy(true);
            DataDirector.Destroy();
        }

        [TestMethod]
        public void CanCreate()
        {
            var product = DataDirector.GetInstance().CopyFields<Product>().Random();
            new InventoryManagerViewModel(product);
        }

        [TestMethod]
        public void TestRegister()
        {
            var product = DataDirector.GetInstance().CopyFields<Product>().Random();
            var viewmodel = new InventoryManagerViewModel(product);
            var name = viewmodel.Specification = "some specification";
            var memo = viewmodel.Memo = "spec";
            var maker = viewmodel.Maker = viewmodel.MakerList.Random();
            var measure = viewmodel.Measure = viewmodel.MeasureList.Random();

            var inventory = viewmodel.Insert();

            Assert.IsNotNull(inventory);
            Assert.AreEqual(inventory.Product, product);
            Assert.AreEqual(inventory.Specification, name);
            Assert.AreEqual(inventory.Memo, memo);
            Assert.AreEqual(inventory.Maker, maker);
            Assert.AreEqual(inventory.Measure, measure);
        }

        [TestMethod]
        public void TestInsert2()
        {
            var product = DataDirector.GetInstance().CopyFields<Product>().Random();
            var viewmodel = new InventoryManagerViewModel(product);
            var name = viewmodel.Specification = "some specification";
            var memo = viewmodel.Memo = "spec";
            var maker = viewmodel.MakerText = "some maker";
            var measure = viewmodel.MeasureText = "some measure";

            var inventory = viewmodel.Insert();

            Assert.IsNotNull(inventory);
            Assert.AreEqual(inventory.Product, product);
            Assert.AreEqual(inventory.Specification, name);
            Assert.AreEqual(inventory.Memo, memo);
            Assert.AreEqual(inventory.Maker.Name, maker);
            Assert.AreEqual(inventory.Measure.Name, measure);
            Assert.IsNotNull(DataDirector.GetInstance().SearchField<Maker>(inventory.Maker.ID));
            Assert.IsNotNull(DataDirector.GetInstance().SearchField<Measure>(inventory.Measure.ID));
        }

        [TestMethod]
        public void WhenNewInventoryDataInsertThenSyncTreeView()
        {
            var inventoryStatusViewModel = new InventoryStatusViewModel();
            var node = inventoryStatusViewModel.TreeViewViewModel.SearchNodeInRoot(NodeType.PRODUCT).Random();
            inventoryStatusViewModel.TreeViewViewModel.AddSelectedNodes(node);

            var product = DataDirector.GetInstance().SearchField<Product>(node.ObservableObjectID);
            var viewmodel = new InventoryManagerViewModel(product);
            var name = viewmodel.Specification = "some specification";

            var inventory = viewmodel.Insert();

            Assert.IsNotNull(DataDirector.GetInstance().SearchInventory(inventory.ID));
            Assert.IsTrue(node.Root.Any(x => x.ObservableObjectID == inventory.ID));
        }

        [TestMethod]
        public void WhenNewInventoryDataInsertThenSyncDataGridViewMdoel()
        {
            var inventoryStatusViewModel = new InventoryStatusViewModel();
            var node = inventoryStatusViewModel.TreeViewViewModel.SearchNodeInRoot(NodeType.PRODUCT).Random();
            inventoryStatusViewModel.TreeViewViewModel.AddSelectedNodes(node);

            var product = DataDirector.GetInstance().SearchField<Product>(node.ObservableObjectID);
            var viewmodel = new InventoryManagerViewModel(product);
            var name = viewmodel.Specification = "some specification";

            var inventory = viewmodel.Insert();

            Assert.IsTrue(inventoryStatusViewModel.GetDataGridItems().Any(x => x.ID == inventory.ID));
        }

        [TestMethod]
        public void CanCreateForModify()
        {
            ObservableInventory inv = DataDirector.GetInstance().CopyInventories().Random();
            InventoryManagerViewModel vm = new InventoryManagerViewModel(inv);
        }

        [TestMethod]
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

        [TestMethod]
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

            inv = vm.Update();

            Assert.AreEqual(spec, inv.Specification);
            Assert.AreEqual(memo, inv.Memo);
            Assert.AreEqual(maker, inv.Maker.Name);
            Assert.AreEqual(measure, inv.Measure.Name);
        }

        [TestMethod]
        public void PropertyChange2()
        {
            ObservableInventory inv = DataDirector.GetInstance().CopyInventories().Random();
            InventoryManagerViewModel vm = new InventoryManagerViewModel(inv);

            string spec = vm.Specification = "some specification";
            string memo = vm.Memo = "memo";
            var maker = vm.Maker = vm.MakerList.Random();
            var measure = vm.Measure = vm.MeasureList.Random();

            inv = vm.Update();

            Assert.AreEqual(spec, inv.Specification);
            Assert.AreEqual(memo, inv.Memo);
            Assert.AreEqual(maker, inv.Maker);
            Assert.AreEqual(measure, inv.Measure);
        }
    }
}