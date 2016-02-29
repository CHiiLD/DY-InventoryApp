using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperSocket.SocketBase.Config;
using R54IN0.Server;
using MySql.Data.MySqlClient;
using MySQL.Test;
using SuperSocket.SocketBase;
using System.Net;
using System.Net.Sockets;
using Dawn.Net.Sockets;
using System.Threading.Tasks;
using SuperSocket.ClientEngine;
using R54IN0.Format;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace R54IN0.ServerTest
{
    [TestClass]
    public class PushServerUnitTest
    {
        private static WriteOnlyServer _server1;
        private static WriteOnlyServer _server2;
        private static ServerConfig _config;
        private static MySqlConnection _conn;
        private static AsyncTcpSession _session1;
        private static AsyncTcpSession _session2;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Console.WriteLine(nameof(ClassInitialize));
            Console.WriteLine(context.TestName);

            _conn = new MySqlConnection(MySqlJsonFormat.ConnectionString("mysql_connection_string.json"));
            _conn.Open();
            Dummy dummy = new Dummy(_conn);
            dummy.Create();

            _config = new ServerConfig
            {
                Port = 4001,
                Ip = "Any",
                MaxConnectionNumber = 10,
                Mode = SocketMode.Tcp,
                Name = nameof(WriteOnlyServer),
                DisableSessionSnapshot = true
            };
            _server1 = new WriteOnlyServer();
            _server1.Setup(_config);

            _server2 = new WriteOnlyServer();
            _server2.Setup(_config);

            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _config.Port);
            _session1 = new AsyncTcpSession(iep);
            _session2 = new AsyncTcpSession(iep);
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
            _server1.Start();
            _session1.Connect();
            _session2.Connect();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _server1.Stop();
            _session1.Close();
            _session2.Close();
        }

        [TestMethod]
        public async Task InitializeTest()
        {
            await Task.Delay(100);
        }

        [TestMethod]
        public async Task ListnersInsertCommandTest()
        {
            Maker maker = new Maker() { ID = Guid.NewGuid().ToString(), Name = "!@# SomE Make r " };
            Maker result = null;
            EventHandler<DataEventArgs> handler = (object o, DataEventArgs e) =>
            {
                ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(e.Data, e.Offset, e.Length);
                JObject jobj = pfmt.Value as JObject;
                result = jobj.ToObject<Maker>();
            };
            _session2.DataReceived += handler;

            byte[] send = new ProtocolFormat(typeof(Maker)).SetInstance(maker).ToBytes(ProtocolCommand.INSERT);
            _session1.Send(send, 0, send.Length);

            await Task.Delay(100);
            _session2.DataReceived -= handler;

            Assert.AreEqual(maker.ID, result.ID);
            Assert.AreEqual(maker.Name, result.Name);
        }

        [TestMethod]
        public async Task ListnersUpdateCommandTest()
        {
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
            Maker maker = new Maker() { ID = id, Name = "!@# SomE Make r " };
            Maker result = null;

            EventHandler<DataEventArgs> handler = (object o, DataEventArgs e) =>
            {
                ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(e.Data, e.Offset, e.Length);
                JObject jobj = pfmt.Value as JObject;
                result = jobj.ToObject<Maker>();
            };
            _session2.DataReceived += handler;

            byte[] send = new ProtocolFormat(typeof(Maker)).SetInstance(maker).ToBytes(ProtocolCommand.UPDATE);
            _session1.Send(send, 0, send.Length);

            await Task.Delay(200);
            _session2.DataReceived -= handler;

            Assert.AreEqual(maker.ID, result.ID);
            Assert.AreEqual(maker.Name, result.Name);
        }

        [TestMethod]
        public async Task ListnerDeleteCommandTest()
        {
            string id = null;
            string result = null;
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
            EventHandler<DataEventArgs> handler = (object o, DataEventArgs e) =>
            {
                ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(e.Data, e.Offset, e.Length);
                result = pfmt.ID;
            };
            _session2.DataReceived += handler;

            byte[] send = new ProtocolFormat(typeof(Maker)).SetID(id).ToBytes(ProtocolCommand.DELETE);
            _session1.Send(send, 0, send.Length);

            await Task.Delay(200);
            _session2.DataReceived -= handler;

            Assert.AreEqual(result, id);
        }
    }
}