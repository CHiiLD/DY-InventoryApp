using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace R54IN0.Test
{
    [TestClass]
    public class FieldWrapperDirectorTest
    {
        [TestMethod]
        public void CanCreate()
        {
            FieldWrapperDirector fwd = FieldWrapperDirector.GetInstance();
        }

        [TestMethod]
        public void CanDistory()
        {
            FieldWrapperDirector fwd = FieldWrapperDirector.GetInstance();
            FieldWrapperDirector.Distroy();
        }

        /// <summary>
        /// IFieldWrapper 객체들을 제너릭 타입에 맞게 데이터베이스에서 쿼리하여 Wrapper 클래스에 장식되어 
        /// 컬렉션으로 호출 가능 여부의 테스트
        /// </summary>
        [TestMethod]
        public void LoadWapper()
        {
            new DummyDbData().Create();
            FieldWrapperDirector fwd = FieldWrapperDirector.GetInstance();
            ObservableCollection<IFieldWrapper> itemCollectoin = fwd.CreateCollection<Item>();
            ObservableCollection<IFieldWrapper> specCollectoin = fwd.CreateCollection<Specification>();
            ObservableCollection<IFieldWrapper> measCollectoin = fwd.CreateCollection<Measure>();
            ObservableCollection<IFieldWrapper> currCollectoin = fwd.CreateCollection<Currency>();
            ObservableCollection<IFieldWrapper> eeplCollectoin = fwd.CreateCollection<Employee>();
            ObservableCollection<IFieldWrapper> accoCollectoin = fwd.CreateCollection<Client>();
            ObservableCollection<IFieldWrapper> makeCollectoin = fwd.CreateCollection<Maker>();
            ObservableCollection<IFieldWrapper> wareCollectoin = fwd.CreateCollection<Warehouse>();
        }

        [TestMethod]
        public void LoadWapper2()
        {
            new DummyDbData().Create();
            FieldWrapperDirector fwd = FieldWrapperDirector.GetInstance();
            ObservableCollection<ItemWrapper> itemCollectoin = fwd.CreateCollection<Item, ItemWrapper>();
            ObservableCollection<SpecificationWrapper> specCollectoin = fwd.CreateCollection<Specification, SpecificationWrapper>();
            ObservableCollection<FieldWrapper<Measure>> measCollectoin = fwd.CreateCollection<Measure, FieldWrapper<Measure>>();
            ObservableCollection<FieldWrapper<Currency>> currCollectoin = fwd.CreateCollection<Currency, FieldWrapper<Currency>>();
            ObservableCollection<FieldWrapper<Employee>> eeplCollectoin = fwd.CreateCollection<Employee, FieldWrapper<Employee>>();
            ObservableCollection<ClientWrapper> accoCollectoin = fwd.CreateCollection<Client, ClientWrapper>();
            ObservableCollection<FieldWrapper<Maker>> makeCollectoin = fwd.CreateCollection<Maker, FieldWrapper<Maker>>();
            ObservableCollection<FieldWrapper<Warehouse>> wareCollectoin = fwd.CreateCollection<Warehouse, FieldWrapper<Warehouse>>();
        }

        /// <summary>
        /// 각 필드 랩퍼 클래스들의 속성을 변경하였을 때 데이터베이스에 자동적으로 업데이트 되는가 여부의 테스트
        /// </summary>
        [TestMethod]
        public void ChangeProperty()
        {
            new DummyDbData().Create();
            FieldWrapperDirector fwd = FieldWrapperDirector.GetInstance();
            ObservableCollection<ItemWrapper> itemCollectoin = fwd.CreateCollection<Item, ItemWrapper>();
            ObservableCollection<SpecificationWrapper> specCollectoin = fwd.CreateCollection<Specification, SpecificationWrapper>();
            ObservableCollection<FieldWrapper<Measure>> measCollectoin = fwd.CreateCollection<Measure, FieldWrapper<Measure>>();
            ObservableCollection<FieldWrapper<Currency>> currCollectoin = fwd.CreateCollection<Currency, FieldWrapper<Currency>>();
            ObservableCollection<FieldWrapper<Employee>> eeplCollectoin = fwd.CreateCollection<Employee, FieldWrapper<Employee>>();
            ObservableCollection<ClientWrapper> accoCollectoin = fwd.CreateCollection<Client, ClientWrapper>();
            ObservableCollection<FieldWrapper<Maker>> makeCollectoin = fwd.CreateCollection<Maker, FieldWrapper<Maker>>();
            ObservableCollection<FieldWrapper<Warehouse>> wareCollectoin = fwd.CreateCollection<Warehouse, FieldWrapper<Warehouse>>();

            Random rand = new Random();
            // FieldWrapper<T> 변경테스트
            string eepName = eeplCollectoin.ElementAtOrDefault(rand.Next(eeplCollectoin.Count - 1)).Name = "홍길동.";
            // AccountWrapper 프로퍼티 변경
            ClientWrapper accountw = accoCollectoin.ElementAt(rand.Next(accoCollectoin.Count - 1));
            string mobile = accountw.MobileNumber = "mobile";
            string accountName = accountw.Name = "newName";
            string phone = accountw.PhoneNumber = "phone";
            // SpecificationWrapper 프로퍼티 변경
            SpecificationWrapper specw = specCollectoin.ElementAt(rand.Next(specCollectoin.Count - 1));
            decimal pp = specw.PurchaseUnitPrice = 1000100;
            decimal sp = specw.SalesUnitPrice = 32333;
            string specName = specw.Name = "스펙";
            string specRemark = specw.Remark = "remark!a";
            // ItemWrapper 프로퍼티 변경
            ItemWrapper itemw = itemCollectoin.Where(x => x.Field.UUID == specw.Field.ItemUUID).Single();
            string itemName = itemw.Name = "Item.a";
            var newCurr = itemw.SelectedCurrency = currCollectoin.ElementAt(rand.Next(currCollectoin.Count - 1));
            var newMaker = itemw.SelectedMaker = makeCollectoin.ElementAt(rand.Next(makeCollectoin.Count - 1));
            var newMeasure = itemw.SelectedMeasure = measCollectoin.ElementAt(rand.Next(measCollectoin.Count - 1));
            // Director 데이터 소멸
            FieldWrapperDirector.Distroy();
            DatabaseDirector.Distroy();
            // Director 데이터 다시 로드
            fwd = FieldWrapperDirector.GetInstance();
            itemCollectoin = fwd.CreateCollection<Item, ItemWrapper>();
            specCollectoin = fwd.CreateCollection<Specification, SpecificationWrapper>();
            measCollectoin = fwd.CreateCollection<Measure, FieldWrapper<Measure>>();
            currCollectoin = fwd.CreateCollection<Currency, FieldWrapper<Currency>>();
            eeplCollectoin = fwd.CreateCollection<Employee, FieldWrapper<Employee>>();
            accoCollectoin = fwd.CreateCollection<Client, ClientWrapper>();
            makeCollectoin = fwd.CreateCollection<Maker, FieldWrapper<Maker>>();
            wareCollectoin = fwd.CreateCollection<Warehouse, FieldWrapper<Warehouse>>();
            // FieldWrapper<T> 검사
            Assert.IsTrue(eeplCollectoin.Any(x => x.Name == eepName));
            // AccountWrapper 검사
            accountw = accoCollectoin.Where(x => x.UUID == accountw.UUID).Single();
            Assert.AreEqual(mobile, accountw.MobileNumber);
            Assert.AreEqual(accountName, accountw.Name);
            Assert.AreEqual(phone, accountw.PhoneNumber);
            // SpecificationWrapper 검사
            specw = specCollectoin.Where(x => x.UUID == specw.UUID).Single();
            Assert.AreEqual(pp, specw.PurchaseUnitPrice);
            Assert.AreEqual(sp, specw.SalesUnitPrice);
            Assert.AreEqual(specName, specw.Name);
            Assert.AreEqual(specRemark, specw.Remark);
            // ItemWrapper 검사
            itemw = itemCollectoin.Where(x => x.UUID == itemw.UUID).Single();
            Assert.AreEqual(itemName, itemw.Name);
            Assert.AreEqual(newCurr.UUID, itemw.SelectedCurrency.UUID);
            Assert.AreEqual(newMaker.UUID, itemw.SelectedMaker.UUID);
            Assert.AreEqual(newMeasure.UUID, itemw.SelectedMeasure.UUID);
        }

        /// <summary>
        /// Collection에 필드 랩핑 객체를 새로 생성하여 추가하는 경우, 또는 객체를 삭제하는 경우 
        /// CreateFieldWrapperCollection 메서드에서 받은 컬렉션의 아이템들을 동일하게 유지하여야 한다.
        /// 또한 FieldWrapperViewModelObserver 클래스는 AddNewItem과 RemoveItem메서드를 실행할 때 
        /// 자동적으로 FieldWrapperCollection 클래스의 아이템과 같이 연동을 한다.
        /// </summary>
        [TestMethod]
        public void AsyncCollection()
        {
            new DummyDbData().Create();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            FieldWrapperViewModel<Employee, FieldWrapper<Employee>> vm1 = new FieldWrapperViewModel<Employee, FieldWrapper<Employee>>(sub);
            FieldWrapperViewModel<Employee, FieldWrapper<Employee>> vm2 = new FieldWrapperViewModel<Employee, FieldWrapper<Employee>>(sub);

            //새로운 아이템을 컬렉션에 추가 후 동기화(Update) 확인
            Employee newEmp = new Employee();
            FieldWrapper<Employee> newEmpW = new FieldWrapper<Employee>(newEmp);
            newEmpW.Name = "홍길동.3";
            vm1.Add(newEmpW);
            Assert.IsTrue(vm2.Items.Contains(newEmpW));
            Assert.IsFalse(newEmpW.IsDeleted);
            Assert.IsTrue(FieldWrapperDirector.GetInstance().Contains<Employee>(newEmpW));
            //아이템을 삭제 후 컬렉션 동기화 확인
            vm1.Remove(newEmpW);
            Assert.IsFalse(vm2.Items.Contains(newEmpW));
            Assert.IsTrue(newEmpW.IsDeleted);
            Assert.IsTrue(FieldWrapperDirector.GetInstance().Contains<Employee>(newEmpW));
        }
    }
}