using MySql.Data.MySqlClient;
using R54IN0;
using R54IN0.Server;
using R54IN0.WPF;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MySQL.Test
{
    [TestFixture]
    public class MySQLAdapterUnitTest
    {
        public ReadOnlyServer _readServer;
        public WriteOnlyServer _writeServer;

        [TestFixtureSetUp]
        public void ClassInitialize()
        {
            using (MySqlConnection conn = new MySqlConnection(MySqlJsonFormat.ConnectionString("mysql_connection_string.json")))
            {
                conn.Open();
                Dummy dummy = new Dummy(conn);
                dummy.Create();
            }

            string json = System.IO.File.ReadAllText("ipconfig.json");
            IPConfigJsonFormat config = JsonConvert.DeserializeObject<IPConfigJsonFormat>(json);
            _readServer = new ReadOnlyServer();
            _readServer.Setup(config.ReadServerHost, config.ReadServerPort);
            _writeServer = new WriteOnlyServer();
            _writeServer.Setup(config.WriteServerHost, config.WriteServerPort);
        }

        [SetUp]
        public void Setup()
        {
            _readServer.Start();
            _writeServer.Start();
        }

        [TearDown]
        public void Clean()
        {
            _readServer.Stop();
            _writeServer.Stop();
        }

        [Test]
        public void CanCreate()
        {
            new MySqlBridge();
        }

        [Test]
        public void CanConnect()
        {
            using (MySqlBridge ms = new MySqlBridge())
            {
                ms.Connect();
                Thread.Sleep(10);
                Assert.IsTrue(ms.Socket.Connected);
                Assert.IsTrue(ms.Session.IsConnected);
            }
        }

        [Test]
        public async Task TestSelect()
        {
            using (var ms = new MySqlBridge())
            {
                ms.Connect();
                await Task.Delay(10);
                List<Product> products = await ms.SelectAsync<Product>();
                products.ForEach(x => Console.WriteLine(x.Name));
            }
        }

        [Test]
        public async Task TestSelect2()
        {
            using (var ms = new MySqlBridge())
            {
                ms.Connect();
                await Task.Delay(10);
                List<Product> products = await ms.SelectAsync<Product>();
                Product product = await ms.SelectAsync<Product>(products.Random().ID);
                Console.WriteLine(product.Name);
            }
        }

        [Test]
        public async Task TestSelect3()
        {
            using (var ms = new MySqlBridge())
            {
                ms.Connect();
                await Task.Delay(10);
                List<InventoryFormat> invs = await ms.SelectAsync<InventoryFormat>();
                InventoryFormat inv = await ms.SelectAsync<InventoryFormat>(invs.Random().ID);
                Console.WriteLine(inv.Memo);
            }
        }

        [Test]
        public async Task TestInsert()
        {
            using (var ms = new MySqlBridge())
            {
                ms.Connect();
                await Task.Delay(10);
                var product = new Product() { Name = "new" };
                ms.Insert<Product>(product);

                await Task.Delay(10);

                Console.WriteLine(product.ID);
                product = await ms.SelectAsync<Product>(product.ID);
                Assert.IsNotNull(product);
            }
        }

        [Test]
        public async Task TestUpdate0()
        {
            using (var ms = new MySqlBridge())
            {
                ms.Connect();
                await Task.Delay(10);
                List<Product> products = await ms.SelectAsync<Product>();
                Product product = products.Random();
                string name = "some";
                product.Name = name;
                ms.Update<Product>(product);
                await Task.Delay(10);
                product = await ms.SelectAsync<Product>(product.ID);
                Assert.AreEqual(product.Name, name);
            }
        }

        [Test]
        public async Task TestDelete0()
        {
            using (var ms = new MySqlBridge())
            {
                ms.Connect();
                await Task.Delay(10);

                List<Product> products = await ms.SelectAsync<Product>();
                Product product = products.Random();
                ms.Delete<Product>(product.ID);

                await Task.Delay(10);
                product = await ms.SelectAsync<Product>(product.ID);
                Assert.IsNull(product);
            }
        }

        [Test]
        public async Task TestDelete1()
        {
            using (var ms = new MySqlBridge())
            {
                ms.Connect();
                await Task.Delay(10);
                List<Employee> employees = await ms.SelectAsync<Employee>();
                Employee employee = employees.Random();
                ms.Delete<Employee>(employee.ID);
                await Task.Delay(10);
                employee = await ms.SelectAsync<Employee>(employee.ID);
                Assert.IsNull(employee);
            }
        }

        [Test]
        public async Task TestDelete2()
        {
            using (var ms = new MySqlBridge())
            {
                ms.Connect();
                await Task.Delay(10);
                List<Maker> makers = await ms.SelectAsync<Maker>();
                Maker maker = makers.Random();
                ms.Delete<Maker>(maker.ID);
                await Task.Delay(10);
                maker = await ms.SelectAsync<Maker>(maker.ID);
                Assert.IsNull(maker);
            }
        }
    }
}