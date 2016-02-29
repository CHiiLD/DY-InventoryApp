using System;
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
using NUnit.Framework;
using System.Threading;

namespace R54IN0.ServerTest
{
    [TestFixture]
    public class ReadOnlyServerUnitTest
    {
        private ReadOnlyServer _server;
        private ServerConfig _config;
        private MySqlConnection _conn;

        [TestFixtureSetUp]
        public void ClassInitialize()
        {
            _conn = new MySqlConnection(MySqlJsonFormat.ConnectionString(@"./mysql_connection_string.json"));
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

        [TestFixtureTearDown]
        public void ClassCleanup()
        {
            Console.WriteLine(nameof(ClassCleanup));
            _conn.Close();
            _conn = null;
        }

        [SetUp]
        public void TestInitialize()
        {
            _server.Start();
        }

        [TearDown]
        public void TestCleanup()
        {
            _server.Stop();
        }

        [Test]
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
                byte[] reqtBytes = new ProtocolFormat().SetPing(pp).ToBytes(ProtocolCommand.PING);
                a.Buffer = new ArraySegment<byte>(reqtBytes);
                await s.SendAsync(a);

                await s.ReceiveAsync(a);
                Assert.AreNotEqual(0, a.Transferred.Count);
                ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(a.Transferred.Array, 0, a.Transferred.Count);

                Assert.AreEqual(pfmt.Name, ProtocolCommand.PONG);
                Assert.AreEqual(pp, pfmt.Ping);
            }
        }

        [Test]
        public void AsyncWaitHandleTest()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _config.Port);
            using (Socket s = new Socket(iep.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                var beginAsyncResult = s.BeginConnect(iep, (beginAr) => 
                {
                    Thread.Sleep(2000);
                    Console.WriteLine("connected!");
                }, null);
                Console.WriteLine("wait start");
                beginAsyncResult.AsyncWaitHandle.WaitOne();
                Console.WriteLine("wait end");
            }
        }

        [Test]
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

                byte[] reqtBytes = new ProtocolFormat(typeof(Maker)).ToBytes(ProtocolCommand.SELECT_ALL);
                Array.Copy(reqtBytes, a.Buffer.Array, reqtBytes.Length);
                await s.SendAsync(a);

                await s.ReceiveAsync(a);
                Assert.AreNotEqual(0, a.Transferred.Count);
                ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(a.Transferred.Array, 0, a.Transferred.Count);

                Assert.AreEqual(pfmt.Name, ProtocolCommand.SELECT_ALL);
                var fmts = pfmt.ConvertToFormat<Maker>();
                foreach (var maker in fmts)
                    Console.WriteLine(maker.Name);
            }
        }

        [Test]
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

                byte[] reqtBytes = new ProtocolFormat(typeof(Maker)).SetID(id).ToBytes(ProtocolCommand.SELECT_ONE);
                Array.Copy(reqtBytes, a.Buffer.Array, reqtBytes.Length);
                await s.SendAsync(a);

                await s.ReceiveAsync(a);
                Assert.AreNotEqual(0, a.Transferred.Count);
                ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(a.Transferred.Array, 0, a.Transferred.Count);

                var fmts = pfmt.ConvertToFormat<Maker>();
                Assert.AreEqual(id, fmts.Single().ID);
            }
        }

        [Test]
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
                byte[] reqtBytes = new ProtocolFormat(typeof(IOStockFormat)).SetSQL(sql).ToBytes(ProtocolCommand.QUERY_FORMAT);
                Array.Copy(reqtBytes, a.Buffer.Array, reqtBytes.Length);
                await s.SendAsync(a);

                await s.ReceiveAsync(a);
                Assert.AreNotEqual(0, a.Transferred.Count);
                ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(a.Transferred.Array, 0, a.Transferred.Count);

                var stofmts = pfmt.ConvertToFormat<IOStockFormat>();
                Assert.AreEqual(10, stofmts.Count());
                foreach (var stofmt in stofmts)
                    Console.WriteLine(stofmt.ID);
            }
        }

        [Test]
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
                byte[] reqtBytes = new ProtocolFormat(typeof(IOStockFormat)).SetSQL(sql).ToBytes(ProtocolCommand.QUERY_VALUE);
                Array.Copy(reqtBytes, a.Buffer.Array, reqtBytes.Length);
                await s.SendAsync(a);

                await s.ReceiveAsync(a);
                Assert.AreNotEqual(0, a.Transferred.Count);
                ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(a.Transferred.Array, 0, a.Transferred.Count);

                object value = pfmt.Value;
                int qty = Convert.ToInt32(value);
                Console.WriteLine("query result " + qty);
            }
        }

        [Test]
        public async Task TcpClientTest()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _config.Port);
            using (TcpClient tcpClient = new TcpClient())
            {
                tcpClient.Connect(iep);
                Socket socket = tcpClient.Client;

                BlockingBufferManager bbm = new BlockingBufferManager(4096, 1);
                var a = new SocketAwaitable();
                a.Buffer = bbm.GetBuffer();

                string sql = "select Quantity from InventoryFormat order by rand() limit 1;";
                byte[] reqtBytes = new ProtocolFormat(typeof(IOStockFormat)).SetSQL(sql).ToBytes(ProtocolCommand.QUERY_VALUE);
                Array.Copy(reqtBytes, a.Buffer.Array, reqtBytes.Length);
                await socket.SendAsync(a);

                await socket.ReceiveAsync(a);
                Assert.AreNotEqual(0, a.Transferred.Count);
                ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(a.Transferred.Array, 0, a.Transferred.Count);

                object value = pfmt.Value;
                int qty = Convert.ToInt32(value);
                Console.WriteLine("query result " + qty);
            }
        }

        [Ignore("")]
        [Test]
        public async Task BufferManagerTest()
        {
            BlockingBufferManager bbm = new BlockingBufferManager(ProtocolFormat.BUFFER_SIZE, 10000);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _config.Port);
            using (Socket socket = new Socket(iep.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                SocketAwaitable awaitable = new SocketAwaitable();
                awaitable.RemoteEndPoint = iep;
                await socket.ConnectAsync(awaitable);

                awaitable = new SocketAwaitable();
                var buf = awaitable.Buffer = bbm.GetBuffer();

                string sql = "select * from IOStockFormat";
                byte[] reqtBytes = new ProtocolFormat(typeof(IOStockFormat)).SetSQL(sql).ToBytes(ProtocolCommand.QUERY_FORMAT);
                Array.Copy(reqtBytes, awaitable.Buffer.Array, reqtBytes.Length);
                await socket.SendAsync(awaitable);
                int offset = buf.Offset;
                int count = 0;
                List<ArraySegment<byte>> segments = new List<ArraySegment<byte>>();
                while (await socket.ReceiveAsync(awaitable) == SocketError.Success && awaitable.Transferred.Count > 0)
                {
                    segments.Add(awaitable.Buffer);
                    count += awaitable.Transferred.Count;
                    if (ProtocolFormat.IsReceivedCompletely(awaitable.Transferred.Array, offset, count))
                        break;
                    awaitable.Buffer = bbm.GetBuffer();
                }
                ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(awaitable.Transferred.Array, offset, count);
                var stofmts = pfmt.ConvertToFormat<IOStockFormat>();
                foreach (var stofmt in stofmts)
                    Console.WriteLine(stofmt.ID);
                Console.WriteLine("recv byte size: " + count);
                segments.ForEach(x => bbm.ReleaseBuffer(x));
            }
        }
    }
}