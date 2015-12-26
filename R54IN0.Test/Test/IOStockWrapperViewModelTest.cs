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
            StockWrapperViewModel vm = new StockWrapperViewModel(StockType.ALL, sub);
        }

        StockWrapper CreateIOStockWrapper()
        {
            //새로운 아이템 생성
            var fwd = FieldWrapperDirector.GetInstance();
            ObservableCollection<ItemWrapper> itemCollectoin = fwd.CreateCollection<Item, ItemWrapper>();
            ObservableCollection<SpecificationWrapper> specCollectoin = fwd.CreateCollection<Specification, SpecificationWrapper>();
            ObservableCollection<FieldWrapper<Measure>> measCollectoin = fwd.CreateCollection<Measure, FieldWrapper<Measure>>();
            ObservableCollection<FieldWrapper<Currency>> currCollectoin = fwd.CreateCollection<Currency, FieldWrapper<Currency>>();
            ObservableCollection<FieldWrapper<Employee>> eeplCollectoin = fwd.CreateCollection<Employee, FieldWrapper<Employee>>();
            ObservableCollection<ClientWrapper> accoCollectoin = fwd.CreateCollection<Client, ClientWrapper>();
            ObservableCollection<FieldWrapper<Maker>> makeCollectoin = fwd.CreateCollection<Maker, FieldWrapper<Maker>>();
            ObservableCollection<FieldWrapper<Warehouse>> wareCollectoin = fwd.CreateCollection<Warehouse, FieldWrapper<Warehouse>>();

            Random rand = new Random();
            var instance = new StockWrapper(new InOutStock());
            var type = instance.StockType == StockType.INCOMING ? StockType.OUTGOING : StockType.INCOMING;
            var specw = instance.Specification = specCollectoin.ElementAt(rand.Next(specCollectoin.Count - 1));
            var itemw = instance.Item = itemCollectoin.Where(x => x.UUID == specw.Field.ItemUUID).Single();
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
            StockType stockTypeAll = StockType.ALL;
            StockType stockTypeIn = StockType.INCOMING;
            StockType stockTypeOut = StockType.OUTGOING;
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
            //var fwd = FieldWrapperDirector.GetInstance();
            //ObservableCollection<SpecificationWrapper> specCollectoin = fwd.CreateCollection<Specification, SpecificationWrapper>();
            //var ioStockw = new IOStockWrapper(new InOutStock());
            //ioStockw.StockType = stockTypeIn;
            //ObservableCollection<ItemWrapper> itemCollectoin = fwd.CreateCollection<Item, ItemWrapper>();
            //ioStockw.Item = itemCollectoin.Where(x => x.UUID == dummy.TestItemUUID).Single();
            //ioStockw.Specification = specCollectoin.Where(x => x.Field.ItemUUID == dummy.TestItemUUID).First();

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
            StockType stockType = new Random().Next(2) == 1 ? StockType.INCOMING : StockType.OUTGOING;
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemFinderViewModel fvm = new ItemFinderViewModel(null);
            StockWrapperViewModel vm = new StockWrapperViewModel(stockType, sub);
            fvm.SelectItemsChanged += vm.OnFinderViewSelectItemChanged;
            ItemWrapper itemw = vm.Items.Random().Item;
            FinderNode node = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM && y.ItemUUID == itemw.UUID)).Single();
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
            StockType stockTypeAll = StockType.ALL;

            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemFinderViewModel fvm = new ItemFinderViewModel(null);
            StockWrapperViewModel vm1 = new StockWrapperViewModel(stockTypeAll, sub);
            var items = vm1.Items;

            fvm.SelectItemsChanged += vm1.OnFinderViewSelectItemChanged;

            //랜덤으로 하나의 아이템을 파인터에서 선택
            ItemWrapper itemw = items.ElementAt(0).Item;
            Assert.AreEqual(0, fvm.SelectedNodes.Count);
            FinderNode node = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM && y.ItemUUID == itemw.UUID)).Single();
            var list = new List<FinderNode>() { node };
            fvm.OnNodeSelected(fvm, new System.Windows.Controls.SelectionChangedCancelEventArgs(list, new List<FinderNode>()));

            Assert.AreEqual(1, fvm.SelectedNodes.Count);
            Assert.IsTrue(vm1.Items.All(x => x.Item.UUID == node.ItemUUID));

            itemw = items.Where(x => x.Item.UUID != itemw.UUID).First().Item;

            FinderNode node2 = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM && y.ItemUUID == itemw.UUID)).Single();
            var list2 = new List<FinderNode>() { node, node2 };
            fvm.OnNodeSelected(fvm, new System.Windows.Controls.SelectionChangedCancelEventArgs(list2, list));

            Assert.AreEqual(2, fvm.SelectedNodes.Count);
            Assert.IsTrue(vm1.Items.All(x => x.Item.UUID == node.ItemUUID || x.Item.UUID == node2.ItemUUID));

            //선택된 아이템들이 아닌 아이템을 추가할 경우 Finder에 제외되었기에 뺀다.
            var unSelectedItem = items.Where(x => x.Item.UUID != node.ItemUUID && x.Item.UUID != node2.ItemUUID).First();
            var newStock = CreateIOStockWrapper();
            newStock.Item = unSelectedItem.Item;
            newStock.Specification = unSelectedItem.Specification;

            vm1.Add(newStock);
            Assert.IsFalse(vm1.Items.Contains(newStock));
            var iowd = StockWrapperDirector.GetInstance();
            Assert.IsTrue(iowd.CreateCollection(stockTypeAll).Contains(newStock));

            //위 상황과 반대인 경우
            var selectedItem = items.Where(x => x.Item.UUID == itemw.UUID).First();
            newStock = CreateIOStockWrapper();
            newStock.Item = selectedItem.Item;
            newStock.Specification = selectedItem.Specification;
            vm1.Add(newStock);
            Assert.IsTrue(vm1.Items.Contains(newStock));
            Assert.IsTrue(iowd.CreateCollection(stockTypeAll).Contains(newStock));
        }
    }
}