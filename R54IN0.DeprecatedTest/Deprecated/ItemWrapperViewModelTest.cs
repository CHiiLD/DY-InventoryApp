using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

namespace R54IN0.Test
{
    [Ignore]
    [TestClass]
    public class ItemWrapperViewModelTest
    {
        [TestMethod]
        public void CanCreate()
        {
            new DummyDbData().Create();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemWrapperViewModel vm = new ItemWrapperViewModel(sub);
        }

        [TestMethod]
        public void LoadItem()
        {
            new DummyDbData().Create();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemWrapperViewModel vm = new ItemWrapperViewModel(sub);
            Assert.IsNotNull(vm.Items);

            if (vm.Items.Count != 0)
            {
                var item = vm.Items.First();
                vm.SelectedItem = item;
                Assert.IsNotNull(vm.Specifications);
            }
        }

        [TestMethod]
        public void WhenSelectitemThenLoadNewSpecCollection()
        {
            new DummyDbData().Create();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemWrapperViewModel vm = new ItemWrapperViewModel(sub);

            var beforeSpec = vm.Specifications;
            vm.SelectedItem = vm.Items.LastOrDefault();
            var afterSpec = vm.Specifications;
            Assert.AreNotEqual(beforeSpec, afterSpec);
        }

        /// <summary>
        /// ItemWrapperViewModel에 임의이 아이템 객체 하나를 설렉션아이템에 넣고 MeasureWrapper 객체를 구한다.
        /// FieldWrapperViewModel<Measure> 뷰모델을 사용하여 모든 Measure 객체를 사용안함으로 변경한다.
        /// ItemWrapperViewModel에서 제공하는 AllMeasure의 아이템의 개수는 제로가 되어야 한다.
        /// </summary>
        [TestMethod]
        public void PropertySync()
        {
            new DummyDbData().Create();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemWrapperViewModel itemViewModel = new ItemWrapperViewModel(sub);
            ItemWrapper itemWrapper = itemViewModel.SelectedItem = itemViewModel.Items.FirstOrDefault();
            Observable<Measure> itemMeasure = itemViewModel.SelectedItem.SelectedMeasure;

            var itemMeasureColl = itemWrapper.AllMeasure;
            Assert.AreNotEqual(0, itemMeasureColl.Count());

            FieldWrapperViewModel<Measure, Observable<Measure>> measureViewModel = new FieldWrapperViewModel<Measure, Observable<Measure>>(sub);
            var itemsCopy = new List<Observable<Measure>>(measureViewModel.Items);
            foreach (var measureW in itemsCopy)
            {
                measureViewModel.SelectedItem = measureW;
                measureViewModel.DeleteItemCommand.Execute(null);
            }
            Assert.IsTrue(itemMeasure.IsDeleted);
            Assert.AreEqual(0, itemMeasureColl.Count);
            Assert.IsNull(itemWrapper.SelectedMeasure);
        }

        /// <summary>
        /// 새로운 아이템을 추가할 경우 FinderDirector 에서도 새로운 아이템이 추가되어야 한다.
        /// </summary>
        [TestMethod]
        public void SyncFinderNodeTree()
        {
            var dummy = new DummyDbData().Create();
            var debug = FinderDirector.GetInstance();
            dummy = new DummyDbData().Create();

            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemWrapperViewModel vm = new ItemWrapperViewModel(sub);

            debug = FinderDirector.GetInstance();

            //새로운 item 추가
            vm.AddNewItemCommand.Execute(null);
            var newItemw = vm.Items.Last();

            //파인더 디렉터에서 Finder가 새로이 추가가 되었는지 확인한다.
            var coll = FinderDirector.GetInstance().Collection.SelectMany(x => x.Descendants());
            var result = coll.Where(x => x.ItemID == newItemw.ID).SingleOrDefault();
            Assert.IsNotNull(result);

            //아이템을 삭제하고 파인더 디렉터에서도 동기화가 되었는지 확인한다.
            vm.Remove(newItemw);
            Assert.IsFalse(vm.Items.Contains(newItemw));
            coll = FinderDirector.GetInstance().Collection.SelectMany(x => x.Descendants());
            result = coll.Where(x => x.ItemID == newItemw.ID).SingleOrDefault();
            Assert.IsNull(result);
        }

