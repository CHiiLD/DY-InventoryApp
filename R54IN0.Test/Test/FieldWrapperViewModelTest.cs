using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace R54IN0.Test
{
    [TestClass]
    public class FieldWrapperViewModelTest
    {
        [TestMethod]
        public void CanCreate()
        {
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            FieldWrapperViewModel<Client, ClientWrapper> vm = new FieldWrapperViewModel<Client, ClientWrapper>(sub);
        }

        /// <summary>
        /// 새로운 아이템 추가 명령어를 실행 하는 테스트
        /// </summary>
        [TestMethod]
        public void PushNewItemAddButton()
        {
            new DummyDbData().Create();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            FieldWrapperViewModel<Client, ClientWrapper> vm = new FieldWrapperViewModel<Client, ClientWrapper>(sub);

            Assert.IsTrue(vm.CanAddNewItem(null));
            Assert.IsTrue(vm.AddNewItemCommand.CanExecute(null));
            int recCnt = vm.Items.Count;
            vm.AddNewItemCommand.Execute(null);
            Assert.AreEqual(recCnt + 1, vm.Items.Count);
        }

        /// <summary>
        /// AddNewItemCommand 객체가 실행되고 난 뒤에, 데이터베이스에 저장되었는지 테스트한다.
        /// </summary>
        [TestMethod]
        public void CanSave()
        {
            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            CollectionViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            StockWrapperDirector.Distory();
            using (var db = DatabaseDirector.GetDbInstance())
            {
                db.Purge();
            }

            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            FieldWrapperViewModel<Maker, FieldWrapper<Maker>> vm = new FieldWrapperViewModel<Maker, FieldWrapper<Maker>>(sub);
            Assert.AreEqual(0, vm.Items.Count());

            vm.AddNewItemCommand.Execute(null);

            Assert.IsNotNull(vm.Items.First().UUID);

            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            CollectionViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            StockWrapperDirector.Distory();

            vm = new FieldWrapperViewModel<Maker, FieldWrapper<Maker>>(sub);
            Assert.AreEqual(1, vm.Items.Count());
        }

        /// <summary>
        /// 선택된 아이템을 삭제 명령을 실행하는 테스트
        /// </summary>
        [TestMethod]
        public void PushDeleteItemButton()
        {
            new DummyDbData().Create();
            CollectionViewModelObserverSubject sub = CollectionViewModelObserverSubject.GetInstance();
            FieldWrapperViewModel<Client, ClientWrapper> vm = new FieldWrapperViewModel<Client, ClientWrapper>(sub);

            if (vm.Items.Count == 0)
                return;
            // 선택된 아이템이 없으면 삭제를 수행할 수 없다.
            vm.SelectedItem = null;
            Assert.IsFalse(vm.CanDeleteSelectedItem(null));

            vm.SelectedItem = vm.Items.FirstOrDefault();
            Assert.IsTrue(vm.CanDeleteSelectedItem(null));
            int recCnt = vm.Items.Count;
            vm.DeleteItemCommand.Execute(null);
            Assert.AreEqual(recCnt - 1, vm.Items.Count);
        }
    }
}