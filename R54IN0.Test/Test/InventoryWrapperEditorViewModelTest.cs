using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;
using System.Collections.Generic;

namespace R54IN0.Test
{
    [TestClass]
    public class InventoryWrapperEditorViewModelTest
    {
        [TestMethod]
        public void CanCreate()
        {
            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            CollectionViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            StockWrapperDirector.Distory();
            using (var db = LexDb.GetDbInstance())
            {
                db.Purge();
            }
            LexDb.Distroy();

            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            InventoryWrapperViewModel iwvm = new InventoryWrapperViewModel(sub);
            InventoryWrapperEditorViewModel helper = new InventoryWrapperEditorViewModel(iwvm);
        }

        /// <summary>
        /// 아이템즈 프로퍼티를 변경하기 위해 Editor 클래스를 사용하고 적용되었는지 확인한다.
        /// </summary>
        [TestMethod]
        public void EditItem()
        {
            new DummyDbData().Create();

            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            InventoryWrapperViewModel iwvm = new InventoryWrapperViewModel(sub);
            InventoryWrapperViewModel iwvm2 = new InventoryWrapperViewModel(sub);
            InventoryWrapperDirector iwd = InventoryWrapperDirector.GetInstance();

            var item = iwvm.Items.First();
            var cnt = iwvm.Items.Count;
            InventoryWrapperEditorViewModel helper = new InventoryWrapperEditorViewModel(iwvm, item);

            Assert.AreEqual(1, helper.ItemList.Count());
            Assert.IsTrue(helper.SpecificationList.Count() == 0 || helper.SpecificationList.Count() == 1);
            Assert.IsNotNull(helper.Item);
            Assert.IsNotNull(helper.Specification);

            var fwd = FieldWrapperDirector.GetInstance();
            var warews = fwd.CreateCollection<Warehouse, Observable<Warehouse>>();

            var itemCnt = helper.Quantity = 201;
            var warew = helper.Warehouse = warews.First() == helper.Warehouse ? warews.Last() : warews.First();

            helper.Update();

            Assert.AreEqual(cnt, iwvm.Items.Count);
            Assert.IsTrue(iwvm.Items.Contains(item));
            Assert.IsTrue(iwvm2.Items.Contains(item));
            Assert.IsTrue(iwd.Contains(item));

            // Clone 했을 때의 db데이터들이 삭제됨을 예상
            using (var db = LexDb.GetDbInstance())
            {
                int invenCnt = db.LoadAll<Inventory>().Count();
                Assert.AreEqual(invenCnt, iwvm.Items.Count);
            }

            var replaceItem = iwvm.Items.Where(x => x.ID == item.ID).Single();

            Assert.AreEqual(itemCnt, replaceItem.Quantity);
            Assert.AreEqual(warew, replaceItem.Warehouse);
            Assert.AreEqual(item.Item, replaceItem.Item);
            Assert.AreEqual(item.Specification, replaceItem.Specification);
        }

        /// <summary>
        /// 새로운 InventoryWrapper를 추가하기 위해 InventoryWrapperEditorViewModel를 사용한다.
        /// </summary>
        [TestMethod]
        public void AddNewItem()
        {
            new DummyDbData().Create();

            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            InventoryWrapperViewModel iwvm = new InventoryWrapperViewModel(sub);
            iwvm.CreateFinderViewModel(null);
            InventoryWrapperViewModel iwvm2 = new InventoryWrapperViewModel(sub);
            iwvm2.CreateFinderViewModel(null);
            InventoryWrapperDirector iwd = InventoryWrapperDirector.GetInstance();

            var cnt = iwvm.Items.Count;
            InventoryWrapperEditorViewModel hel = new InventoryWrapperEditorViewModel(iwvm);

            Assert.IsNull(hel.Item);
            Assert.IsNull(hel.Specification);

            var allItemws = hel.ItemList;
            var selectedItem = hel.Item = allItemws.ElementAt(new Random().Next(allItemws.Count() - 1));
            var allSpecws = hel.SpecificationList;

            var coll = InventoryWrapperDirector.GetInstance().CreateCollection();
            foreach (var spec in allSpecws)
                Assert.IsFalse(coll.Any(x => x.Specification.ID == spec.ID));

            var selectedSpec = hel.Specification = allSpecws.FirstOrDefault();

            if (hel.Item != null)
            {
                hel.Warehouse = hel.WarehouseList.FirstOrDefault();
                hel.Quantity = 203;

                var invenw = hel.Update();

                Assert.IsNotNull(invenw.ID);

                var addItem = iwvm.Items.Last();

                Assert.AreEqual(cnt + 1, iwvm.Items.Count);
                Assert.IsTrue(iwvm.Items.Contains(addItem));
                Assert.IsTrue(iwvm2.Items.Contains(addItem));
                Assert.IsTrue(iwd.Contains(addItem));
                Assert.AreEqual(selectedItem, addItem.Item);
                Assert.AreEqual(selectedSpec, addItem.Specification);
            }
        }

