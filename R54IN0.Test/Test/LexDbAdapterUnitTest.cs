using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace R54IN0.Test
{
    [TestClass]
    public class LexDbAdapterUnitTest
    {
        [TestMethod]
        public void CanCreate()
        {
            new DbAdapter();
        }

        /// <summary>
        /// 데이터베이스 레코드 삭제 메서드 테스트
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DeleteAsync()
        {
            new Dummy().Create();

            var stock = LexDb.GetDbInstance().LoadAll<IOStockFormat>().Random();
            var inventory = LexDb.GetDbInstance().LoadAll<InventoryFormat>().Random();
            var prodoct = LexDb.GetDbInstance().LoadAll<Product>().Random();

            DbAdapter adapter = new DbAdapter();
            bool isConnected = await adapter.ConnectAsync();
            if (isConnected)
            {
                await adapter.DeleteAsync<IOStockFormat>(stock);
                await adapter.DeleteAsync<InventoryFormat>(inventory);
                await adapter.DeleteAsync<Product>(prodoct);
            }

            Assert.IsNull(LexDb.GetDbInstance().LoadByKey<IOStockFormat>(stock.ID));
            Assert.IsNull(LexDb.GetDbInstance().LoadByKey<InventoryFormat>(inventory.ID));
            Assert.IsNull(LexDb.GetDbInstance().LoadByKey<Product>(prodoct.ID));
        }

        /// <summary>
        /// 데이터베이스 레코드 추가 메서드 테스트
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InsertAsync()
        {
            Product product = new Product() { Name = "some name", ID = Guid.NewGuid().ToString() };
            InventoryFormat invenFmt = new InventoryFormat() { Specification = "some specification", ProductID = product.ID, ID = Guid.NewGuid().ToString() };
            IOStockFormat iosFmt = new IOStockFormat() { InventoryID = invenFmt.ID, StockType = IOStockType.INCOMING, ID = Guid.NewGuid().ToString() };

            DbAdapter adapter = new DbAdapter();
            bool isConnected = await adapter.ConnectAsync();
            if (isConnected)
            {
                await adapter.InsertAsync<Product>(product);
                await adapter.InsertAsync<InventoryFormat>(invenFmt);
                await adapter.InsertAsync<IOStockFormat>(iosFmt);
            }

            Assert.IsNotNull(LexDb.GetDbInstance().LoadByKey<Product>(product.ID));
            Assert.IsNotNull(LexDb.GetDbInstance().LoadByKey<InventoryFormat>(invenFmt.ID));
            Assert.IsNotNull(LexDb.GetDbInstance().LoadByKey<IOStockFormat>(iosFmt.ID));
        }

        /// <summary>
        /// 데이터베이스 레코드의 필드를 업데이트(수정)
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UpdateAsync()
        {
            Product product = new Product() { Name = "some name", ID = Guid.NewGuid().ToString() };
            DbAdapter adapter = new DbAdapter();
            bool isConnected = await adapter.ConnectAsync();
            if (isConnected)
            {
                await adapter.InsertAsync(product);
                product.Name = "new name";
                await adapter.UpdateAsync(product, "Name");
            }
            var loadedProduct = LexDb.GetDbInstance().LoadByKey<Product>(product.ID);
            Assert.IsNotNull(loadedProduct);
            Assert.AreEqual(product.Name, loadedProduct.Name);
        }

        /// <summary>
        /// 테이블의 모든 레코드 가져오기
        /// </summary>
        /// <typeparam name="TableT"></typeparam>
        /// <returns></returns>
        [TestMethod]
        public async Task SelectAllAsync()
        {
            new Dummy().Create();
            Product[] products = null;
            Product[] products2 = null;
            products = LexDb.GetDbInstance().LoadAll<Product>();

            DbAdapter adapter = new DbAdapter();
            bool isConnected = await adapter.ConnectAsync();
            if (isConnected)
            {
                var items = await adapter.SelectAllAsync<Product>();
                products2 = items.ToArray();
            }
            Assert.IsTrue(products.Any(x => products2.Any(y => x.ID == y.ID)));
        }

        /// <summary>
        /// 프로퍼티 이름과 일치 값으로 쿼리 테스트
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task QueryAsync()
        {
            Product product = null;
            product = LexDb.GetDbInstance().LoadAll<Product>().Random();
            DbAdapter adapter = new DbAdapter();
            bool isConnected = await adapter.ConnectAsync();
            if (isConnected)
            {
                var result = await adapter.QueryAsync<Product>(DbCommand.WHERE, "Name", product.Name);
                Assert.IsTrue(result.All(x => x.Name == product.Name));
            }
        }

        [TestMethod]
        public async Task QueryAsync2()
        {
            InventoryFormat infmt = null;
            infmt = LexDb.GetDbInstance().LoadAll<InventoryFormat>().Random();

            DbAdapter adapter = new DbAdapter();
            bool isConnected = await adapter.ConnectAsync();
            if (isConnected)
            {
                var s = new DateTime(2010, 1, 1);
                var n = DateTime.Now;
                var result = await adapter.QueryAsync<IOStockFormat>(
                    DbCommand.WHERE, "InventoryID", infmt.ID,
                    DbCommand.BETWEEN | DbCommand.OR_EQUAL, "Date", s, n);
                Assert.IsTrue(result.All(x => x.InventoryID == infmt.ID && s <= x.Date && x.Date <= n));
            }
        }

        [TestMethod]
        public async Task QueryAsync3()
        {
            InventoryFormat infmt = null;
            infmt = LexDb.GetDbInstance().LoadAll<InventoryFormat>().Random();

            DbAdapter adapter = new DbAdapter();
            bool isConnected = await adapter.ConnectAsync();
            if (isConnected)
            {
                var s = new DateTime(2010, 1, 1);
                var n = DateTime.Now;
                var result = await adapter.QueryAsync<IOStockFormat>(
                    DbCommand.WHERE, "InventoryID", infmt.ID,
                    DbCommand.IS_LESS_THEN | DbCommand.OR_EQUAL, "Date", n,
                    DbCommand.DESCENDING, "Date",
                    DbCommand.LIMIT, 1);
                Assert.IsTrue(result.All(x => x.InventoryID == infmt.ID && s <= x.Date && x.Date <= n));
            }
        }

        [TestMethod]
        public async Task QueryAsync4()
        {
            DbAdapter adapter = new DbAdapter();
            bool isConnected = await adapter.ConnectAsync();
            if (isConnected)
            {
                var date = new DateTime(2015, 1, 1);
                var result = await adapter.QueryAsync<IOStockFormat>(
                    DbCommand.IS_LESS_THEN, "Date", date);
                Assert.IsTrue(result.Count() != 0);
                Assert.IsTrue(result.All(x => x.Date < date));
                var max = result.Max(x => x.Date);
                result = await adapter.QueryAsync<IOStockFormat>(
                    DbCommand.IS_LESS_THEN, "Date", date,
                    DbCommand.DESCENDING, "Date",
                    DbCommand.LIMIT, 1);
                Assert.AreEqual(max, result.Single().Date);
            }
        }
    }
}