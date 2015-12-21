using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

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

            var itemws = FieldWrapperDirector.GetInstance().CreateFieldWrapperCollection<Item, ItemWrapper>();
            var itemw = itemws.Where(x => x.UUID == dummy.TestItemUUID).Single();
            var specws = FieldWrapperDirector.GetInstance().CreateFieldWrapperCollection<Specification, SpecificationWrapper>();

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
            FinderNode node = fvm.Root.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM)).FirstOrDefault();
            Assert.IsNotNull(node);
            var list = new List<FinderNode>() { node };
            fvm.OnSelectNodes(fvm, new System.Windows.Controls.SelectionChangedCancelEventArgs(list, new List<FinderNode>()));
            Assert.AreEqual(1, fvm.SelectedNodes.Count);

            Assert.IsTrue(iwvm.Items.All(x=>x.Item.UUID == node.ItemUUID));

            //FinderViewModel에 여러개의 아이템을 선택
            FinderNode node2 = fvm.Root.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM)).LastOrDefault();
            Assert.AreNotEqual(node, node2);
            var list2 = new List<FinderNode>() { node, node2 };
            fvm.OnSelectNodes(fvm, new System.Windows.Controls.SelectionChangedCancelEventArgs(list2, list));
            Assert.AreEqual(2, fvm.SelectedNodes.Count);

            Assert.IsTrue(iwvm.Items.All(x => x.Item.UUID == node.ItemUUID || x.Item.UUID == node2.ItemUUID));
        }
    }
} 