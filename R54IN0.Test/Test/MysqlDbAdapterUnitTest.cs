using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace R54IN0.Test
{
    [TestClass]
    public class MysqlDbAdapterUnitTest
    {
        [TestMethod]
        public async Task CanCreate()
        {
            var mysql = new MysqlDbAdapter();
        }

        [TestMethod]
        public async Task TestInsert()
        {
            var mysql = new MysqlDbAdapter();
            await mysql.ConnectAsync();
            await mysql.InsertAsync(new Product() { Name = "new product" });
            await mysql.DisconnectAsync();
        }
    }
}