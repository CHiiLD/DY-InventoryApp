using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace R54IN0.Test
{
    [TestClass]
    public class MysqlDbAdapterUnitTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var mysql = new MysqlDbAdapter();
        }

        [TestMethod]
        public async Task CanConnect()
        {
            var mysql = new MysqlDbAdapter();
            await mysql.ConnectAsync();
            await mysql.DisconnectAsync();
        }
    }
}