using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace R54IN0.Test
{
    [TestClass]
    public class InventoryWrapperViewModelHelperTest
    {
        [TestMethod]
        public void CanCreate()
        {
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            InventoryWrapperViewModel iwvm = new InventoryWrapperViewModel(sub);
            InventoryWrapperViewModelHelper helper = new InventoryWrapperViewModelHelper(iwvm);
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
            InventoryWrapperViewModelHelper helper = new InventoryWrapperViewModelHelper(iwvm, item);

            Assert.AreEqual(1, helper.AllItem.Count());
            Assert.IsTrue(helper.AllSpecification.Count() == 0 || helper.AllSpecification.Count() == 1);
            Assert.IsNotNull(helper.SelectedItem);
            Assert.IsNotNull(helper.SelectedSpecification);

            var fwd = FieldWrapperDirector.GetInstance();
            var warews = fwd.CreateFieldWrapperCollection<Warehouse, FieldWrapper<Warehouse>>();

            var itemCnt = helper.ItemCount = 201;
            var warew = helper.SelectedWarehouse = warews.ElementAt(new Random().Next(warews.Count - 1));

            helper.Update();

            Assert.AreEqual(cnt, iwvm.Items.Count);
            Assert.IsFalse(iwvm.Items.Contains(item));
            Assert.IsFalse(iwvm2.Items.Contains(item));
            Assert.IsFalse(iwd.Contains(item));

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
            InventoryWrapperViewModelHelper hel = new InventoryWrapperViewModelHelper(iwvm);

            Assert.IsNull(hel.SelectedItem);
            Assert.IsNull(hel.SelectedSpecification);

            var allItemws = hel.AllItem;
            var selectedItem = hel.SelectedItem = allItemws.ElementAt(new Random().Next(allItemws.Count() - 1));
            var allSpecws = hel.AllSpecification;

            var coll = InventoryWrapperDirector.GetInstance().CreateInventoryWrapperCollection();
            foreach(var spec in allSpecws)
                Assert.IsFalse(coll.Any(x => x.Specification.UUID == spec.UUID));

            var selectedSpec = hel.SelectedSpecification = allSpecws.FirstOrDefault();

            if (hel.SelectedItem != null)
            {
                hel.SelectedWarehouse = hel.AllWarehouse.FirstOrDefault();
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