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
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            FieldWrapperViewModel<Account, AccountWrapper> vm = new FieldWrapperViewModel<Account, AccountWrapper>(sub);
        }

        /// <summary>
        /// 새로운 아이템 추가 명령어를 실행 하는 테스트
        /// </summary>
        [TestMethod]
        public void PushNewItemAddButton()
        {
            new DummyDbData().Create();
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            FieldWrapperViewModel<Account, AccountWrapper> vm = new FieldWrapperViewModel<Account, AccountWrapper>(sub);

            Assert.IsTrue(vm.CanAddNewItem(null));
            Assert.IsTrue(vm.AddNewItemCommand.CanExecute(null));
            int recCnt = vm.Items.Count;
            vm.AddNewItemCommand.Execute(null);
            Assert.AreEqual(recCnt + 1, vm.Items.Count);
        }

        [TestMethod]
        public void CanSave()
        {
            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            ViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            IOStockWrapperDirector.Distory();
            using (var db = DatabaseDirector.GetDbInstance())
            {
                db.Purge();
            }

            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            FieldWrapperViewModel<Maker, FieldWrapper<Maker>> vm = new FieldWrapperViewModel<Maker, FieldWrapper<Maker>>(sub);
            Assert.AreEqual(0, vm.Items.Count());

            vm.AddNewItemCommand.Execute(null);

            Assert.IsNotNull(vm.Items.First().UUID);

            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            ViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            IOStockWrapperDirector.Distory();

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
            ViewModelObserverSubject sub = ViewModelObserverSubject.GetInstance();
            FieldWrapperViewModel<Account, AccountWrapper> vm = new FieldWrapperViewModel<Account, AccountWrapper>(sub);

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