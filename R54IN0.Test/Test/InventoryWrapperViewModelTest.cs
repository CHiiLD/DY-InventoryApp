using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Controls;

namespace R54IN0.Test
{
    [TestClass]
    public class InventoryWrapperViewModelTest
    {
        [TestMethod]
        public void CanCreate()
        {
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            InventoryWrapperViewModel vm = new InventoryWrapperViewModel(sub);
        }

        /// <summary>
        /// 여러 InventoryWrapperViewModel이 있으면 아이템들을 동기화 시킨다.
        /// </summary>
        [TestMethod]
        public void SyncCollections()
        {
            var dummy = new DummyDbData().Create();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            InventoryWrapperViewModel vm1 = new InventoryWrapperViewModel(sub);
            InventoryWrapperViewModel vm2 = new InventoryWrapperViewModel(sub);

            var itemws = FieldWrapperDirector.GetInstance().CreateCollection<Item, ItemWrapper>();
            var itemw = itemws.Where(x => x.UUID == dummy.UnregisterdTestItemUUID).Single();
            var specws = FieldWrapperDirector.GetInstance().CreateCollection<Specification, SpecificationWrapper>();

            Inventory inven = new Inventory();
            InventoryWrapper invenw = new InventoryWrapper(inven);
            invenw.Item = itemw;
            invenw.Specification = specws.Where(x => x.Field.ItemUUID == itemw.UUID).First();
            invenw.Quantity = 1010;
            invenw.Remark = "hehehehe";

            //새로 추가 테스트
            Assert.IsFalse(vm2.Items.Contains(invenw));
            vm1.Add(invenw);
            Assert.IsTrue(vm2.Items.Contains(invenw));

            //삭제 테스트
            vm1.Remove(invenw);
            Assert.IsFalse(vm2.Items.Contains(invenw));
        }

        /// <summary>
        /// Finder 에서 아이템을 하나 선택하였을 때
        /// Items를 새로 업데이트한다.
        /// </summary>
        [TestMethod]
        public void FinderSelectItemsEvent()
        {
            new DummyDbData().Create();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemFinderViewModel fvm = new ItemFinderViewModel(null);
            InventoryWrapperViewModel iwvm = new InventoryWrapperViewModel(sub);
            fvm.SelectItemsChanged += iwvm.OnFinderViewSelectItemChanged;

            //FinderViewModel에 아이템 하나를 선택
            Assert.AreEqual(0, fvm.SelectedNodes.Count);
            FinderNode node = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM)).FirstOrDefault();
            Assert.IsNotNull(node);
            var list = new List<FinderNode>() { node };
            fvm.OnNodeSelected(fvm, new SelectionChangedCancelEventArgs(list, new List<FinderNode>()));
            Assert.AreEqual(1, fvm.SelectedNodes.Count);

            Assert.IsTrue(iwvm.Items.All(x => x.Item.UUID == node.ItemUUID));

