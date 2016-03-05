using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [TestMethod]
        public void ParallelRead()
        {
            List<Task> tasks = new List<Task>();
            int i = 100;
            while (--i != 0)
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    using (var conn = new MySqlConnection(ConnectingString.KEY))
                    {
                        conn.Open();
                        using (var cmd = new MySqlCommand("select * from IOStockFormat order by rand() limit 1;", conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                Console.WriteLine(string.Format("ID: {0}", reader.GetString(0)));
                        }
                    }
                });
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }
    }
}