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
            StockFormat fmt = new StockFormat();
            ObservableStock stock = new ObservableStock();
            stock = new ObservableStock(fmt);
        }

        [TestMethod]
        public void LoadProperties()
        {
            using (var db = LexDb.GetDbInstance())
                db.Purge();
            ObservableInventory oinven = new ObservableInventory();
            oinven.Currency = new Observable<Currency>() { Name = "doller" };
            oinven.Maker = new Observable<Maker>() { Name = "maker name" };
            oinven.Measure = new Observable<Measure>() { Name = "EA" };
            oinven.Product = new Observable<Product>() { Name = "product name" };
            oinven.Memo = "memo";
            oinven.Quantity = 123;
            oinven.Specification = "product's specification name(standard)";

            ObservableStock ostock = new ObservableStock();
            ostock.Date = DateTime.Now;
            ostock.Employee = new Observable<Employee>() { Name = "who?" };
            ostock.Project = new Observable<Project>() { Name = "dy1234" };
            ostock.Supplier = new Observable<Supplier>() { Name = "여명" };
            ostock.Customer = new Observable<Customer>() { Name = "어딘가 .." };
            ostock.Inventory = oinven;
            ostock.Memo = "abcd";
            ostock.Quantity = 123;
            ostock.StockType = StockType.INCOMING;
            ostock.UnitPrice = 10000;

            StockFormat stockFormat;
            using (var db = LexDb.GetDbInstance())
                stockFormat = db.LoadByKey<StockFormat>(ostock.ID);

            ObservableStock newOStock = new ObservableStock(stockFormat);
            Assert.AreEqual(ostock.ID, newOStock.ID);
            Assert.AreEqual(ostock.Employee.ID, newOStock.Employee.ID);
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
