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
    }
}
