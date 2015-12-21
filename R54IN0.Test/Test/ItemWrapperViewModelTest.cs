﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

namespace R54IN0.Test
{
    [TestClass]
    public class ItemWrapperViewModelTest
    {
        [TestMethod]
        public void CanCreate()
        {
            new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            ItemWrapperViewModel vm = new ItemWrapperViewModel(sub);
        }

        [TestMethod]
        public void LoadItem()
        {
            new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
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
        public void WhenSelectitemLoadNewSpecCollection()
        {
            new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
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
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            ItemWrapperViewModel itemViewModel = new ItemWrapperViewModel(sub);
            ItemWrapper itemWrapper = itemViewModel.SelectedItem = itemViewModel.Items.FirstOrDefault();
            FieldWrapper<Measure> itemMeasure = itemViewModel.SelectedItem.SelectedMeasure;

            var itemMeasureColl = itemWrapper.AllMeasure;
            Assert.AreNotEqual(0, itemMeasureColl.Count());

            FieldWrapperViewModel<Measure, FieldWrapper<Measure>> measureViewModel = new FieldWrapperViewModel<Measure, FieldWrapper<Measure>>(sub);
            var itemsCopy = new List<FieldWrapper<Measure>>(measureViewModel.Items);
            foreach (var measureW in itemsCopy)
            {
                measureViewModel.SelectedItem = measureW;
                measureViewModel.DeleteItemCommand.Execute(null);
            }
            Assert.IsTrue(itemMeasure.IsDeleted);
            Assert.AreEqual(0, itemMeasureColl.Count);
            Assert.AreEqual(itemMeasure, itemWrapper.SelectedMeasure);
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

            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            ItemWrapperViewModel vm = new ItemWrapperViewModel(sub);

            debug = FinderDirector.GetInstance();

            //새로운 item 추가
            vm.AddNewItemCommand.Execute(null);
            var newItemw = vm.Items.Last();

            //파인더 디렉터에서 Finder가 새로이 추가가 되었는지 확인한다.
            var coll = FinderDirector.GetInstance().Collection.SelectMany(x => x.Descendants());
            var result = coll.Where(x => x.ItemUUID == newItemw.UUID).SingleOrDefault();
            Assert.IsNotNull(result);

            //아이템을 삭제하고 파인더 디렉터에서도 동기화가 되었는지 확인한다.
            vm.Remove(newItemw);
            Assert.IsFalse(vm.Items.Contains(newItemw));
            coll = FinderDirector.GetInstance().Collection.SelectMany(x => x.Descendants());
            result = coll.Where(x => x.ItemUUID == newItemw.UUID).SingleOrDefault();
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
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            ItemWrapperViewModel vm = new ItemWrapperViewModel(sub);

            var finderColl = FinderDirector.GetInstance().Collection;

            var item = vm.Items.Last();
            var finderNode = finderColl.SelectMany(x => x.Descendants().Where(y => y.ItemUUID == item.UUID)).Single();

            Assert.AreEqual(item.Name, finderNode.Name);
            item.Name = "Rd12dac#$dkd";
            Assert.AreEqual(item.Name, finderNode.Name);
        }
    }
}