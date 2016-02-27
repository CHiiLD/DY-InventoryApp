using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySQL.Test;
using MySql.Data.MySqlClient;
using R54IN0.WPF;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R54IN0.Test.Test
{
    [TestClass]
    public class ProjectUnitTest
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

        [TestInitialize]
        public void TestInitialize()
        {
            //MySqlConnection conn = DataDirector.GetInstance().DB.Connection;
            //using (MySqlCommand cmd = new MySqlCommand("begin work;", conn))
            //    cmd.ExecuteNonQuery();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            //MySqlConnection conn = DataDirector.GetInstance().DB.Connection;
            //using (MySqlCommand cmd = new MySqlCommand("rollback;", conn))
            //    cmd.ExecuteNonQuery();

            CollectionViewModelObserverSubject.Destory();
            TreeViewNodeDirector.Destroy(true);
            DataDirector.Destroy();
        }

        /// <summary>
        /// IOStockFormat에 기록된 Project의 개수가 0가 된 경우 Project를 삭제한다.
        /// </summary>
        [TestMethod]
        public async Task DeleteProjectAutomatically()
        {
            DataDirector ddr = DataDirector.GetInstance();
            Observable<Project> project = ddr.CopyFields<Project>().Random();

            //IOStockFormat 로드 해서 하나씩 삭제 중 ..
            List<Tuple<string>> tuples = await ddr.DB.QueryReturnTuple<string>("select ID from {0} where ProjectID = '{1}';", nameof(IOStockFormat), project.ID);
            tuples.ForEach(x => ddr.DB.Delete<IOStockFormat>(x.Item1));

            Assert.IsFalse(ddr.CopyFields<Project>().Any(x => x == project));
        }
    }
}