        /// <summary>
        /// 기존 아이템의 이름을 변경한 경우 Finder에도 똑같이 동기화되어 이름이 변경된다.
        /// </summary>
        [TestMethod]
        public void SyncFinderNodeNameProperty()
        {
            new DummyDbData().Create();
            var finder = FinderDirector.GetInstance();

            var dummy = new DummyDbData().Create();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemWrapperViewModel vm = new ItemWrapperViewModel(sub);

            var finderColl = FinderDirector.GetInstance().Collection;

            var item = vm.Items.Last();
            var finderNode = finderColl.SelectMany(x => x.Descendants().Where(y => y.ItemID == item.ID)).Single();

            Assert.AreEqual(item.Name, finderNode.Name);
            item.Name = "Rd12dac#$dkd";
            Assert.AreEqual(item.Name, finderNode.Name);
        }

        /// <summary>
        /// Sepected Property 들이 전부 Null이 들어간 경우를 상정할 떄 예외가 없어야 한다.
        /// </summary>
        [TestMethod]
        public void SyncWrapperProperty()
        {
            new DummyDbData().Create();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();

            ItemWrapperViewModel vm = new ItemWrapperViewModel(sub);

            FieldWrapperViewModel<Maker, Observable<Maker>> mvm = new FieldWrapperViewModel<Maker, Observable<Maker>>(sub);
            FieldWrapperViewModel<Currency, Observable<Currency>> cvm = new FieldWrapperViewModel<Currency, Observable<Currency>>(sub);
            FieldWrapperViewModel<Measure, Observable<Measure>> msvm = new FieldWrapperViewModel<Measure, Observable<Measure>>(sub);

            foreach (var item in new List<Observable<Maker>>(mvm.Items))
            {
                mvm.SelectedItem = item;
                mvm.DeleteItemCommand.Execute(null);
            }

            foreach (var item in new List<Observable<Currency>>(cvm.Items))
            {
                cvm.SelectedItem = item;
                cvm.DeleteItemCommand.Execute(null);
            }

            foreach (var item in new List<Observable<Measure>>(msvm.Items))
            {
                msvm.SelectedItem = item;
                msvm.DeleteItemCommand.Execute(null);
            }

            foreach (var item in vm.Items)
            {
                Assert.IsNull(item.SelectedCurrency);
                Assert.IsNull(item.SelectedMaker);
                Assert.IsNull(item.SelectedMeasure);

                Assert.AreEqual(0, item.AllCurrency.Count);
                Assert.AreEqual(0, item.AllMaker.Count);
                Assert.AreEqual(0, item.AllMeasure.Count);
            }
        }

        /// <summary>
        /// Finder에서 아이템노드를 하나 선택하였을 경우 
        /// 품목과 규격 컬렉션이 업데이트 되어야 한다.
        /// </summary>
        [TestMethod]
        public void WhenClickFinderItemThenUpdateSpecifications()
        {
            new DummyDbData().Create();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemFinderViewModel fvm = new ItemFinderViewModel(null);
            ItemWrapperViewModel vm = new ItemWrapperViewModel(sub);
            fvm.SelectItemsChanged += vm.OnFinderViewSelectItemChanged;
            //finder 에서 아이템 선택
            fvm.SelectedNodes.Clear();
            var node = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).First();
            fvm.OnNodeSelected(fvm, new System.Windows.Controls.SelectionChangedCancelEventArgs(
                new List<FinderNode>() { node }, new List<FinderNode>()));

            //아이템과 규격 컬렉션의 동기화 확인
            Assert.IsTrue(vm.Items.All(x => x.ID == node.ItemID));
            Assert.IsTrue(vm.Specifications.All(x => x.Field.ItemID == node.ItemID));
        }

        /// <summary>
        /// 아이템을 새로 추가한 경우 해당 아이템에 finder포커스가 가고 
        /// 새로운 품목 아이템이 추가되며 새로운 규격이 추가되어야 한다.
        /// </summary>
        [TestMethod]
        public void ClickNewAddItemButtonThenWork()
        {
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemFinderViewModel fvm = new ItemFinderViewModel(null);
            ItemWrapperViewModel vm = new ItemWrapperViewModel(sub);
            fvm.SelectItemsChanged += vm.OnFinderViewSelectItemChanged;
            vm.FinderViewModel = fvm;

            fvm.OnNodeSelected(null, 
                new System.Windows.Controls.SelectionChangedCancelEventArgs(new List<FinderNode>() { fvm.Nodes.First() }, new List<FinderNode>()));

            vm.SelectedItem = null;
            vm.SelectedSpecification = null;

            vm.AddNewItemCommand.Execute(null);

            var node = fvm.SelectedNodes.Single();
            Assert.AreEqual(node.ItemID, vm.SelectedItem.ID);
        }
    }
}