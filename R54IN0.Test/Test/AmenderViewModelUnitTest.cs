using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using R54IN0.WPF;

namespace R54IN0.Test
{
    [TestClass]
    public class AmenderViewModelUnitTest
    {
        /// <summary>
        /// 새로운 입출고 데이터를 등록하였을 경우 입출고 수량이 더해진 만큼 재고수량 값이 변동되어야 한다.
        /// </summary>
        [TestMethod]
        public async Task RecordNewItemThenInvenQuantitySameRemainQty()
        {
            await new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.INCOMING;
            viewmodel.Product = ObservableFieldDirector.GetInstance().Copy<Product>().Random();
            var selectedInven = viewmodel.Inventory = viewmodel.InventoryList.Random();
            int inQty = selectedInven.Quantity;
            int icQty = viewmodel.Quantity = 10;

            var record = await viewmodel.RecordAsync();

            Assert.AreEqual(record.Inventory.Quantity, inQty + icQty);
        }

        /// <summary>
        /// 기존의 데이터를 불러와서 각 항목을 채움 물론 입출고 별로 달리
        /// 채우는 것들 .. 
        /// 수량, 가격
        /// 입출고처, 
        /// 창고 or 프로젝트 이름 
        /// 적은 사람
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ClickLoadButton()
        {
            await new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.INCOMING;
            viewmodel.Product = ObservableFieldDirector.GetInstance().Copy<Product>().Random();
            viewmodel.Inventory = viewmodel.InventoryList.Random();

            viewmodel.LoadLastRecordCommand.Execute(null);

            Assert.IsNotNull(viewmodel.Client);
            Assert.IsNotNull(viewmodel.Warehouse);
            Assert.IsNotNull(viewmodel.Employee);
            Assert.IsNull(viewmodel.Project);
        }

        /// <summary>
        /// AmenderView에서 새로운 프로젝트를 등록하였다면 ProjectListView에도 추가가 되어야 한다.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task SyncProjectListViewModel()
        {
            await new Dummy().Create();
            IOStockStatusViewModel iosViewModel = new IOStockStatusViewModel();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(iosViewModel);
            viewmodel.StockType = IOStockType.OUTGOING;
            viewmodel.Product = ObservableFieldDirector.GetInstance().Copy<Product>().Random();
            viewmodel.Inventory = viewmodel.InventoryList.Random();
            var name = viewmodel.ProjectText = "DY=NEW=FACE";

            var record = await viewmodel.RecordAsync();

            Assert.IsTrue(iosViewModel.ProjectListBoxViewModel.Items.Contains(record.Project));
        }

        [TestMethod]
        public async Task ClickLoadButton2()
        {
            await new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.OUTGOING;
            viewmodel.Product = ObservableFieldDirector.GetInstance().Copy<Product>().Random();
            viewmodel.Inventory = viewmodel.InventoryList.Random();

            viewmodel.LoadLastRecordCommand.Execute(null);

            Assert.IsNotNull(viewmodel.Client);
            Assert.IsNotNull(viewmodel.Project);
            Assert.IsNotNull(viewmodel.Employee);
            Assert.IsNull(viewmodel.Warehouse);
        }
    }
}
