﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;

namespace R54IN0.Test
{
    [TestClass]
    public class InventoryWrapperViewModelTest
    {
        [TestMethod]
        public void CanCreate()
        {
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            InventoryWrapperViewModel vm = new InventoryWrapperViewModel(sub);
        }

        /// <summary>
        /// 여러 InventoryWrapperViewModel이 있으면 아이템들을 동기화 시킨다.
        /// </summary>
        [TestMethod]
        public void SyncCollections()
        {
            var dummy = new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            InventoryWrapperViewModel vm1 = new InventoryWrapperViewModel(sub);
            InventoryWrapperViewModel vm2 = new InventoryWrapperViewModel(sub);

            var itemws = FieldWrapperDirector.GetInstance().CreateCollection<Item, ItemWrapper>();
            var itemw = itemws.Where(x => x.UUID == dummy.TestItemUUID).Single();
            var specws = FieldWrapperDirector.GetInstance().CreateCollection<Specification, SpecificationWrapper>();

            Inventory inven = new Inventory();
            InventoryWrapper invenw = new InventoryWrapper(inven);
            invenw.Item = itemw;
            invenw.Specification = specws.Where(x => x.Field.ItemUUID == itemw.UUID).First();
            invenw.ItemCount = 1010;
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
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            FinderViewModel fvm = new FinderViewModel(null);
            InventoryWrapperViewModel iwvm = new InventoryWrapperViewModel(sub);
            fvm.SelectItemsChanged += iwvm.OnFinderViewSelectItemChanged;

            //FinderViewModel에 아이템 하나를 선택
            Assert.AreEqual(0, fvm.SelectedNodes.Count);
            FinderNode node = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM)).FirstOrDefault();
            Assert.IsNotNull(node);
            var list = new List<FinderNode>() { node };
            fvm.OnSelectNodes(fvm, new System.Windows.Controls.SelectionChangedCancelEventArgs(list, new List<FinderNode>()));
            Assert.AreEqual(1, fvm.SelectedNodes.Count);

            Assert.IsTrue(iwvm.Items.All(x=>x.Item.UUID == node.ItemUUID));

            //FinderViewModel에 여러개의 아이템을 선택
            FinderNode node2 = fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM)).LastOrDefault();
            Assert.AreNotEqual(node, node2);
            var list2 = new List<FinderNode>() { node, node2 };
            fvm.OnSelectNodes(fvm, new System.Windows.Controls.SelectionChangedCancelEventArgs(list2, list));
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

            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
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
            ObservableCollection<AccountWrapper> accoCollectoin = fwd.CreateCollection<Account, AccountWrapper>();
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
    }
} 