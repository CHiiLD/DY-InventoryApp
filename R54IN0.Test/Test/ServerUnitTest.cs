using System;
using MySql.Data.MySqlClient;
using R54IN0.Server;
using MySQL.Test;
using SuperSocket.SocketBase.Config;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace R54IN0.WPF.Test
{
    [TestFixture]
    public class ServerUnitTest
    {
        private static ReadOnlyServer _rserver;
        private static WriteOnlyServer _wserver;
        private static MySqlConnection _conn;

        [TestFixtureSetUp]
        public void TestSetUp()
        {
            MySqlConnection conn = new MySqlConnection(MySqlJsonFormat.ConnectionString("mysql_connection_string.json"));
            conn.Open();
            _conn = conn;

            Dummy dummy = new Dummy(conn);
            dummy.Create();

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
            _conn.Close();
        }

        [Test]
        public async Task WorkSetUp()
        {
            await Task.Delay(10);
        }

        [Test]
        public void TestFieldManagerAddAction()
        {
            string name = "soime";
            Maker maker = new Maker(name);
            FieldManagerViewModel vm = new FieldManagerViewModel();
            vm.AddField(maker);

            Task.Delay(500).Wait();

            Assert.IsTrue(vm.MakerList.Any(x => x.Name == maker.Name));
            Assert.IsTrue(vm.MakerList.Any(x => x.ID == maker.ID));
        }

        [Test]
        public void TestFieldManagerRemoveAction()
        {
            FieldManagerViewModel vm = new FieldManagerViewModel();
            var maker = vm.MakerList.Random();
            vm.RemoveField(maker);

            Task.Delay(100).Wait();

            vm.MakerList.Any(x => x.Name == maker.Name);
            vm.MakerList.Any(x => x.ID == maker.ID);
            Assert.IsFalse(vm.MakerList.Any(x => x.Name == maker.Name));
            Assert.IsFalse(vm.MakerList.Any(x => x.ID == maker.ID));
        }

        [Test]
        public void TestCreateNewProduct()
        {
            MultiSelectTreeViewModelView vm = new MultiSelectTreeViewModelView();
            vm.NewProductNodeAddCommand.Execute(null);

            Task.Delay(100).Wait();

            List<TreeViewNode> nodes = vm.SearchNodesInRoot(NodeType.PRODUCT);
            foreach (var node in nodes)
                Console.WriteLine(node.Name);
        }

        [Test]
        public void TestCreateNewInventoryFormat()
        {
            var product = DataDirector.GetInstance().CopyFields<Product>().Random();
            var viewmodel = new InventoryManagerViewModel(product);
            var name = viewmodel.Specification = "some specification";
            var memo = viewmodel.Memo = "spec";
            var maker = viewmodel.MakerText = "some maker";
            var measure = viewmodel.MeasureText = "some measure";

            string invID = viewmodel.Insert();
            Task.Delay(500).Wait();

            ObservableInventory inv = DataDirector.GetInstance().SearchInventory(invID);
            Assert.IsNotNull(invID);
            Assert.IsNotNull(DataDirector.GetInstance().SearchInventory(inv.ID));
            Assert.IsNotNull(DataDirector.GetInstance().SearchField<Maker>(inv.Maker.ID));
            Assert.IsNotNull(DataDirector.GetInstance().SearchField<Measure>(inv.Measure.ID));
        }

        /// <summary>
        /// 알 수 없는 에러 발생
        /// </summary>
        [Ignore]
        [Test]
        public void TestCreateNewStockFormat()
        {
            var prod = DataDirector.GetInstance().CopyFields<Product>().Random();
            var vm = new IOStockManagerViewModel(prod);

            var inv = vm.SelectedInventory = vm.Inventories.Random();

            vm.SelectedAccount = null;
            var acc = vm.AccountText = "new";
            var eep = vm.EmployeeText = "new";
            var prj = vm.ProjectText = "new";

            string id = vm.Insert();
            Task.Delay(100).Wait();

            Task<IOStockFormat> task = DataDirector.GetInstance().Db.SelectAsync<IOStockFormat>(id);
            task.Wait();

            IOStockFormat fmt = task.Result;
            ObservableIOStock oio = new ObservableIOStock(fmt);

            Thread.Sleep(100);

            Assert.AreEqual(inv, oio.Inventory);
            Assert.AreEqual(acc, oio.Supplier.Name);
            Assert.AreEqual(eep, oio.Employee.Name);
            Assert.AreEqual(prj, oio.Warehouse.Name);
        }

        [Test]
        public async Task ParallelConnect()
        {
            int i = 100;
            while (--i != 0)
            {
                await Task.Factory.StartNew(() =>
                {
                    MySqlBridge ms = new MySqlBridge();
                    var ar = ms.Connect();
                    ar.AsyncWaitHandle.WaitOne();
                });
            }
        }

        [Test]
        public async Task ParallelCommunication()
        {
            List<MySqlBridge> mss = new List<MySqlBridge>();
            int i = 100;
            while (--i != 0)
            {
                await Task.Factory.StartNew(() =>
                {
                    MySqlBridge ms = new MySqlBridge();
                    var ar = ms.Connect();
                    ar.AsyncWaitHandle.WaitOne();
                    mss.Add(ms);
                });
            }

            List<Task> tasks = new List<Task>();
            foreach (var ms in mss)
            {
                Task task = ms.SelectAsync<InventoryFormat>();
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        [Test]
        public async Task ParallelUpdate()
        {
            List<MySqlBridge> mss = new List<MySqlBridge>();
            int i = 100;
            while (--i != 0)
            {
                await Task.Factory.StartNew(() =>
                {
                    MySqlBridge ms = new MySqlBridge();
                    var ar = ms.Connect();
                    ar.AsyncWaitHandle.WaitOne();
                    mss.Add(ms);
                });
            }

            string id = null;
            using (MySqlCommand cmd = new MySqlCommand("select ID from Maker order by rand() limit 1;", _conn))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    id = reader.GetString(0);
                    break;
                }
            }
            Assert.IsNotNull(id);

            List<Task> tasks = new List<Task>();
            Maker maker = new Maker();
            maker.ID = id;
            foreach (var ms in mss)
            {
                maker.Name = Guid.NewGuid().ToString().Substring(0, 6);
                var task = Task.Factory.StartNew(() => { ms.Update<Maker>(maker); });
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());

            await Task.Delay(500);
        }
    }
}