using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace R54IN0.Test
{
    [TestClass]
    public class InventoryWrapperEditorViewModelTest
    {
        [TestMethod]
        public void CanCreate()
        {
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            InventoryWrapperViewModel iwvm = new InventoryWrapperViewModel(sub);
            InventoryWrapperEditorViewModel helper = new InventoryWrapperEditorViewModel(iwvm);
        }

        /// <summary>
        /// 아이템즈 프로퍼티를 변경하기 위해 Helper 클래스를 사용하고 적용되었는지 확인한다.
        /// </summary>
        [TestMethod]
        public void EditItem()
        {
            new DummyDbData().Create();

            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
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
            var warews = fwd.CreateCollection<Warehouse, FieldWrapper<Warehouse>>();

            var itemCnt = helper.ItemCount = 201;
            var warew = helper.Warehouse = warews.First() == helper.Warehouse ? warews.Last() : warews.First();

            helper.Update();

            Assert.AreEqual(cnt, iwvm.Items.Count);
            Assert.IsTrue(iwvm.Items.Contains(item));
            Assert.IsTrue(iwvm2.Items.Contains(item));
            Assert.IsTrue(iwd.Contains(item));

            // Clone 했을 때의 db데이터들이 삭제됨을 예상
            using (var db = DatabaseDirector.GetDbInstance())
            {
                int invenCnt = db.LoadAll<Inventory>().Count();
                Assert.AreEqual(invenCnt, iwvm.Items.Count);
            }

            var replaceItem = iwvm.Items.Where(x => x.UUID == item.UUID).Single();

            Assert.AreEqual(itemCnt, replaceItem.ItemCount);
            Assert.AreEqual(warew, replaceItem.Warehouse);
            Assert.AreEqual(item.Item, replaceItem.Item);
            Assert.AreEqual(item.Specification, replaceItem.Specification);
        }

        /// <summary>
        /// 새로운 InventoryWrapper를 추가하기 위해 InventoryWrapperViewModelHelper를 사용한다.
        /// </summary>
        [TestMethod]
        public void AddNewItem()
        {
            new DummyDbData().Create();

            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            InventoryWrapperViewModel iwvm = new InventoryWrapperViewModel(sub);
            InventoryWrapperViewModel iwvm2 = new InventoryWrapperViewModel(sub);
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
                Assert.IsFalse(coll.Any(x => x.Specification.UUID == spec.UUID));

            var selectedSpec = hel.Specification = allSpecws.FirstOrDefault();

            if (hel.Item != null)
            {
                hel.Warehouse = hel.WarehouseList.FirstOrDefault();
                hel.ItemCount = 203;

                hel.Update();
                var addItem = iwvm.Items.Last();

                Assert.AreEqual(cnt + 1, iwvm.Items.Count);
                Assert.IsTrue(iwvm.Items.Contains(addItem));
                Assert.IsTrue(iwvm2.Items.Contains(addItem));
                Assert.IsTrue(iwd.Contains(addItem));
                Assert.AreEqual(selectedItem, addItem.Item);
                Assert.AreEqual(selectedSpec, addItem.Specification);
            }
        }
    }
}