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
            ObservableCollection<StockWrapper> collection = iowd.CreateCollection(StockType.ALL);
            ObservableCollection<StockWrapper> collection2 = iowd.CreateCollection(StockType.INCOMING);
            ObservableCollection<StockWrapper> collection3 = iowd.CreateCollection(StockType.OUTGOING);
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
            ObservableCollection<StockWrapper> collection = iowd.CreateCollection(StockType.ALL);
            var ioStockws = collection.ElementAt(rand.Next(collection.Count - 1));

            var fwd = FieldWrapperDirector.GetInstance();
            ObservableCollection<ItemWrapper> itemCollectoin = fwd.CreateCollection<Item, ItemWrapper>();
            ObservableCollection<SpecificationWrapper> specCollectoin = fwd.CreateCollection<Specification, SpecificationWrapper>();
            ObservableCollection<FieldWrapper<Measure>> measCollectoin = fwd.CreateCollection<Measure, FieldWrapper<Measure>>();
            ObservableCollection<FieldWrapper<Currency>> currCollectoin = fwd.CreateCollection<Currency, FieldWrapper<Currency>>();
            ObservableCollection<FieldWrapper<Employee>> eeplCollectoin = fwd.CreateCollection<Employee, FieldWrapper<Employee>>();
            ObservableCollection<ClientWrapper> accoCollectoin = fwd.CreateCollection<Client, ClientWrapper>();
            ObservableCollection<FieldWrapper<Maker>> makeCollectoin = fwd.CreateCollection<Maker, FieldWrapper<Maker>>();
            ObservableCollection<FieldWrapper<Warehouse>> wareCollectoin = fwd.CreateCollection<Warehouse, FieldWrapper<Warehouse>>();

            var type = ioStockws.StockType == StockType.INCOMING ? StockType.OUTGOING : StockType.INCOMING;
            var specw = ioStockws.Specification = specCollectoin.ElementAt(rand.Next(specCollectoin.Count - 1));
            var itemw = ioStockws.Item = itemCollectoin.Where(x => x.UUID == specw.Field.ItemUUID).Single();
            var itemCnt = ioStockws.Quantity = 20332;
            var date = ioStockws.Date = DateTime.Now.AddTicks(2000000221);
            var accountw = ioStockws.Client = accoCollectoin.ElementAt(rand.Next(accoCollectoin.Count - 1));
            var eemployeew = ioStockws.Employee = eeplCollectoin.ElementAt(rand.Next(eeplCollectoin.Count - 1));
            //var warehousew = ioStockws.Warehouse = wareCollectoin.ElementAt(rand.Next(wareCollectoin.Count - 1));
            var remark = ioStockws.Remark = "23_1jdjfa";

            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            CollectionViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            StockWrapperDirector.Distory();
            DatabaseDirector.Distroy();

            iowd = StockWrapperDirector.GetInstance();
            collection = iowd.CreateCollection(StockType.ALL);
            var target = collection.Where(x => x.UUID == ioStockws.UUID).Single();

            Assert.AreNotEqual(ioStockws, target);
            Assert.AreEqual(specw.UUID, target.Specification.UUID);
            Assert.AreEqual(itemw.UUID, target.Item.UUID);
            Assert.AreEqual(0, date.CompareTo(target.Date));
            Assert.AreEqual(accountw.UUID, target.Client.UUID);
            Assert.AreEqual(eemployeew.UUID, target.Employee.UUID);
            //Assert.AreEqual(warehousew.UUID, target.Warehouse.UUID);
            Assert.AreEqual(remark, target.Remark);
        }
    }
}