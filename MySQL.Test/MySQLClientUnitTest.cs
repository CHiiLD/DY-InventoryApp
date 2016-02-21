﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using R54IN0;
using R54IN0.WPF;
using System;
using System.Collections.Generic;

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
            new MySQLClient();
            new MySQLClient(_conn);
        }

        [TestMethod]
        public void TestSelect()
        {
            var ms = new MySQLClient(_conn);
            List<Product> products = ms.Select<Product>();
            products.ForEach(x => Console.WriteLine(x.Name));
        }

        [TestMethod]
        public void TestSelect2()
        {
            var ms = new MySQLClient(_conn);
            List<Product> products = ms.Select<Product>();
            Product product = ms.Select<Product>(products.Random().ID);
            Console.WriteLine(product.Name);
        }

        [TestMethod]
        public void TestSelect3()
        {
            var ms = new MySQLClient(_conn);
            List<InventoryFormat> invs = ms.Select<InventoryFormat>();
            InventoryFormat inv = ms.Select<InventoryFormat>(invs.Random().ID);
            Console.WriteLine(inv.Memo);
        }

        [TestMethod]
        public void TestInsert()
        {
            var ms = new MySQLClient(_conn);
            var product = new Product() { Name = "new" };
            string id = ms.Insert<Product>(product);
            Console.WriteLine(id);

            product = ms.Select<Product>(id);
            Assert.IsNotNull(product);
        }

        [TestMethod]
        public void TestUpdate0()
        {
            var ms = new MySQLClient(_conn);
            List<Product> products = ms.Select<Product>();
            Product product = products.Random();
            string name = "some";
            string id = ms.Update<Product>(product.ID, nameof(product.Name), name);
            Console.WriteLine(id);

            product = ms.Select<Product>(id);
            Assert.AreEqual(product.Name, name);
        }

        [TestMethod]
        public void TestUpdate1()
        {
            var ms = new MySQLClient(_conn);
            List<Product> products = ms.Select<Product>();
            Product product = products.Random();
            string name = "some";
            string id = ms.Update<Product>(product.ID, new Dictionary<string, object>() { { nameof(product.Name), name } });
            Console.WriteLine(id);

            product = ms.Select<Product>(id);
            Assert.AreEqual(product.Name, name);
        }

        [TestMethod]
        public void TestDelete0()
        {
            var ms = new MySQLClient(_conn);
            List<Product> products = ms.Select<Product>();
            Product product = products.Random();
            ms.Delete<Product>(product.ID);

            product = ms.Select<Product>(product.ID);
            Assert.IsNull(product);
        }

        [TestMethod]
        public void TestDelete1()
        {
            var ms = new MySQLClient(_conn);
            List<Employee> employees = ms.Select<Employee>();
            Employee employee = employees.Random();
            ms.Delete<Employee>(employee.ID);

            employee = ms.Select<Employee>(employee.ID);
            Assert.IsNull(employee);
        }

        [TestMethod]
        public void TestDelete2()
        {
            var ms = new MySQLClient(_conn);
            List<Maker> makers = ms.Select<Maker>();
            Maker maker = makers.Random();
            ms.Delete<Maker>(maker.ID);

            maker = ms.Select<Maker>(maker.ID);
            Assert.IsNull(maker);
        }
    }
}