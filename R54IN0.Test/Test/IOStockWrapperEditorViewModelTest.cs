using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Collections.Generic;

namespace R54IN0.Test
{
    [TestClass]
    public class IOStockWrapperEditorViewModelTest
    {
        [TestMethod]
        public void CanCreate()
        {
            new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            IOStockWrapperViewModel iowvm = new IOStockWrapperViewModel(StockType.ALL, sub);
            new IOStockWrapperEditorViewModel(iowvm);
            var iostockw = IOStockWrapperDirector.GetInstance().CreateCollection(StockType.ALL).First();
            new IOStockWrapperEditorViewModel(iowvm, iostockw);
        }

        /// <summary>
        /// IOStockWrapperEditorViewModel객체 생성시 StockType이 고정되어야 한다.
        /// </summary>
        [TestMethod]
        public void WhenCreatedInstanceThenStockTypeDefiend()
        {
            new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            IOStockWrapperViewModel vm = new IOStockWrapperViewModel(StockType.IN, sub);
            var evm = new IOStockWrapperEditorViewModel(vm);
            Assert.AreEqual(StockType.IN, evm.StockType);
            Assert.AreEqual(1, evm.StockTypeList.Count());
            Assert.AreEqual(StockType.IN, evm.StockTypeList.First());

            vm = new IOStockWrapperViewModel(StockType.OUT, sub);
            evm = new IOStockWrapperEditorViewModel(vm);
            Assert.AreEqual(StockType.OUT, evm.StockType);
            Assert.AreEqual(1, evm.StockTypeList.Count());
            Assert.AreEqual(StockType.OUT, evm.StockTypeList.First());

            vm = new IOStockWrapperViewModel(StockType.ALL, sub);
            evm = new IOStockWrapperEditorViewModel(vm);
            Assert.AreEqual(StockType.IN, evm.StockType);
            Assert.AreEqual(2, evm.StockTypeList.Count());
        }

        /// <summary>
        /// IOStockWrapperEditorViewModel객체 생성시 거래처와 담당자 컬렉션이 채워져 있어야 한다.
        /// </summary>
        [TestMethod]
        public void WhenCreatedInstanceThenInitCollectionProperties()
        {
            new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            IOStockWrapperViewModel vm = new IOStockWrapperViewModel(StockType.IN, sub);
            var evm = new IOStockWrapperEditorViewModel(vm);

            Assert.IsNotNull(evm.AccountList);
            Assert.IsNotNull(evm.EmployeeList);
        }

        /// <summary>
        /// Finder에서 객체를 클릭하였을 때 해당 품목에 관련된 규격을 규격리스트에 불러온다.
        /// </summary>
        [TestMethod]
        public void WhenClickFinderNodeThenUpdateProperties()
        {
            new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            IOStockWrapperViewModel vm = new IOStockWrapperViewModel(StockType.IN, sub);
            var evm = new IOStockWrapperEditorViewModel(vm);
            FinderViewModel fvm = evm.CreateFinderViewModel(null);
            var node = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM)).Random();

            evm.ItemCount = 120;

            Assert.IsNull(evm.ItemList);
            Assert.IsNull(evm.Item);
            Assert.IsNull(evm.Maker);
            Assert.IsNull(evm.Measure);
            Assert.IsNull(evm.Currency);
            Assert.IsNull(evm.Warehouse);

            fvm.OnNodeSelected(fvm, new SelectionChangedCancelEventArgs(new List<FinderNode>() { node }, new List<FinderNode>()));

            Assert.IsNotNull(evm.ItemList);
            evm.Item = evm.ItemList.Random();

            Assert.IsTrue(evm.SpecificationList.All(x => x.Field.ItemUUID == node.ItemUUID));
            Assert.IsNotNull(evm.Item);
            Assert.IsNotNull(evm.Maker);
            Assert.IsNotNull(evm.Measure);
            Assert.IsNotNull(evm.Currency);

            Assert.IsNull(evm.Specification);
            Assert.AreEqual(0, evm.PurchaseUnitPrice);
            Assert.AreEqual(0, evm.TotalPurchasePrice);
            Assert.AreEqual(0, evm.TotalSalesPrice);
            Assert.AreEqual(0, evm.SalesUnitPrice);

            evm.Specification = evm.SpecificationList.Random();