            //FinderViewModel에 여러개의 아이템을 선택
            FinderNode node2 = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM)).LastOrDefault();
            Assert.AreNotEqual(node, node2);
            var list2 = new List<FinderNode>() { node, node2 };
            fvm.OnNodeSelected(fvm, new System.Windows.Controls.SelectionChangedCancelEventArgs(list2, list));
            Assert.AreEqual(2, fvm.SelectedNodes.Count);

            Assert.IsTrue(iwvm.Items.All(x => x.Item.UUID == node.ItemUUID || x.Item.UUID == node2.ItemUUID));
        }

        /// <summary>
        /// ItemWrapperViewModel 에서 아이템과 스펙을 변경하였을 때 InventoryWrapperViewModel의 아이템 또한 동기화되어야 한다.
        /// </summary>
        [TestMethod]
        public void SyncItemWrapperViewModel()
        {
            new DummyDbData().Create();

            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            InventoryWrapperViewModel iwvm = new InventoryWrapperViewModel(sub);
            ItemWrapperViewModel ivm = new ItemWrapperViewModel(sub);

            var invenw = iwvm.Items.ElementAt(new Random().Next(iwvm.Items.Count - 1));
            var itemw = ivm.SelectedItem = ivm.Items.Where(x => x.UUID == invenw.Item.UUID).Single();
            var specw = ivm.SelectedSpecification = ivm.Specifications.Where(x => x.UUID == invenw.Specification.UUID).Single();

            Random r = new Random();
            var fwd = FieldWrapperDirector.GetInstance();
            ObservableCollection<FieldWrapper<Measure>> measCollectoin = fwd.CreateCollection<Measure, FieldWrapper<Measure>>();
            ObservableCollection<FieldWrapper<Currency>> currCollectoin = fwd.CreateCollection<Currency, FieldWrapper<Currency>>();
            ObservableCollection<FieldWrapper<Employee>> eeplCollectoin = fwd.CreateCollection<Employee, FieldWrapper<Employee>>();
            ObservableCollection<ClientWrapper> accoCollectoin = fwd.CreateCollection<Client, ClientWrapper>();
            ObservableCollection<FieldWrapper<Maker>> makeCollectoin = fwd.CreateCollection<Maker, FieldWrapper<Maker>>();
            ObservableCollection<FieldWrapper<Warehouse>> wareCollectoin = fwd.CreateCollection<Warehouse, FieldWrapper<Warehouse>>();

            itemw.Name = "멘도롱또똣";
            itemw.SelectedCurrency = currCollectoin.Random();
            itemw.SelectedMeasure = measCollectoin.Random();
            itemw.SelectedMaker = makeCollectoin.Random();

            specw.Name = "미지근하게 따끈하다";
            specw.PurchaseUnitPrice = 111203;
            specw.SalesUnitPrice = 222932;
            specw.Remark = "뿌잉뿌잉";

            Assert.AreEqual(itemw.Name, invenw.Item.Name);
            Assert.AreEqual(itemw.SelectedCurrency, invenw.Currency);
            Assert.AreEqual(itemw.SelectedMeasure, invenw.Measure);
            Assert.AreEqual(itemw.SelectedMaker, invenw.Maker);

            Assert.AreEqual(specw.Name, invenw.Specification.Name);
            Assert.AreEqual(specw.PurchaseUnitPrice, invenw.PurchaseUnitPrice);
            Assert.AreEqual(specw.SalesUnitPrice, invenw.SalesUnitPrice);
            Assert.AreEqual(specw.Remark, invenw.Remark);
        }

        [TestMethod]
        public void CanDelete()
        {
            new DummyDbData().Create();

            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            InventoryWrapperViewModel ivm = new InventoryWrapperViewModel(sub);
            InventoryWrapperViewModel ivm2 = new InventoryWrapperViewModel(sub);

            var selectedItem = ivm.SelectedItem = ivm.Items.Random();

            Assert.IsTrue(ivm.Items.Any(x => x == selectedItem));
            Assert.IsTrue(ivm.Items.Any(x => x == selectedItem));

            ivm.RemoveCommand.Execute(null);

            Assert.IsNull(ivm.SelectedItem);
            Assert.IsTrue(ivm.Items.All(x => x != selectedItem));
            Assert.IsTrue(ivm.Items.All(x => x != selectedItem));
            InventoryWrapperDirector iwd = InventoryWrapperDirector.GetInstance();
            Assert.IsFalse(iwd.Contains(selectedItem));
        }

        /// <summary>
        /// 에러8번 
        /// </summary>
        [TestMethod]
        public void CreateItemButNotSetMakerThenCreateIOStockThenAgainLoad()
        {
            //모든 디비 데이터 삭제
            using (var db = DatabaseDirector.GetDbInstance())
            {
                db.Purge();
            }
            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            CollectionViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            StockWrapperDirector.Distory();

            FieldWrapperDirector fwd = FieldWrapperDirector.GetInstance();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemWrapperViewModel ivm = new ItemWrapperViewModel(sub);
            StockWrapperViewModel svm = new StockWrapperViewModel(StockType.INCOMING, sub);
            InventoryWrapperViewModel invm = new InventoryWrapperViewModel(sub);
            //아이템 새로 생성 하지만 Maker 프로퍼티는 설정 하지 아니함
            ivm.AddNewItemCommand.Execute(null);
            Assert.IsFalse(fwd.CreateCollection<Specification, SpecificationWrapper>().Any(x => x.Field.ItemUUID == null));
            //입고 데이터 생성 .. 재고에 데이터가 없을테니 재고도 같이 생성됨
            StockWrapperEditorViewModel evm = new StockWrapperEditorViewModel(svm);
            //FINDER 생성
            FinderViewModel fvm = evm.CreateFinderViewModel(null);
            //생성한 아이템을 선택하도록함
            fvm.OnNodeSelected(fvm, new SelectionChangedCancelEventArgs(
                new List<FinderNode>() { fvm.Nodes.Last() }, new List<FinderNode>()));
            var itemw = evm.Item = evm.ItemList.First();
            var specw = evm.Specification = evm.SpecificationList.First();
            //설정한 Stock 데이터 저장
            StockWrapper savedData = evm.Update();
            //디렉터 파괴
            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            CollectionViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            StockWrapperDirector.Distory();

            //다시 로드
            fwd = FieldWrapperDirector.GetInstance();
            sub = CollectionViewModelObserverSubject.GetInstance();
            ivm = new ItemWrapperViewModel(sub);
            svm = new StockWrapperViewModel(StockType.INCOMING, sub);
            invm = new InventoryWrapperViewModel(sub);

            //품목의 여러 프로퍼티 호출 
            itemw = ivm.Items.Where(x => x.UUID == itemw.UUID).Single();
            var ac = itemw.AllCurrency;
            var ac1 = itemw.AllMaker;
            var ac2 = itemw.AllMeasure;
            Assert.IsNull(itemw.SelectedCurrency);
            Assert.IsNull(itemw.SelectedMaker);
            Assert.IsNull(itemw.SelectedMeasure);

            var invenw = invm.Items.Where(x => x.Specification.UUID == specw.UUID).Single();
            Assert.IsNull(invenw.Maker);
            Assert.IsNull(invenw.Measure);
            Assert.IsNull(invenw.Currency);
        }

        /// <summary>
        /// 새로운 아이템을 만듬
        /// 새로운 아이템으로 새로운 재고 데이터를 등록한다
        /// 재고 리스트에 제대로 업데이트 되어 있는지 확인한다.
        /// </summary>
        [TestMethod]
        public void UpdateNewItem()
        {
            new DummyDbData().Create();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            //아이템 생성 
            ItemWrapperViewModel itemvm = new ItemWrapperViewModel(sub);
            itemvm.AddNewItemCommand.Execute(null);
            var newItem = itemvm.Items.Last();
            var newSpec = itemvm.Specifications.Last();
            //Finder에서 선택
            InventoryWrapperViewModel ivm = new InventoryWrapperViewModel(sub);
            FinderViewModel fvm = ivm.CreateFinderViewModel(null);

            var list = new List<FinderNode>() { fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.ItemUUID == newItem.UUID)).Single() };
            fvm.OnNodeSelected(fvm, new SelectionChangedCancelEventArgs(list, new List<FinderNode>()));

            InventoryWrapperEditorViewModel ievm = new InventoryWrapperEditorViewModel(ivm);
            ievm.Item = newItem;
            ievm.Specification = newSpec;
            ievm.Quantity = 100;
            ievm.Update();

            Assert.AreEqual(1, ivm.Items.Count);
            Assert.IsTrue(ivm.Items.First().Item.UUID == newItem.UUID);
        }
    }
}