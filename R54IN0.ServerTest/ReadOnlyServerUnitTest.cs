using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase;
using R54IN0.Server;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using Dawn.Net.Sockets;
using System.Text;
using R54IN0.Format;
using MySQL.Test;
using MySql.Data.MySqlClient;
using System.Linq;

namespace R54IN0.ServerTest
{
    [TestClass]
    public class ReadOnlyServerUnitTest
    {
        private static ReadOnlyServer _server;
        private static ServerConfig _config;
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

            _config = new ServerConfig
            {
                Port = 4000,
                Ip = "Any",
                MaxConnectionNumber = 10,
                Mode = SocketMode.Tcp,
                Name = nameof(ReadOnlyServer)
            };
            _server = new ReadOnlyServer();
            _server.Setup(_config);
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
            _server.Start();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _server.Stop();
        }

        [TestMethod]
        public async Task PingPongCommandTest()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _config.Port);
            using (Socket s = new Socket(iep.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                SocketAwaitable a = new SocketAwaitable();
                a.RemoteEndPoint = iep;
                await s.ConnectAsync(a);

                string pp = "1234";

                a = new SocketAwaitable();
                byte[] reqtBytes = new ProtocolFormat().SetPing(pp).ToBytes(ReceiveName.PING);
                a.Buffer = new ArraySegment<byte>(reqtBytes);
                await s.SendAsync(a);

                await s.ReceiveAsync(a);
                Assert.AreNotEqual(0, a.Transferred.Count);
                ProtocolFormat pfmt = ProtocolFormat.ToFormat(a.Transferred.Array, 0, a.Transferred.Count);

                Assert.AreEqual(pfmt.Name, ReceiveName.PONG);
                Assert.AreEqual(pp, pfmt.Ping);
            }
        }

        [TestMethod]
        public async Task SelectAllCommandTest()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _config.Port);
            using (Socket s = new Socket(iep.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                SocketAwaitable a = new SocketAwaitable();
                a.RemoteEndPoint = iep;
                await s.ConnectAsync(a);
                BlockingBufferManager bbm = new BlockingBufferManager(4096, 1);
                a = new SocketAwaitable();
                a.Buffer = bbm.GetBuffer();

                byte[] reqtBytes = new ProtocolFormat(typeof(Maker)).ToBytes(ReceiveName.SELECT_ALL);
                Array.Copy(reqtBytes, a.Buffer.Array, reqtBytes.Length);
                await s.SendAsync(a);

                await s.ReceiveAsync(a);
                Assert.AreNotEqual(0, a.Transferred.Count);
                ProtocolFormat pfmt = ProtocolFormat.ToFormat(a.Transferred.Array, 0, a.Transferred.Count);

                Assert.AreEqual(pfmt.Name, ReceiveName.SELECT_ALL);
                var fmts = pfmt.ConvertJFormatList<Maker>();
                foreach (var maker in fmts)
                    Console.WriteLine(maker.Name);
            }
        }

        [TestMethod]
        public async Task SelectCommandTest()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _config.Port);
            using (Socket s = new Socket(iep.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                SocketAwaitable a = new SocketAwaitable();
                a.RemoteEndPoint = iep;
                await s.ConnectAsync(a);
                BlockingBufferManager bbm = new BlockingBufferManager(4096, 1);
                a = new SocketAwaitable();
                a.Buffer = bbm.GetBuffer();

                string id = null;
                string sql = "select ID from Maker order by rand() limit 1;";
                using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = reader.GetString(0);
                    }
                }

                byte[] reqtBytes = new ProtocolFormat(typeof(Maker)).SetID(id).ToBytes(ReceiveName.SELECT_ONE);
                Array.Copy(reqtBytes, a.Buffer.Array, reqtBytes.Length);
                await s.SendAsync(a);

                await s.ReceiveAsync(a);
                Assert.AreNotEqual(0, a.Transferred.Count);
                ProtocolFormat pfmt = ProtocolFormat.ToFormat(a.Transferred.Array, 0, a.Transferred.Count);

                var fmts = pfmt.ConvertJFormatList<Maker>();
                Assert.AreEqual(id, fmts.Single().ID);
            }
        }

        [TestMethod]
        public async Task QueryInstanceCommandTest()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _config.Port);
            using (Socket s = new Socket(iep.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                SocketAwaitable a = new SocketAwaitable();
                a.RemoteEndPoint = iep;
                await s.ConnectAsync(a);
                BlockingBufferManager bbm = new BlockingBufferManager(4096, 1);
                a = new SocketAwaitable();
                a.Buffer = bbm.GetBuffer();

                string sql = "select * from IOStockFormat where StockType = 1 order by Date limit 10;";
                byte[] reqtBytes = new ProtocolFormat(typeof(IOStockFormat)).SetSQL(sql).ToBytes(ReceiveName.QUERY_FORMAT);
                Array.Copy(reqtBytes, a.Buffer.Array, reqtBytes.Length);
                await s.SendAsync(a);

                await s.ReceiveAsync(a);
                Assert.AreNotEqual(0, a.Transferred.Count);
                ProtocolFormat pfmt = ProtocolFormat.ToFormat(a.Transferred.Array, 0, a.Transferred.Count);

                var stofmts = pfmt.ConvertJFormatList<IOStockFormat>();
                Assert.AreEqual(10, stofmts.Count());
                foreach (var stofmt in stofmts)
                    Console.WriteLine(stofmt.ID);
            }
        }

        [TestMethod]
        public async Task QueryValueCommand()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _config.Port);
            using (Socket s = new Socket(iep.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                SocketAwaitable a = new SocketAwaitable();
                a.RemoteEndPoint = iep;
                await s.ConnectAsync(a);
                BlockingBufferManager bbm = new BlockingBufferManager(4096, 1);
                a = new SocketAwaitable();
                a.Buffer = bbm.GetBuffer();

                string sql = "select Quantity from InventoryFormat order by rand() limit 1;";
                byte[] reqtBytes = new ProtocolFormat(typeof(IOStockFormat)).SetSQL(sql).ToBytes(ReceiveName.QUERY_VALUE);
                Array.Copy(reqtBytes, a.Buffer.Array, reqtBytes.Length);
                await s.SendAsync(a);

                await s.ReceiveAsync(a);
                Assert.AreNotEqual(0, a.Transferred.Count);
                ProtocolFormat pfmt = ProtocolFormat.ToFormat(a.Transferred.Array, 0, a.Transferred.Count);

                object value = pfmt.Value;
                int qty = Convert.ToInt32(value);
                Console.WriteLine("query result " + qty);
            }
        }
    }
}