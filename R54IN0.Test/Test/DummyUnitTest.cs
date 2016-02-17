using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System.Linq;

namespace R54IN0.Test.Test
{
    [TestClass]
    public class DummyUnitTest
    {
        [TestMethod]
        public void Create()
        {
            new Dummy().Create();
        }

        [TestMethod]
        public void CheckReteinQty()
        {
            new Dummy().Create();
            var inven = DataDirector.GetInstance().CopyInventories().Random();
            var fmts = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' order by Date;"
                , inven.ID);

            Assert.AreNotEqual(0, fmts.Count());

            IOStockFormat near = null;
            foreach (var fmt in fmts)
            {
                int remainQty = 0;
                int iosQty = fmt.Quantity;
                if (fmt.StockType == IOStockType.OUTGOING)
                    iosQty = -iosQty;
                if (near != null)
                    remainQty = near.RemainingQuantity;
                int exp = remainQty + iosQty;
                Assert.AreEqual(fmt.RemainingQuantity, exp);
                near = fmt;
            }

            var lastObj = fmts.Last();
            var oIOStock = new ObservableIOStock(lastObj);
            Assert.AreEqual(oIOStock.RemainingQuantity, oIOStock.Inventory.Quantity);
        }
    }
}