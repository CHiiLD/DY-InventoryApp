using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using R54IN0;
using R54IN0.WPF;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MySQL.Test
{
    [TestClass]
    public class MySQLClientUnitTest
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

        [TestMethod]
        public void CanCreate()
        {
            new ClientAdapter();
            new ClientAdapter(_conn);
        }

        [TestMethod]
        public async Task TestSelect()
        {
            var ms = new ClientAdapter(_conn);
            List<Product> products = await ms.SelectAsync<Product>();
            products.ForEach(x => Console.WriteLine(x.Name));
        }

        [TestMethod]
        public async Task TestSelect2()
        {
            var ms = new ClientAdapter(_conn);
            List<Product> products = await ms.SelectAsync<Product>();
            Product product = await ms.SelectAsync<Product>(products.Random().ID);
            Console.WriteLine(product.Name);
        }

        [TestMethod]
        public async Task TestSelect3()
        {
            var ms = new ClientAdapter(_conn);
            List<InventoryFormat> invs = await ms.SelectAsync<InventoryFormat>();
            InventoryFormat inv = await ms.SelectAsync<InventoryFormat>(invs.Random().ID);
            Console.WriteLine(inv.Memo);
        }

        [TestMethod]
        public async Task TestInsert()
        {
            var ms = new ClientAdapter(_conn);
            var product = new Product() { Name = "new" };
            string id = ms.Insert<Product>(product);
            Console.WriteLine(id);

            product = await ms.SelectAsync<Product>(id);
            Assert.IsNotNull(product);
        }

        [TestMethod]
        public async Task TestUpdate0()
        {
            var ms = new ClientAdapter(_conn);
            List<Product> products = await ms.SelectAsync<Product>();
            Product product = products.Random();
            string name = "some";
            string id = ms.Update<Product>(product.ID, nameof(product.Name), name);
            Console.WriteLine(id);

            product = await ms.SelectAsync<Product>(id);
            Assert.AreEqual(product.Name, name);
        }

        [TestMethod]
        public async Task TestUpdate1()
        {
            var ms = new ClientAdapter(_conn);
            List<Product> products = await ms.SelectAsync<Product>();
            Product product = products.Random();
            string name = "some";
            string id = ms.Update<Product>(product.ID, new Dictionary<string, object>() { { nameof(product.Name), name } });
            Console.WriteLine(id);

            product = await ms.SelectAsync<Product>(id);
            Assert.AreEqual(product.Name, name);
        }

        [TestMethod]
        public async Task TestDelete0()
        {
            var ms = new ClientAdapter(_conn);
            List<Product> products = await ms.SelectAsync<Product>();
            Product product = products.Random();
            ms.Delete<Product>(product.ID);

            product = await ms.SelectAsync<Product>(product.ID);
            Assert.IsNull(product);
        }

        [TestMethod]
        public async Task TestDelete1()
        {
            var ms = new ClientAdapter(_conn);
            List<Employee> employees = await ms.SelectAsync<Employee>();
            Employee employee = employees.Random();
            ms.Delete<Employee>(employee.ID);

            employee = await ms.SelectAsync<Employee>(employee.ID);
            Assert.IsNull(employee);
        }

        [TestMethod]
        public async Task TestDelete2()
        {
            var ms = new ClientAdapter(_conn);
            List<Maker> makers = await ms.SelectAsync<Maker>();
            Maker maker = makers.Random();
            ms.Delete<Maker>(maker.ID);

            maker = await ms.SelectAsync<Maker>(maker.ID);
            Assert.IsNull(maker);
        }
    }
}