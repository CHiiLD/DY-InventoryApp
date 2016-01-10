using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;

namespace R54IN0.Test
{
    [TestClass]
    public class IOStockWrapperDirectorTest
    {
        [TestMethod]
        public void CanCreate()
        {
            StockWrapperDirector iowd = StockWrapperDirector.GetInstance();
        }

        [TestMethod]
        public void Load()
        {
            new DummyDbData().Create();
            StockWrapperDirector iowd = StockWrapperDirector.GetInstance();
            ObservableCollection<StockWrapper> collection = iowd.CreateCollection(IOStockType.ALL);
            ObservableCollection<StockWrapper> collection2 = iowd.CreateCollection(IOStockType.INCOMING);
            ObservableCollection<StockWrapper> collection3 = iowd.CreateCollection(IOStockType.OUTGOING);
        }

        [TestMethod]
        public void CanDistory()
        {
            StockWrapperDirector.Distory();
        }

        [TestMethod]
        public void ChangeProperty()
        {
            new DummyDbData().Create();
            Random rand = new Random();
            StockWrapperDirector iowd = StockWrapperDirector.GetInstance();
            ObservableCollection<StockWrapper> collection = iowd.CreateCollection(IOStockType.ALL);
            var ioStockws = collection.Random();

            var fwd = FieldWrapperDirector.GetInstance();
            ObservableCollection<ItemWrapper> itemCollectoin = fwd.CreateCollection<Item, ItemWrapper>();
            ObservableCollection<SpecificationWrapper> specCollectoin = fwd.CreateCollection<Specification, SpecificationWrapper>();
            ObservableCollection<Observable<Measure>> measCollectoin = fwd.CreateCollection<Measure, Observable<Measure>>();
            ObservableCollection<Observable<Currency>> currCollectoin = fwd.CreateCollection<Currency, Observable<Currency>>();
            ObservableCollection<Observable<Employee>> eeplCollectoin = fwd.CreateCollection<Employee, Observable<Employee>>();
            ObservableCollection<ClientWrapper> accoCollectoin = fwd.CreateCollection<Client, ClientWrapper>();
            ObservableCollection<Observable<Maker>> makeCollectoin = fwd.CreateCollection<Maker, Observable<Maker>>();
            ObservableCollection<Observable<Warehouse>> wareCollectoin = fwd.CreateCollection<Warehouse, Observable<Warehouse>>();

            var type = ioStockws.StockType == IOStockType.INCOMING ? IOStockType.OUTGOING : IOStockType.INCOMING;
            var specw = ioStockws.Specification = specCollectoin.Random();
            var itemw = ioStockws.Item = itemCollectoin.Where(x => x.ID == specw.Field.ItemID).Single();
            var itemCnt = ioStockws.Quantity = 20332;
            var date = ioStockws.Date = DateTime.Now.AddTicks(2000000221);
            var accountw = ioStockws.Client = accoCollectoin.Random();
            var eemployeew = ioStockws.Employee = eeplCollectoin.Random();
            var remark = ioStockws.Remark = "23_1jdjfa";

            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            CollectionViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            StockWrapperDirector.Distory();
            LexDb.Distroy();

            iowd = StockWrapperDirector.GetInstance();
            collection = iowd.CreateCollection(IOStockType.ALL);
            var target = collection.Where(x => x.ID == ioStockws.ID).Single();

            Assert.AreNotEqual(ioStockws, target);
            Assert.AreEqual(specw.ID, target.Specification.ID);
            Assert.AreEqual(itemw.ID, target.Item.ID);
            Assert.AreEqual(0, date.CompareTo(target.Date));
            Assert.AreEqual(accountw.ID, target.Client.ID);
            Assert.AreEqual(eemployeew.ID, target.Employee.ID);
            //Assert.AreEqual(warehousew.ID, target.Warehouse.ID);
            Assert.AreEqual(remark, target.Remark);
        }
    }
}