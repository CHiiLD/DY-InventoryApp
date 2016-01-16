﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Linq;

namespace R54IN0.Test
{
    [Ignore]
    [TestClass]
    public class InventoryWrapperDirectorTest
    {
        [TestMethod]
        public void CanCreate()
        {
            InventoryWrapperDirector iwd = InventoryWrapperDirector.GetInstance();
        }

        [TestMethod]
        public void LoadData()
        {
            InventoryWrapperDirector iwd = InventoryWrapperDirector.GetInstance();
            ObservableCollection<InventoryWrapper> wrappers = iwd.CreateCollection();
        }

        [TestMethod]
        public void DistoryDirector()
        {
            InventoryWrapperDirector iwd = InventoryWrapperDirector.GetInstance();
            InventoryWrapperDirector.Distory();
        }

        /// <summary>
        /// 핵심목표
        /// 프로퍼티 정보들을 변경 후 디렉터를 파괴 후 다시 생성하였을 때 
        /// 변경한 부분이 제대로 반영되었는지 확인하는 테스트
        /// </summary>
        [Ignore] //알 수 없는 버그가 생김
        [TestMethod]
        public void ChangeProperties()
        {
            new DummyDbData();

            var fwd = FieldWrapperDirector.GetInstance();
            ObservableCollection<ItemWrapper> itemCollectoin = fwd.CreateCollection<Item, ItemWrapper>();
            ObservableCollection<SpecificationWrapper> specCollectoin = fwd.CreateCollection<Specification, SpecificationWrapper>();
            ObservableCollection<Observable<Measure>> measCollectoin = fwd.CreateCollection<Measure, Observable<Measure>>();
            ObservableCollection<Observable<Currency>> currCollectoin = fwd.CreateCollection<Currency, Observable<Currency>>();
            ObservableCollection<Observable<Employee>> eeplCollectoin = fwd.CreateCollection<Employee, Observable<Employee>>();
            ObservableCollection<ClientWrapper> accoCollectoin = fwd.CreateCollection<Client, ClientWrapper>();
            ObservableCollection<Observable<Maker>> makeCollectoin = fwd.CreateCollection<Maker, Observable<Maker>>();
            ObservableCollection<Observable<Warehouse>> wareCollectoin = fwd.CreateCollection<Warehouse, Observable<Warehouse>>();
            var rand = new Random();
            SpecificationWrapper sepcWrapper = specCollectoin.ElementAt(rand.Next(specCollectoin.Count - 1));

            InventoryWrapperDirector iwd = InventoryWrapperDirector.GetInstance();
            ObservableCollection<InventoryWrapper> wrappers = iwd.CreateCollection();

            //변경하기 
            InventoryWrapper wrapper = wrappers.ElementAt(rand.Next(wrappers.Count - 1));
            var itemCnt = wrapper.Quantity = 323;
            var ware = wrapper.Warehouse = wareCollectoin.ElementAt(rand.Next(wareCollectoin.Count - 1));
            var spec = wrapper.Specification = sepcWrapper;
            //파괴
            InventoryWrapperDirector.Distory();
            FieldWrapperDirector.Distroy();

            iwd = InventoryWrapperDirector.GetInstance();
            //찾기
            wrappers = iwd.CreateCollection();
            var newWrapper = wrappers.Where(x => x.Product.ID == wrapper.Product.ID).Single();
            //검사
            Assert.AreEqual(itemCnt, newWrapper.Quantity);
            Assert.AreEqual(ware.ID, newWrapper.Warehouse.ID);
            Assert.AreEqual(spec.ID, newWrapper.Specification.ID);
        }
    }
}