        /// <summary>
        /// 재고 데이터 중 보관장소를 변경한 경우 ioStock 데이터들도 똑같이 연동하여야 한다.
        /// </summary>
        [Ignore]
        [TestMethod]
        public void WhenChangeWarehouseThenSyncIOStockItems()
        {
            new DummyDbData().Create();
            FieldWrapperDirector fwd = FieldWrapperDirector.GetInstance();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            ItemWrapperViewModel ivm = new ItemWrapperViewModel(sub);
            InventoryWrapperViewModel invm = new InventoryWrapperViewModel(sub);
            var invenw = invm.Items.Random();
            InventoryWrapperEditorViewModel inventoryEditorViewModel = new InventoryWrapperEditorViewModel(invm, invenw);
            SearchStockWrapperViewModel svm = new SearchStockWrapperViewModel(IOStockType.ALL, sub);

            Assert.IsTrue(svm.Items.All(x => x.Inventory != null));

            var specw = inventoryEditorViewModel.Specification;
            var warew = inventoryEditorViewModel.Warehouse = inventoryEditorViewModel.WarehouseList.Where(x => x != inventoryEditorViewModel.Warehouse).First();
            inventoryEditorViewModel.Update();

            Assert.AreEqual(invenw.Warehouse, warew);

            var result = svm.Items.Where(x => x.Specification.ID == specw.ID);
            Assert.IsTrue(result.All(x => x.Warehouse == warew));
        }

        /// <summary>
        /// 에러8번 
        /// </summary>
        [TestMethod]
        public void CreateItemButNotSetMakerThenCreateInvenThenAgainLoad()
        {
            //모든 디비 데이터 삭제
            using (var db = LexDb.GetDbInstance())
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
            InventoryWrapperViewModel invm = new InventoryWrapperViewModel(sub);
            //ItemFinderViewModel fvm = new ItemFinderViewModel(null);
            //아이템 새로 생성 하지만 Maker 프로퍼티는 설정 하지 아니함
            ivm.AddNewItemCommand.Execute(null);
            Assert.IsFalse(fwd.CreateCollection<Specification, SpecificationWrapper>().Any(x => x.Field.ItemID == null));
            //입고 데이터 생성 .. 재고에 데이터가 없을테니 재고도 같이 생성됨
            InventoryWrapperEditorViewModel evm = new InventoryWrapperEditorViewModel(invm);
            var itemw = evm.Item = evm.ItemList.First();
            var specw = evm.Specification = evm.SpecificationList.First();

            //설정한 inven 데이터 저장
            InventoryWrapper savedData = evm.Update();
            //Assert.AreEqual(1, fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM)).Count());
            
            //디렉터 파괴
            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            CollectionViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            StockWrapperDirector.Distory();
            //다시 로드
            sub = CollectionViewModelObserverSubject.GetInstance();
            ivm = new ItemWrapperViewModel(sub);
            invm = new InventoryWrapperViewModel(sub);
            var fvm = new ItemFinderViewModel(null);
            //FinderViewModel에 이유를 알 수 없는 에러 .. 품목이 2개로 들어감
            Assert.AreEqual(1, fvm.Nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).Count());

            //품목의 여러 프로퍼티 호출 
            itemw = ivm.Items.Where(x => x.ID == itemw.ID).Single();
            var ac = itemw.AllCurrency;
            var ac1 = itemw.AllMaker;
            var ac2 = itemw.AllMeasure;
            Assert.IsNull(itemw.SelectedCurrency);
            Assert.IsNull(itemw.SelectedMaker);
            Assert.IsNull(itemw.SelectedMeasure);

            var invenw = invm.Items.Where(x => x.Specification.ID == specw.ID).Single();
            Assert.IsNull(invenw.Maker);
            Assert.IsNull(invenw.Measure);
            Assert.IsNull(invenw.Currency);
        }
    }
}