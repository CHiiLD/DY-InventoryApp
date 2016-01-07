using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace R54IN0.Test
{
    [TestClass]
    public class ObservableStockTest
    {
        [TestMethod]
        public void CanCreate()
        {
            InoutStockFormat fmt = new InoutStockFormat();
            ObservableInoutStock stock = new ObservableInoutStock();
            stock = new ObservableInoutStock(fmt);
        }

        [TestMethod]
        public void LoadProperties()
        {
            new Dummy2().Create();
            using (var db = LexDb.GetDbInstance())
                db.Purge();
            ObservableInventory oinven = new ObservableInventory();
            oinven.Measure = new Observable<Measure>() { Name = "EA" };
            oinven.Product = new Observable<Product>() { Name = "product name" };
            oinven.Memo = "memo";
            oinven.Quantity = 123;
            oinven.Maker = new Observable<Maker>() { Name = "maker name" };
            oinven.Specification = "product's specification name(standard)";

            ObservableInoutStock ostock = new ObservableInoutStock();
            ostock.Date = DateTime.Now;
            ostock.Project = new Observable<Project>() { Name = "dy1234" };
            ostock.Supplier = new Observable<Supplier>() { Name = "여명" };
            ostock.Customer = new Observable<Customer>() { Name = "어딘가 .." };
            ostock.Inventory = oinven;
            ostock.Memo = "abcd";
            ostock.Quantity = 123;
            ostock.StockType = StockType.INCOMING;
            ostock.UnitPrice = 10000;

            InoutStockFormat stockFormat;
            using (var db = LexDb.GetDbInstance())
                stockFormat = db.LoadByKey<InoutStockFormat>(ostock.ID);

            ObservableInvenDirector.Distory();
            ObservableInoutStock newOStock = new ObservableInoutStock(stockFormat);
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
