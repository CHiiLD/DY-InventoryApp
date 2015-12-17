using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace R54IN0.Test
{
    [TestClass]
    public class StockViewModelUnitTest
    {
        public InOutStockDataGridViewModel CreateViewModel(StockType type)
        {
            return new InOutStockDataGridViewModel(type);
        }

        [TestMethod]
        public void CanCreateInOutStockViewModel()
        {
            CreateViewModel(StockType.ALL);
            CreateViewModel(StockType.IN);
            CreateViewModel(StockType.OUT);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CanNotCreateInOutStockViewModel()
        {
            CreateViewModel(StockType.NONE);
        }

        [TestMethod]
        public void CanCreateDummyDbData()
        {
            new DummyDbData().Create();
        }

        [TestMethod]
        public void NewItemAdd()
        {
            new DummyDbData().Create();
            var stype = StockType.ALL;
            var viewModel = CreateViewModel(stype);
            var selectedItem =  viewModel.SelectedItem = viewModel.Items.FirstOrDefault();
            viewModel.RemoveSelectedItem();

            Assert.IsFalse(viewModel.Items.Contains(selectedItem));
            Assert.IsFalse(InOutStockPipeCollectionDirector.GetInstance().NewPipe(stype).Contains(selectedItem));
        }

        /// <summary>
        /// 컬렉션 중 InOutStockPipe 아이템의 속성을 변경하여 ReplaceItem 메서드를 실행하였을 때
        /// 컬렉션에서 정상적으로 변경사항을 업데이트하는지 테스트
        /// </summary>
        [TestMethod]
        public void ItemPropertiesChange()
        {
            new DummyDbData().Create();
            string remark = "a";
            int count = 0;
            var stype = StockType.ALL;
            var viewModel = CreateViewModel(stype);
            var selectedItem = viewModel.SelectedItem = viewModel.Items.FirstOrDefault();

            var copy = new InOutStockPipe(selectedItem.Inven);
            copy.Remark = remark;
            copy.ItemCount = count;
            string copy_uuid = copy.Inven.UUID;

            viewModel.ReplaceItem(copy.Inven);
            var first = viewModel.Items.FirstOrDefault();

            Assert.AreEqual(count, first.ItemCount);
            Assert.AreEqual(remark, first.Remark);
            Assert.AreEqual(copy_uuid, first.Inven.UUID);
        }
    }
}