            Assert.IsNotNull(evm.Specification);
            Assert.AreNotEqual(0, evm.PurchaseUnitPrice);
            Assert.AreNotEqual(0, evm.TotalPurchasePrice);
            Assert.AreNotEqual(0, evm.TotalSalesPrice);
            Assert.AreNotEqual(0, evm.SalesUnitPrice);
        }

        /// <summary>
        /// IOStockWrapper 객체를 수정하고자 할 때
        /// </summary>
        [TestMethod]
        public void WhenEditStockWrapperThenInitProperties()
        {
            new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            var vm = new IOStockWrapperViewModel(StockType.IN, sub);
            var ioStockw = IOStockWrapperDirector.GetInstance().CreateCollection(StockType.IN).Random();
            var evm = new IOStockWrapperEditorViewModel(vm, ioStockw);

            Assert.AreEqual(1, evm.SpecificationList.Count());
            Assert.AreEqual(ioStockw.Specification, evm.Specification);

            Assert.IsNotNull(evm.Item);
            Assert.IsNotNull(evm.Maker);
            Assert.IsNotNull(evm.Measure);
            Assert.IsNotNull(evm.Currency);
            Assert.IsNotNull(evm.Warehouse);

            Assert.IsNotNull(evm.ItemList);
            Assert.IsNotNull(evm.WarehouseList);
            Assert.IsNotNull(evm.SpecificationList);
            Assert.IsNotNull(evm.AccountList);
            Assert.IsNotNull(evm.EmployeeList);
            Assert.IsNotNull(evm.SpecificationList);

            Assert.AreNotEqual(0, evm.PurchaseUnitPrice);
            Assert.AreNotEqual(0, evm.TotalPurchasePrice);
            Assert.AreNotEqual(0, evm.TotalSalesPrice);
            Assert.AreNotEqual(0, evm.SalesUnitPrice);
        }

        /// <summary>
        /// 입출고 시 재고수량을 계산한다.
        /// </summary>
        [TestMethod]
        public void WhenInputItemCountThenSyncInventoryItemCount()
        {
            new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            IOStockWrapperViewModel vm = new IOStockWrapperViewModel(StockType.IN, sub);
            var evm = new IOStockWrapperEditorViewModel(vm);
            FinderViewModel fvm = evm.CreateFinderViewModel(null);
            var node = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM)).Random();
            fvm.OnNodeSelected(fvm, new SelectionChangedCancelEventArgs(new List<FinderNode>() { node }, new List<FinderNode>()));
            evm.Item = evm.ItemList.Random();
            evm.Specification = evm.SpecificationList.Random();
            var stockCnt = evm.ItemCount = 5;
            InventoryWrapperDirector iwd = InventoryWrapperDirector.GetInstance();
            var invenw = iwd.CreateCollection().Where(x => x.Specification == evm.Specification).SingleOrDefault();
            if (invenw != null)
            {
                Assert.AreEqual(evm.InventoryItemCount, invenw.ItemCount + stockCnt);
                evm.StockType = StockType.OUT;
                Assert.AreEqual(evm.InventoryItemCount, invenw.ItemCount - stockCnt);
            }
        }

        /// <summary>
        /// 입출고 시 재고수량을 계산한다.
        /// </summary>
        [TestMethod]
        public void WhenInputItemCountThenSyncInventoryItemCount2()
        {
            new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            IOStockWrapperViewModel vm = new IOStockWrapperViewModel(StockType.IN, sub);
            var iostockw = IOStockWrapperDirector.GetInstance().CreateCollection(StockType.IN).Random();
            var evm = new IOStockWrapperEditorViewModel(vm, iostockw);

            var beforCnt = evm.ItemCount;
            var afterCnt = evm.ItemCount = 2;

            InventoryWrapperDirector iwd = InventoryWrapperDirector.GetInstance();
            var invenw = iwd.CreateCollection().Where(x => x.Specification == evm.Specification).SingleOrDefault();
            if (invenw != null)
            {
                Assert.AreEqual(evm.InventoryItemCount, invenw.ItemCount - beforCnt + afterCnt);
                evm.StockType = StockType.OUT;
                Assert.AreEqual(evm.InventoryItemCount, invenw.ItemCount + beforCnt - afterCnt);
            }
        }

        /// <summary>
        /// 추가된 객체가 제대로 적용되었는지 살펴본다.
        /// </summary>
        [TestMethod]
        public void Update()
        {
            new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            IOStockWrapperViewModel vm = new IOStockWrapperViewModel(StockType.IN, sub);
            var evm = new IOStockWrapperEditorViewModel(vm);
            FinderViewModel fvm = new FinderViewModel(null, new ObservableCollection<FinderNode>(FinderDirector.GetInstance().Collection));
            fvm.SelectItemsChanged += evm.OnFinderViewSelectItemChanged;
            var node = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM)).Random();
            fvm.OnNodeSelected(fvm, new SelectionChangedCancelEventArgs(new List<FinderNode>() { node }, new List<FinderNode>()));
            var item = evm.Item = evm.ItemList.Random();
            var spec = evm.Specification = evm.SpecificationList.Random();
            var account = evm.Account = evm.AccountList.Random();
            var employee = evm.Employee = evm.EmployeeList.Random();
            var remark = evm.Remark = "rkfkskekfkakqkt!!!3089432";
            var stockCnt = evm.ItemCount = 5;

            var result = evm.Update();

            Assert.IsTrue(vm.Items.Any(x => x == result));
            Assert.AreEqual(remark, result.Remark);
            Assert.AreEqual(stockCnt, result.ItemCount);
            Assert.AreEqual(item, result.Item);
            Assert.AreEqual(spec, result.Specification);
            Assert.AreEqual(employee, result.Employee);
            Assert.AreEqual(account, result.Account);
        }

        /// <summary>
        /// 변경된 객체가 제대로 적용되었는지 살펴본다.
        /// </summary>
        [TestMethod]
        public void Update2()
        {
            new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            IOStockWrapperViewModel vm = new IOStockWrapperViewModel(StockType.IN, sub);
            var iostockw = IOStockWrapperDirector.GetInstance().CreateCollection(StockType.IN).Random();
            var evm = new IOStockWrapperEditorViewModel(vm, iostockw);
            var account = evm.Account = evm.AccountList.Random();
            var employee = evm.Employee = evm.EmployeeList.Random();
            var remark = evm.Remark = "rkfkskekfkakqkt!!!308922432";
            var stockCnt = evm.ItemCount = 20;

            var result = evm.Update();

            Assert.IsTrue(vm.Items.Any(x => x == result));
            Assert.AreEqual(remark, result.Remark);
            Assert.AreEqual(employee, result.Employee);
            Assert.AreEqual(account, result.Account);
        }

        [TestMethod]
        public void CanCreateFinder()
        {
            new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            IOStockWrapperViewModel iowvm = new IOStockWrapperViewModel(StockType.ALL, sub);
            var evm = new IOStockWrapperEditorViewModel(iowvm);
            FinderViewModel fvm = evm.CreateFinderViewModel(null);
        }

        [TestMethod]
        public void WarehouseProperty()
        {
            new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            IOStockWrapperViewModel vm = new IOStockWrapperViewModel(StockType.IN, sub);
            var evm = new IOStockWrapperEditorViewModel(vm);
            FinderViewModel fvm = evm.CreateFinderViewModel(null);

            Assert.IsNull(evm.Warehouse);
            Assert.IsNull(evm.WarehouseList);

            var node = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM)).Random();
            fvm.OnNodeSelected(fvm, new SelectionChangedCancelEventArgs(new List<FinderNode>() { node }, new List<FinderNode>()));

            var iwd = InventoryWrapperDirector.GetInstance();

            foreach (var item in evm.ItemList)
            {
                evm.Item = item;
                evm.Specification = evm.SpecificationList.Random();
                var inven = iwd.CreateCollection().Where(x => x.Specification.UUID == evm.Specification.UUID).SingleOrDefault();
                if (inven != null)
                {
                    Assert.IsNotNull(evm.Warehouse);
                    Assert.IsNotNull(evm.WarehouseList);
                    Assert.AreEqual(1, evm.WarehouseList.Count());
                    Assert.AreEqual(inven.Warehouse, evm.Warehouse);
                }
                else
                {
                    Assert.IsNull(evm.Warehouse);
                    Assert.IsNotNull(evm.WarehouseList);
                }
            }
        }

        /// <summary>
        /// 변경 또는 재고에 등록되어 있으면서 새로 입출고 데이터를 업데이트하는 경우 해당 재고의 재고개수를 변경하고
        /// 재고데이터에 등록되어 있지 아니하지만 새로 입출고 데이터를 업데이트하는 경우 재고 인스턴스를 생성하여 
        /// 재고 뷰와 재고 관리자에 각각 새로운 재고 인스턴스를 등록하여야 한다.
        /// </summary>
        [TestMethod]
        public void SyncInventoryViewModel()
        {
            //IN
            new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            IOStockWrapperViewModel vm = new IOStockWrapperViewModel(StockType.IN, sub);

            var iostockw = IOStockWrapperDirector.GetInstance().CreateCollection(StockType.IN).Random();
            iostockw.ItemCount = 10;

            var invenw = InventoryWrapperDirector.GetInstance().CreateCollection().Where(x => x.Specification.UUID == iostockw.Specification.UUID).Single();
            invenw.ItemCount = 20;

            var evm = new IOStockWrapperEditorViewModel(vm, iostockw);
            evm.Account = evm.AccountList.Random();
            evm.Employee = evm.EmployeeList.Random();
            evm.ItemCount = 3;
            var invenCnt = evm.InventoryItemCount;
            evm.Update();

            //OUT
            Assert.AreEqual(13, invenw.ItemCount);
            vm = new IOStockWrapperViewModel(StockType.OUT, sub);

            iostockw = IOStockWrapperDirector.GetInstance().CreateCollection(StockType.OUT).Random();
            iostockw.ItemCount = 10;

            invenw = InventoryWrapperDirector.GetInstance().CreateCollection().Where(x => x.Specification.UUID == iostockw.Specification.UUID).Single();
            invenw.ItemCount = 20;

            evm = new IOStockWrapperEditorViewModel(vm, iostockw);
            evm.Account = evm.AccountList.Random();
            evm.Employee = evm.EmployeeList.Random();
            evm.ItemCount = 3;
            invenCnt = evm.InventoryItemCount;
            evm.Update();

            Assert.AreEqual(27, invenw.ItemCount);
        }

        /// <summary>
        /// Update 시 재고에 등록되지 아니한 데이터라면 재고 인스턴스를 생성하여 
        /// 재고 데이터에 등록
        /// </summary>
        [TestMethod]
        public void WhenUpdate_IfNotExistInvenData_CreateInvenWrapper()
        {
            var dummy = new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            InventoryWrapperViewModel ivm = new InventoryWrapperViewModel(sub);
            IOStockWrapperViewModel vm = new IOStockWrapperViewModel(StockType.IN, sub);
            var evm = new IOStockWrapperEditorViewModel(vm);
            FinderViewModel fvm = evm.CreateFinderViewModel(null);
            var node = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM)).Where(x => x.ItemUUID == dummy.UnregisterdTestItemUUID).Single();
            fvm.OnNodeSelected(fvm, new SelectionChangedCancelEventArgs(new List<FinderNode>() { node }, new List<FinderNode>()));
            var item = evm.Item = evm.ItemList.Random();
            var spec = evm.Specification = evm.SpecificationList.Random();
            var account = evm.Account = evm.AccountList.Random();
            var employee = evm.Employee = evm.EmployeeList.Random();
            var stockCnt = evm.ItemCount = 5;
            var result = evm.Update();

            var iwd = InventoryWrapperDirector.GetInstance();
            //새로이 등록된 재고 데이터를 확인
            Assert.IsTrue(ivm.Items.Any(x => x.Specification.UUID == result.Specification.UUID));
            Assert.IsTrue(iwd.CreateCollection().Any(x => x.Specification.UUID == result.Specification.UUID));
        }

        /// <summary>
        /// Node 선택 후 품목 선택 후 다시 다른 Node를 선택할 경우 나타나는 에러를 구현 -> 구현 실패
        /// </summary>
        [TestMethod]
        public void ClickNodeThenClickItemThenClickNode()
        {
            var dummy = new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            IOStockWrapperViewModel vm = new IOStockWrapperViewModel(StockType.IN, sub);
            IOStockWrapperEditorViewModel evm = new IOStockWrapperEditorViewModel(vm);
            FinderViewModel fvm = evm.CreateFinderViewModel(null);
            var itemNdoes = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM));
            var node = itemNdoes.ElementAt(0);
            //노드 클릭
            fvm.OnNodeSelected(fvm, new SelectionChangedCancelEventArgs(new List<FinderNode>() { node }, new List<FinderNode>()));
            //품목 선택
            evm.Item = evm.ItemList.First();
            //다른 노드 클릭
            fvm.OnNodeSelected(fvm, new SelectionChangedCancelEventArgs(new List<FinderNode>() { itemNdoes.ElementAt(1) }, new List<FinderNode>() { node }));
        }
    }
}