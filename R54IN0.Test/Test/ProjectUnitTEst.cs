using System;
using NUnit.Framework;
using MySQL.Test;
using MySql.Data.MySqlClient;
using R54IN0.WPF;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using R54IN0.Server;

namespace R54IN0.WPF.Test.Test
{
    [TestFixture]
    public class ProjectUnitTest
    {
        MySqlConnection _conn;

        [TestFixtureSetUp]
        public void TestSetUp()
        {
            _conn = new MySqlConnection(MySqlJsonFormat.ConnectionString("mysql_connection_string.json"));
            _conn.Open();
            Dummy dummy = new Dummy(_conn);
            dummy.Create();
        }

        [TestFixtureTearDown]
        public void TestTearDown()
        {
            _conn.Close();
        }

        [SetUp]
        public void Setup()
        {
            IDbAction dbAction = new FakeDbAction(_conn);
            DataDirector.IntializeInstance(dbAction);
        }

        [TearDown]
        public void TearDown()
        {
            CollectionViewModelObserverSubject.Destory();
            TreeViewNodeDirector.Destroy(true);
            DataDirector.Destroy();
        }

        /// <summary>
        /// IOStockFormat에 기록된 Project의 개수가 0가 된 경우 Project를 삭제한다.
        /// </summary>
        [Test]
        public async Task DeleteProjectAutomatically()
        {
            DataDirector ddr = DataDirector.GetInstance();
            Observable<Project> project = ddr.CopyFields<Project>().Random();

            //IOStockFormat 로드 해서 하나씩 삭제 중 ..
            List<Tuple<string>> tuples = await ddr.Db.QueryReturnTupleAsync<string>("select ID from {0} where ProjectID = '{1}';", nameof(IOStockFormat), project.ID);
            tuples.ForEach(x => ddr.Db.Delete<IOStockFormat>(x.Item1));

            Assert.IsFalse(ddr.CopyFields<Project>().Any(x => x == project));
        }
    }
}
