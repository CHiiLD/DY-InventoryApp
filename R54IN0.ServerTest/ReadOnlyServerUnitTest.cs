using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase;
using R54IN0.Server;
using R54IN0.WPF;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace R54IN0.ServerTest
{
    [TestClass]
    public class ReadOnlyServerUnitTest
    {
        private static ReadOnlyServer _server;

        [ClassInitialize]
        public static void Setup(TestContext testContest)
        {
            Console.WriteLine(testContest.TestName);

            ServerConfig config = new ServerConfig
            {
                Port = 4000,
                Ip = "Any",
                MaxConnectionNumber = 10,
                Mode = SocketMode.Tcp,
                Name = nameof(ReadOnlyServer)
            };

            _server = new ReadOnlyServer();
            _server.Setup(config);
        }

        [TestInitialize]
        public void StartServer()
        {
            _server.Start();
        }

        [TestCleanup]
        public void StopServer()
        {
            _server.Stop();
        }

        [TestMethod]
        public void CanCreate()
        {

        }

        [TestMethod]
        public async Task SelectAll()
        {
            using (ClientAdapter db = new ClientAdapter())
            {
                db.Open();
                List<Maker> makers = await db.SelectAsync<Maker>();
                makers.ForEach(x => Console.WriteLine(x.Name));
            }
        }
    }
}
