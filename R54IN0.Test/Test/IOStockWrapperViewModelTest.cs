using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace R54IN0.Test
{
    [TestClass]
    public class IOStockWrapperViewModelTest
    {
        [TestMethod]
        public void CanCreate()
        {
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            StockWrapperViewModel vm = new StockWrapperViewModel(IOStockType.ALL, sub);
        }

        StockWrapper CreateIOStockWrapper()
        {
            //새로운 아이템 생성
            var fwd = FieldWrapperDirector.GetInstance();
            ObservableCollection<ItemWrapper> itemCollectoin = fwd.CreateCollection<Item, ItemWrapper>();
            ObservableCollection<SpecificationWrapper> specCollectoin = fwd.CreateCollection<Specification, SpecificationWrapper>();
            ObservableCollection<Observable<Measure>> measCollectoin = fwd.CreateCollection<Measure, Observable<Measure>>();
            ObservableCollection<Observable<Currency>> currCollectoin = fwd.CreateCollection<Currency, Observable<Currency>>();
            ObservableCollection<Observable<Employee>> eeplCollectoin = fwd.CreateCollection<Employee, Observable<Employee>>();
            ObservableCollection<ClientWrapper> accoCollectoin = fwd.CreateCollection<Client, ClientWrapper>();
            ObservableCollection<Observable<Maker>> makeCollectoin = fwd.CreateCollection<Maker, Observable<Maker>>();
            ObservableCollection<Observable<Warehouse>> wareCollectoin = fwd.CreateCollection<Warehouse, Observable<Warehouse>>();

            Random rand = new Random();
            var instance = new StockWrapper(new InOutStock());
            instance.Inventory = InventoryWrapperDirector.GetInstance().CreateCollection().Random();
            var type = instance.StockType == IOStockType.INCOMING ? IOStockType.OUTGOING : IOStockType.INCOMING;
            var specw = instance.Specification = specCollectoin.ElementAt(rand.Next(specCollectoin.Count - 1));
            var itemw = instance.Item = itemCollectoin.Where(x => x.ID == specw.Field.ItemID).Single();
            var itemCnt = instance.Quantity = 20332;
            var date = instance.Date = DateTime.Now.AddTicks(2000000221);
            var accountw = instance.Client = accoCollectoin.ElementAt(rand.Next(accoCollectoin.Count - 1));
            var eemployeew = instance.Employee = eeplCollectoin.ElementAt(rand.Next(eeplCollectoin.Count - 1));
            var warehousew = instance.Warehouse;
            var remark = instance.Remark = "3^^a";
            return instance;
        }

        /// <summary>
        /// ViewModel 끼리 아이템 동기화
        /// </summary>
        [TestMethod]
        public void SyncViewModel()
        {
            IOStockType stockTypeAll = IOStockType.ALL;
            IOStockType stockTypeIn = IOStockType.INCOMING;
            IOStockType stockTypeOut = IOStockType.OUTGOING;
            var dummy = new DummyDbData().Create();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            StockWrapperViewModel vm1 = new StockWrapperViewModel(stockTypeAll, sub);
            StockWrapperViewModel vm2 = new StockWrapperViewModel(stockTypeAll, sub);
            StockWrapperViewModel vm3 = new StockWrapperViewModel(stockTypeIn, sub);
            StockWrapperViewModel vm4 = new StockWrapperViewModel(stockTypeOut, sub);

            Assert.AreNotEqual(0, vm1.Items.Count);
            int cnt = vm1.Items.Count();

            var ioStockw = CreateIOStockWrapper();
            ioStockw.StockType = stockTypeIn;

            //추가 이전
            Assert.IsFalse(vm1.Items.Contains(ioStockw));
            Assert.IsFalse(vm2.Items.Contains(ioStockw));
            Assert.IsFalse(vm3.Items.Contains(ioStockw));
            Assert.IsFalse(vm4.Items.Contains(ioStockw));
            //추가
            vm1.Add(ioStockw);
            Assert.AreEqual(cnt + 1, vm1.Items.Count);
            Assert.IsTrue(vm1.Items.Contains(ioStockw));
            Assert.IsTrue(vm2.Items.Contains(ioStockw));
            Assert.IsTrue(vm3.Items.Contains(ioStockw));
            Assert.IsFalse(vm4.Items.Contains(ioStockw));

            var iowd = StockWrapperDirector.GetInstance();
            Assert.IsTrue(iowd.CreateCollection(stockTypeAll).Contains(ioStockw));
            //삭제
            vm1.Remove(ioStockw);

            Assert.AreEqual(cnt, vm1.Items.Count);
            Assert.IsFalse(vm1.Items.Contains(ioStockw));
            Assert.IsFalse(vm2.Items.Contains(ioStockw));
            Assert.IsFalse(vm3.Items.Contains(ioStockw));
            Assert.IsFalse(vm4.Items.Contains(ioStockw));

            Assert.IsFalse(iowd.CreateCollection(stockTypeAll).Contains(ioStockw));
        }

        /// <summary>
        /// Finder에서 품목을 선택하였을 때 해당 품목에 해당되는 리스트 아이템들의 StockType이 
        /// 올바르게 업데이트되는지를 확인한다.
        /// </summary>
        [TestMethod]
        public void WhenClickFinderNodeThenUpdateItemsWidthStockTypeMatched()
        {
            new DummyDbData().Create();
            IOStockType stockType = new Random().Next(2) == 1 ? IOStockType.INCOMING : IOStockType.OUTGOING;
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemFinderViewModel fvm = new ItemFinderViewModel(null);
            StockWrapperViewModel vm = new StockWrapperViewModel(stockType, sub);
            fvm.SelectItemsChanged += vm.OnFinderViewSelectItemChanged;
            ItemWrapper itemw = vm.Items.Random().Item;
            FinderNode node = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT && y.ItemID == itemw.ID)).Single();
            var list = new List<FinderNode>() { node };
            //클릭
            fvm.OnNodeSelected(fvm, new System.Windows.Controls.SelectionChangedCancelEventArgs(list, new List<FinderNode>()));

            Assert.IsTrue(vm.Items.All(x => x.StockType == stockType));
        }

        /// <summary>
        /// Finder에 의한 동기화
        /// </summary>
        [TestMethod]
        public void SyncFinder()
        {
            new DummyDbData().Create();
            IOStockType stockTypeAll = IOStockType.ALL;

            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemFinderViewModel fvm = new ItemFinderViewModel(null);
            StockWrapperViewModel vm1 = new StockWrapperViewModel(stockTypeAll, sub);
            var items = vm1.Items;

            fvm.SelectItemsChanged += vm1.OnFinderViewSelectItemChanged;

            //랜덤으로 하나의 아이템을 파인터에서 선택
            ItemWrapper itemw = items.ElementAt(0).Item;
            Assert.AreEqual(0, fvm.SelectedNodes.Count);
            FinderNode node = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT && y.ItemID == itemw.ID)).Single();
            var list = new List<FinderNode>() { node };
            fvm.OnNodeSelected(fvm, new System.Windows.Controls.SelectionChangedCancelEventArgs(list, new List<FinderNode>()));

            Assert.AreEqual(1, fvm.SelectedNodes.Count);
            Assert.IsTrue(vm1.Items.All(x => x.Item.ID == node.ItemID));

            itemw = items.Where(x => x.Item.ID != itemw.ID).First().Item;

            FinderNode node2 = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT && y.ItemID == itemw.ID)).Single();
            var list2 = new List<FinderNode>() { node, node2 };
            fvm.OnNodeSelected(fvm, new System.Windows.Controls.SelectionChangedCancelEventArgs(list2, list));

            Assert.AreEqual(2, fvm.SelectedNodes.Count);
            Assert.IsTrue(vm1.Items.All(x => x.Item.ID == node.ItemID || x.Item.ID == node2.ItemID));

            //선택된 아이템들이 아닌 아이템을 추가할 경우 Finder에 제외되었기에 뺀다.
            var unSelectedItem = items.Where(x => x.Item.ID != node.ItemID && x.Item.ID != node2.ItemID).First();
            var newStock = CreateIOStockWrapper();
            newStock.Item = unSelectedItem.Item;
            newStock.Specification = unSelectedItem.Specification;

            vm1.Add(newStock);
            Assert.IsFalse(vm1.Items.Contains(newStock));
            var iowd = StockWrapperDirector.GetInstance();
            Assert.IsTrue(iowd.CreateCollection(stockTypeAll).Contains(newStock));

            //위 상황과 반대인 경우
            var selectedItem = items.Where(x => x.Item.ID == itemw.ID).First();
            newStock = CreateIOStockWrapper();
            newStock.Item = selectedItem.Item;
            newStock.Specification = selectedItem.Specification;
            vm1.Add(newStock);
            Assert.IsTrue(vm1.Items.Contains(newStock));
            Assert.IsTrue(iowd.CreateCollection(stockTypeAll).Contains(newStock));
        }

        /// <summary>
        /// 날짜와 키워드를 사용하여 특정 데이터를 검색가능케한다.
        /// 검색 가능 키워드는? 
        /// Item, Specification Property만 가능하다. (Binary Search 기준)
        /// item - Currency(필요없), Measure(필요없), Maker
        /// Item - Specification->ItemID
        /// Item - InventoryItemID - Warehouse
        /// </summary>
        [TestMethod]
        public void SearchAsDateAndKeyword()
        {
            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            CollectionViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            StockWrapperDirector.Distory();
            LexDb.Distroy();
            new DYDummyDbData().Create();

            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemFinderViewModel fvm = new ItemFinderViewModel(null);
            SearchStockWrapperViewModel vm = new SearchStockWrapperViewModel(IOStockType.ALL, sub);

            string itemName = "     스위치 ";
            string dummyName = "23094832098432";
            string makerName = "\tLG\t";
            string specificationName = "삼파";
            string warehouseName = "연구";
            string sumName = "버튼\t 단자부\n버섯\r 213o4u12oi\t";

            DateTime date1 = DateTime.Now.AddYears(-1);
            DateTime date2 = DateTime.Now;

            //item
            vm.SearchKeyword<Item>(itemName);
            Assert.IsTrue(vm.Items.All(x => x.Item.Name.Contains("스위치")));

            vm.SearchKeyword<Item>(dummyName);
            Assert.IsTrue(vm.Items.Count == 0);

            vm.SearchKeyword<Item>(itemName, date1, date2);
            Assert.IsTrue(vm.Items.All(x => x.Item.Name.Contains("스위치")));

            foreach (var item in vm.Items)
            {
                var date = item.Date;
                Assert.IsTrue(date1 <= date && date <= date2);
            }

            vm.SearchKeyword<Item>(sumName);
            foreach (var name in sumName.Split(new char[] { ' ', '\t', '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries))
            {
                Assert.IsTrue
                    (
                        vm.Items.Any(x => x.Item.Name.Contains("버튼")) ||
                        vm.Items.Any(x => x.Item.Name.Contains("단자부")) ||
                        vm.Items.Any(x => x.Item.Name.Contains("버섯"))
                    );
            }

            //maker
            vm.SearchKeyword<Maker>(makerName, date1, date2);
            Assert.IsTrue(vm.Items.All(x => x.Item.SelectedMaker.Name.Contains("LG")));

            foreach (var item in vm.Items)
            {
                var date = item.Date;
                Assert.IsTrue(date1 <= date && date <= date2);
            }
            //spec
            vm.SearchKeyword<Specification>(specificationName);
            Assert.AreNotEqual(0, vm.Items.Count);
            Assert.IsTrue(vm.Items.All(x => x.Specification.Name.Contains(specificationName)));
            //warehouse
            vm.SearchKeyword<Warehouse>(warehouseName);
            Assert.IsTrue(vm.Items.All(x => x.Warehouse.Name.Contains(warehouseName)));
        }

        /// <summary>
        /// ISearchEngine 인터페이스의 프로퍼티와 커맨드가 제대로 작동하는지 확인한다.
        /// </summary>
        [TestMethod]
        public void SearchEngineProperty()
        {
            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            CollectionViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            StockWrapperDirector.Distory();
            LexDb.Distroy();
            new DYDummyDbData().Create();

            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemFinderViewModel fvm = new ItemFinderViewModel(null);
            SearchStockWrapperViewModel vm = new SearchStockWrapperViewModel(IOStockType.ALL, sub);
            //메모리 할당 확인
            Assert.IsNotNull(vm.SearchTypes);
            Assert.IsNotNull(vm.SelectedSearchType);
            //오늘자 확인
            vm.TodayCommand.Execute(null);
            var now = DateTime.Now;
            DateTime formD = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0, DateTimeKind.Local);
            DateTime toD = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Local);
            Assert.AreEqual(vm.FromDateTime, formD);
            Assert.AreEqual(vm.ToDateTime, toD);
            //이번달 1일부터 오늘까지
            vm.ThisMonthCommand.Execute(null);
            formD = new DateTime(now.Year, now.Month, 1, 0, 0, 0, 0, DateTimeKind.Local);
            toD = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Local);
            Assert.AreEqual(vm.FromDateTime, formD);
            Assert.AreEqual(vm.ToDateTime, toD);
            //어제 확인 
            vm.YesterdayCommand.Execute(null);
            var yes = now.AddDays(-1);
            formD = new DateTime(yes.Year, yes.Month, yes.Day, 0, 0, 0, 0, DateTimeKind.Local);
            toD = new DateTime(yes.Year, yes.Month, yes.Day, 23, 59, 59, 999, DateTimeKind.Local);
            Assert.AreEqual(vm.FromDateTime, formD);
            Assert.AreEqual(vm.ToDateTime, toD);
            //이번주부터 오늘까지
            vm.ThisWorkCommand.Execute(null);
            var work = now.AddDays(-(int)now.DayOfWeek);
            formD = new DateTime(work.Year, work.Month, work.Day, 0, 0, 0, 0, DateTimeKind.Local);
            toD = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Local);
            Assert.AreEqual(vm.FromDateTime, formD);
            Assert.AreEqual(vm.ToDateTime, toD);
            //검색
            vm.FromDateTime = DateTime.Now.AddDays(-300);
            vm.ToDateTime = DateTime.Now;
            vm.SelectedSearchType = vm.SearchTypes.First(); //품목으로

            Assert.IsTrue("텍스트".Contains(""));

            vm.Keyword = "";
            vm.SearchCommand.Execute(null); //전체검사

            Assert.IsNotNull(vm.Items);
            Assert.AreNotEqual(0, vm.Items.Count);
        }
    }
}