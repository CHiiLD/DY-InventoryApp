using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System;

namespace MySQL.Test
{
    [TestClass]
    public class MySQLUnitTest
    {
        private static MySqlConnection _conn;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Console.WriteLine(nameof(ClassInitialize));
            Console.WriteLine(context.TestName);

            _conn = new MySqlConnection(ConnectingString.KEY);
            _conn.Open();

            Dummy dummy = new Dummy(_conn);
            dummy.Create();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Console.WriteLine(nameof(ClassCleanup));
            _conn.Close();
            _conn = null;
        }

        [TestMethod]
        public void DummyWorkTest()
        {
            Assert.IsNotNull(_conn);
        }
    }
}