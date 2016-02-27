using System;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using R54IN0.Server;
using MySQL.Test;
using SuperSocket.SocketBase.Config;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace R54IN0.WPF.Test
{
    [TestFixture]
    public class ServerStressUnitTest
    {
        private ReadOnlyServer _server;

        [TestFixtureSetUp]
        public void TestSetUp()
        {
            using (MySqlConnection conn = new MySqlConnection(MySqlJsonFormat.ConnectionString("mysql_connection_string.json")))
            {
                conn.Open();
                Dummy dummy = new Dummy(conn);
                dummy.Create();
            }

            _server = new ReadOnlyServer();
            _server.Setup(new ServerConfig()
            {
                Ip = "Any",
                Port = 4000,
                DisableSessionSnapshot = true,
            });
        }

        [TestFixtureTearDown]
        public void TestTearDown()
        {

        }

        [SetUp]
        public void Setup()
        {
            _server.Start();
            DataDirector.InitialzeInstanceAsync().Wait();
        }

        [TearDown]
        public void TearDown()
        {
            CollectionViewModelObserverSubject.Destory();
            TreeViewNodeDirector.Destroy(true);
            DataDirector.Destroy();
            _server.Stop();
        }

        /// <summary>
        /// 10초 동안 서버로부터 데이터를 받는다.
        /// </summary>
        [Test]
        public async Task ReceiveDataForAbout10s()
        {
            IDbAction db = DataDirector.GetInstance().Db;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < 1000 * 2)
            {
                await db.SelectAsync<IOStockFormat>();
            }
            Console.WriteLine(sw.ElapsedMilliseconds + " ms");
        }
    }
}
