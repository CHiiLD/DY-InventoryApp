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
            IOStockWrapperDirector iowd = IOStockWrapperDirector.GetInstance();
        }

        [TestMethod]
        public void Load()
        {
            new DummyDbData().Create();
            IOStockWrapperDirector iowd = IOStockWrapperDirector.GetInstance();
            ObservableCollection<IOStockWrapper> collection = iowd.CreateCollection(StockType.ALL);
            ObservableCollection<IOStockWrapper> collection2 = iowd.CreateCollection(StockType.IN);
            ObservableCollection<IOStockWrapper> collection3 = iowd.CreateCollection(StockType.OUT);
        }

        [TestMethod]
        public void Distory()
        {
            IOStockWrapperDirector.Distory();
        }

        [TestMethod]
        public void ChangeProperty()
        {
            new DummyDbData().Create();
            Random rand = new Random();
            IOStockWrapperDirector iowd = IOStockWrapperDirector.GetInstance();
            ObservableCollection<IOStockWrapper> collection = iowd.CreateCollection(StockType.ALL);
            var ioStockws = collection.ElementAt(rand.Next(collection.Count - 1));

            var fwd = FieldWrapperDirector.GetInstance();
            ObservableCollection<ItemWrapper> itemCollectoin = fwd.CreateCollection<Item, ItemWrapper>();
            ObservableCollection<SpecificationWrapper> specCollectoin = fwd.CreateCollection<Specification, SpecificationWrapper>();
            ObservableCollection<FieldWrapper<Measure>> measCollectoin = fwd.CreateCollection<Measure, FieldWrapper<Measure>>();
            ObservableCollection<FieldWrapper<Currency>> currCollectoin = fwd.CreateCollection<Currency, FieldWrapper<Currency>>();
            ObservableCollection<FieldWrapper<Employee>> eeplCollectoin = fwd.CreateCollection<Employee, FieldWrapper<Employee>>();
            ObservableCollection<AccountWrapper> accoCollectoin = fwd.CreateCollection<Account, AccountWrapper>();
            ObservableCollection<FieldWrapper<Maker>> makeCollectoin = fwd.CreateCollection<Maker, FieldWrapper<Maker>>();
            ObservableCollection<FieldWrapper<Warehouse>> wareCollectoin = fwd.CreateCollection<Warehouse, FieldWrapper<Warehouse>>();

            var type = ioStockws.StockType == StockType.IN ? StockType.OUT : StockType.IN;
            var specw = ioStockws.Specification = specCollectoin.ElementAt(rand.Next(specCollectoin.Count - 1));
            var itemw = ioStockws.Item = itemCollectoin.Where(x => x.UUID == specw.Field.ItemUUID).Single();
            var itemCnt = ioStockws.ItemCount = 20332;
            var date = ioStockws.Date = DateTime.Now.AddTicks(2000000221);
            var accountw = ioStockws.Account = accoCollectoin.ElementAt(rand.Next(accoCollectoin.Count - 1));
            var eemployeew = ioStockws.Employee = eeplCollectoin.ElementAt(rand.Next(eeplCollectoin.Count - 1));
            var warehousew = ioStockws.Warehouse = wareCollectoin.ElementAt(rand.Next(wareCollectoin.Count - 1));
            var remark = ioStockws.Remark = "23_1jdjfa";

            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            ViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            IOStockWrapperDirector.Distory();
            DatabaseDirector.Distroy();

            iowd = IOStockWrapperDirector.GetInstance();
            collection = iowd.CreateCollection(StockType.ALL);
            var target = collection.Where(x => x.UUID == ioStockws.UUID).Single();

            Assert.AreNotEqual(ioStockws, target);
            Assert.AreEqual(specw.UUID, target.Specification.UUID);
            Assert.AreEqual(itemw.UUID, target.Item.UUID);
            Assert.AreEqual(0, date.CompareTo(target.Date));
            Assert.AreEqual(accountw.UUID, target.Account.UUID);
            Assert.AreEqual(eemployeew.UUID, target.Employee.UUID);
            Assert.AreEqual(warehousew.UUID, target.Warehouse.UUID);
            Assert.AreEqual(remark, target.Remark);
        }
        //TODO : IOStockWrapperViewModel 객체 생성 후 동기화 검사
        //TODO : WPF에 적용 후 리팩토링하고 iostock ViewModelEditor 를 새로 디자인하고 그게 맞는 ViewModel 생성
    }
}