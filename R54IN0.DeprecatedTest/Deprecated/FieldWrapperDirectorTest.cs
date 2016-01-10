using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace R54IN0.Test
{
    [Ignore]
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
            ObservableCollection<IObservableField> itemCollectoin = fwd.CreateCollection<Item>();
            ObservableCollection<IObservableField> specCollectoin = fwd.CreateCollection<Specification>();
            ObservableCollection<IObservableField> measCollectoin = fwd.CreateCollection<Measure>();
            ObservableCollection<IObservableField> currCollectoin = fwd.CreateCollection<Currency>();
            ObservableCollection<IObservableField> eeplCollectoin = fwd.CreateCollection<Employee>();
            ObservableCollection<IObservableField> accoCollectoin = fwd.CreateCollection<Client>();
            ObservableCollection<IObservableField> makeCollectoin = fwd.CreateCollection<Maker>();
            ObservableCollection<IObservableField> wareCollectoin = fwd.CreateCollection<Warehouse>();
        }

        [TestMethod]
        public void LoadWapper2()
        {
            new DummyDbData().Create();
            FieldWrapperDirector fwd = FieldWrapperDirector.GetInstance();
            ObservableCollection<ItemWrapper> itemCollectoin = fwd.CreateCollection<Item, ItemWrapper>();
            ObservableCollection<SpecificationWrapper> specCollectoin = fwd.CreateCollection<Specification, SpecificationWrapper>();
            ObservableCollection<Observable<Measure>> measCollectoin = fwd.CreateCollection<Measure, Observable<Measure>>();
            ObservableCollection<Observable<Currency>> currCollectoin = fwd.CreateCollection<Currency, Observable<Currency>>();
            ObservableCollection<Observable<Employee>> eeplCollectoin = fwd.CreateCollection<Employee, Observable<Employee>>();
            ObservableCollection<ClientWrapper> accoCollectoin = fwd.CreateCollection<Client, ClientWrapper>();
            ObservableCollection<Observable<Maker>> makeCollectoin = fwd.CreateCollection<Maker, Observable<Maker>>();
            ObservableCollection<Observable<Warehouse>> wareCollectoin = fwd.CreateCollection<Warehouse, Observable<Warehouse>>();
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
            ObservableCollection<Observable<Measure>> measCollectoin = fwd.CreateCollection<Measure, Observable<Measure>>();
            ObservableCollection<Observable<Currency>> currCollectoin = fwd.CreateCollection<Currency, Observable<Currency>>();
            ObservableCollection<Observable<Employee>> eeplCollectoin = fwd.CreateCollection<Employee, Observable<Employee>>();
            ObservableCollection<ClientWrapper> accoCollectoin = fwd.CreateCollection<Client, ClientWrapper>();
            ObservableCollection<Observable<Maker>> makeCollectoin = fwd.CreateCollection<Maker, Observable<Maker>>();
            ObservableCollection<Observable<Warehouse>> wareCollectoin = fwd.CreateCollection<Warehouse, Observable<Warehouse>>();

            Random rand = new Random();
            // FieldWrapper<T> 변경테스트
            string eepName = eeplCollectoin.Random().Name = "홍길동.";
            // AccountWrapper 프로퍼티 변경
            ClientWrapper accountw = accoCollectoin.Random();
            string mobile = accountw.MobileNumber = "mobile";
            string accountName = accountw.Name = "newName";
            string phone = accountw.PhoneNumber = "phone";
            // SpecificationWrapper 프로퍼티 변경
            SpecificationWrapper specw = specCollectoin.Random();
            decimal pp = specw.PurchaseUnitPrice = 1000100;
            decimal sp = specw.SalesUnitPrice = 32333;
            string specName = specw.Name = "스펙";
            string specRemark = specw.Remark = "remark!a";
            // ItemWrapper 프로퍼티 변경
            ItemWrapper itemw = itemCollectoin.Where(x => x.Field.ID == specw.Field.ItemID).Single();
            string itemName = itemw.Name = "Item.a";
            var newCurr = itemw.SelectedCurrency = currCollectoin.Random();
            var newMaker = itemw.SelectedMaker = makeCollectoin.Random();
            var newMeasure = itemw.SelectedMeasure = measCollectoin.Random();
            // Director 데이터 소멸
            FieldWrapperDirector.Distroy();
            LexDb.Distroy();
            // Director 데이터 다시 로드
            fwd = FieldWrapperDirector.GetInstance();
            itemCollectoin = fwd.CreateCollection<Item, ItemWrapper>();
            specCollectoin = fwd.CreateCollection<Specification, SpecificationWrapper>();
            measCollectoin = fwd.CreateCollection<Measure, Observable<Measure>>();
            currCollectoin = fwd.CreateCollection<Currency, Observable<Currency>>();
            eeplCollectoin = fwd.CreateCollection<Employee, Observable<Employee>>();
            accoCollectoin = fwd.CreateCollection<Client, ClientWrapper>();
            makeCollectoin = fwd.CreateCollection<Maker, Observable<Maker>>();
            wareCollectoin = fwd.CreateCollection<Warehouse, Observable<Warehouse>>();
            // FieldWrapper<T> 검사
            Assert.IsTrue(eeplCollectoin.Any(x => x.Name == eepName));
            // AccountWrapper 검사
            accountw = accoCollectoin.Where(x => x.ID == accountw.ID).Single();
            Assert.AreEqual(mobile, accountw.MobileNumber);
            Assert.AreEqual(accountName, accountw.Name);
            Assert.AreEqual(phone, accountw.PhoneNumber);
            // SpecificationWrapper 검사
            specw = specCollectoin.Where(x => x.ID == specw.ID).Single();
            Assert.AreEqual(pp, specw.PurchaseUnitPrice);
            Assert.AreEqual(sp, specw.SalesUnitPrice);
            Assert.AreEqual(specName, specw.Name);
            Assert.AreEqual(specRemark, specw.Remark);
            // ItemWrapper 검사
            itemw = itemCollectoin.Where(x => x.ID == itemw.ID).Single();
            Assert.AreEqual(itemName, itemw.Name);
            Assert.AreEqual(newCurr.ID, itemw.SelectedCurrency.ID);
            Assert.AreEqual(newMaker.ID, itemw.SelectedMaker.ID);
            Assert.AreEqual(newMeasure.ID, itemw.SelectedMeasure.ID);
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
            FieldWrapperViewModel<Employee, Observable<Employee>> vm1 = new FieldWrapperViewModel<Employee, Observable<Employee>>(sub);
            FieldWrapperViewModel<Employee, Observable<Employee>> vm2 = new FieldWrapperViewModel<Employee, Observable<Employee>>(sub);

            //새로운 아이템을 컬렉션에 추가 후 동기화(Update) 확인
            Employee newEmp = new Employee();
            Observable<Employee> newEmpW = new Observable<Employee>(newEmp);
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