using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace R54IN0.Test
{
    [TestClass]
    public class ObservableStockTest
    {
        [TestMethod]
        public void CanCreate()
        {
            IOStockFormat fmt = new IOStockFormat();
            ObservableIOStock stock = new ObservableIOStock();
            stock = new ObservableIOStock(fmt);
        }

        [TestMethod]
        public void LoadProperties()
        {
            new Dummy().Create();
            using (var db = LexDb.GetDbInstance())
                db.Purge();
            ObservableInventory oinven = new ObservableInventory();
            oinven.Measure = new Observable<Measure>() { Name = "EA" };
            oinven.Product = new Observable<Product>() { Name = "product name" };
            oinven.Memo = "memo";
            oinven.Quantity = 123;
            oinven.Maker = new Observable<Maker>() { Name = "maker name" };
            oinven.Specification = "product's specification name(standard)";

            ObservableIOStock ostock = new ObservableIOStock();
            ostock.Date = DateTime.Now;
            ostock.Project = new Observable<Project>() { Name = "dy1234" };
            ostock.Supplier = new Observable<Supplier>() { Name = "some supplier" };
            ostock.Customer = new Observable<Customer>() { Name = "some customer" };
            ostock.Inventory = oinven;
            ostock.Memo = "some memo";
            ostock.Quantity = 123;
            ostock.StockType = IOStockType.INCOMING;
            ostock.UnitPrice = 10000;

            IOStockFormat stockFormat;
            using (var db = LexDb.GetDbInstance())
                stockFormat = db.LoadByKey<IOStockFormat>(ostock.ID);

            ObservableInventoryDirector.Distory();
            ObservableIOStock newOStock = new ObservableIOStock(stockFormat);
            Assert.AreEqual(ostock.ID, newOStock.ID);
            Assert.AreEqual(ostock.Project.ID, newOStock.Project.ID);
            Assert.AreEqual(ostock.Supplier.ID, newOStock.Supplier.ID);
            Assert.AreEqual(ostock.Customer.ID, newOStock.Customer.ID);
            Assert.AreEqual(ostock.Memo, newOStock.Memo);
            Assert.AreEqual(ostock.Quantity, newOStock.Quantity);
            Assert.AreEqual(ostock.StockType, newOStock.StockType);
            Assert.AreEqual(ostock.UnitPrice, newOStock.UnitPrice);
            Assert.AreEqual(ostock.Inventory.ID, newOStock.Inventory.ID);
        }
    }
}