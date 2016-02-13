using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace R54IN0.Test
{
    [TestClass]
    public class SQLiteServerUnitTest
    {
        [TestMethod]
        public void TestField()
        {
            SQLiteServer server = new SQLiteServer();
            server.Open();
            server.DropAllTableThenReCreateTable();
            //insert
            Customer customer = new Customer();
            string id = customer.ID = Guid.NewGuid().ToString();
            string name = customer.Name = "some customer";
            server.Insert(customer);
            //select
            Customer result = server.Select<Customer>(nameof(customer.ID), id);
            Assert.AreEqual(id, result.ID);
            Assert.AreEqual(name, result.Name);
            //udpate
            string newname = "new customer";
            result.Name = newname;
            server.Update(result, nameof(customer.Name));

            result = server.Select<Customer>(nameof(customer.ID), id);
            Assert.AreEqual(id, result.ID);
            Assert.AreEqual(newname, result.Name);
            //delete
            server.Delete(result);
            result = server.Select<Customer>(nameof(customer.ID), id);
            Assert.IsNull(result);

            server.Close();
        }

        [TestMethod]
        public void TestInventoryFormat()
        {
            SQLiteServer server = new SQLiteServer();
            server.Open();
            server.DropAllTableThenReCreateTable();

            InventoryFormat fmt = new InventoryFormat();
            //insert
            string id = fmt.ID = Guid.NewGuid().ToString();
            int qty = fmt.Quantity = 101221210;
            server.Insert(fmt);

            //select
            InventoryFormat result = server.Select<InventoryFormat>(nameof(fmt.ID), id);
            Assert.AreEqual(id, result.ID);
            Assert.AreEqual(qty, result.Quantity);

            server.Close();
        }

        [TestMethod]
        public void TestIOStockFormat()
        {
            SQLiteServer server = new SQLiteServer();
            server.Open();
            server.DropAllTableThenReCreateTable();

            IOStockFormat fmt = new IOStockFormat();
            //insert
            string id = fmt.ID = Guid.NewGuid().ToString();
            decimal price = fmt.UnitPrice = 100000000000000L;
            DateTime date = fmt.Date = DateTime.Now;
            IOStockType type = fmt.StockType = IOStockType.INCOMING;
            server.Insert(fmt);

            //select
            IOStockFormat result = server.Select<IOStockFormat>(nameof(fmt.ID), id);
            Assert.AreEqual(id, result.ID);
            Assert.AreEqual(price, result.UnitPrice);
            Assert.AreEqual(date, result.Date);
            Assert.AreEqual(type, result.StockType);

            server.Close();
        }
    }
}