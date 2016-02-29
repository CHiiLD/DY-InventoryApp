using System;
using MySql.Data.MySqlClient;
//using NUnit.Framework;
using R54IN0.Server;
using MySQL.Test;
using SuperSocket.SocketBase.Config;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using NUnit.Framework;

namespace R54IN0.WPF.Test
{
    [TestFixture]
    public class WriteServerUnitTest
    {
        private static ReadOnlyServer _rserver;
        private static WriteOnlyServer _wserver;

        [TestFixtureSetUp]
        public void TestSetUp()
        {
            using (MySqlConnection conn = new MySqlConnection(MySqlJsonFormat.ConnectionString("mysql_connection_string.json")))
            {
                conn.Open();
                Dummy dummy = new Dummy(conn);
                dummy.Create();
            }

            _rserver = new ReadOnlyServer();
            _rserver.Setup(new ServerConfig()
            {
                Ip = "Any",
                Port = 4000,
                DisableSessionSnapshot = true,
            });

            _wserver = new WriteOnlyServer();
            _wserver.Setup(new ServerConfig()
            {
                Ip = "Any",
                Port = 4001,
                DisableSessionSnapshot = true,
            });
        }

        
        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Setup");
            _rserver.Start();
            _wserver.Start();
            DataDirector.InitialzeInstanceAsync().Wait();
        }

        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("TearDown");
            CollectionViewModelObserverSubject.Destory();
            TreeViewNodeDirector.Destroy(true);
            DataDirector.Destroy();
            _rserver.Stop();
            _wserver.Stop();
        }

        [Test]
        public async Task WorkSetUp()
        {
            await Task.Delay(10);
        }

        [Test]
        public async Task TestFieldManagerAddAction()
        {
            string name = "soime";
            Maker maker = new Maker(name);
            FieldManagerViewModel vm = new FieldManagerViewModel();
            vm.AddField(maker);

            await Task.Delay(500);

            Assert.IsTrue(vm.MakerList.Any(x => x.Name == maker.Name));
            Assert.IsTrue(vm.MakerList.Any(x => x.ID == maker.ID));
        }

        [Test]
        public async Task TestFieldManagerRemoveAction()
        {
            FieldManagerViewModel vm = new FieldManagerViewModel();
            var maker = vm.MakerList.Random();
            vm.RemoveField(maker);

            await Task.Delay(100);

            vm.MakerList.Any(x => x.Name == maker.Name);
            vm.MakerList.Any(x => x.ID == maker.ID);
            Assert.IsFalse(vm.MakerList.Any(x => x.Name == maker.Name));
            Assert.IsFalse(vm.MakerList.Any(x => x.ID == maker.ID));
        }

        [Test]
        public async Task TestCreateNewProduct()
        {
            MultiSelectTreeViewModelView vm = new MultiSelectTreeViewModelView();
            vm.NewProductNodeAddCommand.Execute(null);
            await Task.Delay(100);
            List<TreeViewNode> nodes = vm.SearchNodesInRoot(NodeType.PRODUCT);
            foreach (var node in nodes)
                Console.WriteLine(node.Name);
        }

        [Test]
        public async Task TestCreateNewInventoryFormat()
        {
            var product = DataDirector.GetInstance().CopyFields<Product>().Random();
            var viewmodel = new InventoryManagerViewModel(product);
            var name = viewmodel.Specification = "some specification";
            var memo = viewmodel.Memo = "spec";
            var maker = viewmodel.MakerText = "some maker";
            var measure = viewmodel.MeasureText = "some measure";

            string invID = viewmodel.Insert();
            await Task.Delay(100);

            ObservableInventory inv = DataDirector.GetInstance().SearchInventory(invID);
            Assert.IsNotNull(invID);
            Assert.IsNotNull(DataDirector.GetInstance().SearchInventory(inv.ID));
            Assert.IsNotNull(DataDirector.GetInstance().SearchField<Maker>(inv.Maker.ID));
            Assert.IsNotNull(DataDirector.GetInstance().SearchField<Measure>(inv.Measure.ID));
        }

        [Ignore]
        [Test, RequiresSTA]
        public void TestUpdateInvQty()
        {
            Task work = Task.Factory.StartNew(new Action(async () =>
            {
                IOStockStatusViewModel svm = new IOStockStatusViewModel();
                svm.SelectedDataGridGroupOption = IOStockStatusViewModel.DATAGRID_OPTION_PRODUCT;
                TreeViewNode node = svm.TreeViewViewModel.SearchNodesInRoot(NodeType.INVENTORY).Random();
                svm.TreeViewViewModel.AddSelectedNodes(node);
                await Task.Delay(100);
                IOStockDataGridItem item = svm.DataGridViewModel.Items.Random();
                ObservableInventory inv = DataDirector.GetInstance().SearchInventory(item.InventoryID);
                int qty = inv.Quantity;
                IOStockManagerViewModel mvm = new IOStockManagerViewModel(svm, item);
                mvm.Quantity = mvm.Quantity * 2;
                mvm.Update();
                await Task.Delay(100);
                Assert.AreNotEqual(qty, inv.Quantity);
            }));
            work.Wait();
        }

        [Ignore]
        [Test]
        public async Task TestCreateNewStockFormat()
        {
            Task work = Task.Factory.StartNew(new Action(async () =>
            {
                var prod = DataDirector.GetInstance().CopyFields<Product>().Random();
                var vm = new IOStockManagerViewModel(prod);

                var inv = vm.SelectedInventory = vm.Inventories.Random();

                vm.SelectedAccount = null;
                var acc = vm.AccountText = "new";
                var eep = vm.EmployeeText = "new";
                var prj = vm.ProjectText = "new";

                string id = vm.Insert();
                IOStockFormat fmt = await DataDirector.GetInstance().Db.SelectAsync<IOStockFormat>(id);
                ObservableIOStock oio = new ObservableIOStock(fmt);

                await Task.Delay(100);

                Assert.AreEqual(inv, oio.Inventory);
                Assert.AreEqual(acc, oio.Supplier.Name);
                Assert.AreEqual(eep, oio.Employee.Name);
                Assert.AreEqual(prj, oio.Warehouse.Name);
            }));
            await work;
        }
    }
}
