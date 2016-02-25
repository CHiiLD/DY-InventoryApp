using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using MySQL.Test;
using SuperSocket.SocketBase.Config;
using R54IN0.Server;
using SuperSocket.SocketBase;
using System.Net;
using System.Net.Sockets;
using Dawn.Net.Sockets;
using R54IN0.Format;
using System.Threading.Tasks;

namespace R54IN0.ServerTest
{
    [TestClass]
    public class WriteOnlyServerUnitTest
    {
        private static WriteOnlyServer _server;
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
                Port = 4001,
                Ip = "Any",
                MaxConnectionNumber = 10,
                Mode = SocketMode.Tcp,
                Name = nameof(WriteOnlyServer)
            };
            _server = new WriteOnlyServer();
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
        public async Task InsertCommandTest()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _config.Port);
            using (Socket s = new Socket(iep.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                SocketAwaitable a = new SocketAwaitable();
                a.RemoteEndPoint = iep;
                await s.ConnectAsync(a);

                a = new SocketAwaitable();
                Maker maker = new Maker() { ID = Guid.NewGuid().ToString(), Name = "SomE Maker" };
                byte[] reqtBytes = new ProtocolFormat(typeof(Maker)).SetInstance(maker).ToBytes(ReceiveName.INSERT);
                a.Buffer = new ArraySegment<byte>(reqtBytes);
                await s.SendAsync(a);

                await Task.Delay(100);

                string name = null;
                string sql = string.Format("select Name from Maker where ID = '{0}';", maker.ID);
                using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        name = reader.GetString(0);
                    }
                }
                Assert.AreEqual(maker.Name, name);
            }
        }

        [TestMethod]
        public async Task DeleteCommandTest()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _config.Port);
            using (Socket s = new Socket(iep.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                SocketAwaitable a = new SocketAwaitable();
                a.RemoteEndPoint = iep;
                await s.ConnectAsync(a);

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
                Assert.IsNotNull(id);

                a = new SocketAwaitable();
                byte[] reqtBytes = new ProtocolFormat(typeof(Maker)).SetID(id).ToBytes(ReceiveName.DELETE);
                a.Buffer = new ArraySegment<byte>(reqtBytes);
                await s.SendAsync(a);

                await Task.Delay(100);

                int count = -1;
                sql = string.Format("select count(*) from Maker where ID = '{0}';", id);
                using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }
                }
                Assert.AreEqual(0, count);
            }
        }

        [TestMethod]
        public async Task UpdateCommandTest()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _config.Port);
            using (Socket s = new Socket(iep.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                SocketAwaitable a = new SocketAwaitable();
                a.RemoteEndPoint = iep;
                await s.ConnectAsync(a);

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
                Assert.IsNotNull(id);

                Maker maker = new Maker() { ID = id, Name = "some maker ~ " };

                a = new SocketAwaitable();
                byte[] reqtBytes = new ProtocolFormat(typeof(Maker)).SetInstance(maker).ToBytes(ReceiveName.UPDATE);
                a.Buffer = new ArraySegment<byte>(reqtBytes);
                await s.SendAsync(a);

                await Task.Delay(100);

                string name = null;
                sql = string.Format("select Name from Maker where ID = '{0}';", id);
                using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        name = reader.GetString(0);
                    }
                }
                Assert.AreEqual(maker.Name, name);
            }
        }
    }
}