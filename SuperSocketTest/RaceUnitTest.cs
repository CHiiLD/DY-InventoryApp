using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Dawn.Net.Sockets;
using System.Linq;
using SuperSocket.ClientEngine;
using System.Threading;
using System.Collections.Generic;

namespace SuperSocketTest
{
    [TestClass]
    public class RaceUnitTest
    {
        [TestMethod]
        public async Task CanConnect()
        {
            var config = new ServerConfig
            {
                Port = 2132,
                Ip = "Any",
                MaxConnectionNumber = 1000,
                Mode = SocketMode.Tcp,
                Name = "RaceServer",
                DisableSessionSnapshot = true
            };

            RaceServer server = new RaceServer();
            server.Setup(config);
            server.Start();
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), config.Port);

            using (Socket s = new Socket(iep.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                SocketAwaitable a = new SocketAwaitable();
                a.RemoteEndPoint = iep;
                await s.ConnectAsync(a);

                a = new SocketAwaitable();
                byte[] buf = Encoding.ASCII.GetBytes("ADD 1\r\n");
                a.Buffer = new ArraySegment<byte>(buf);
                await s.SendAsync(a);
                await Task.Delay(100);
            }
            Assert.AreEqual(1, server.Count2);
            server.Stop();
        }

        [TestMethod]
        public void Race0()
        {
            var config = new ServerConfig
            {
                Port = 2132,
                Ip = "Any",
                MaxConnectionNumber = 1000,
                Mode = SocketMode.Tcp,
                Name = "RaceServer",
                DisableSessionSnapshot = true
            };

            RaceServer server = new RaceServer();
            server.Setup(config);
            server.Start();
            server.Count1 = 0;
            server.Count2 = 0;
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), config.Port);
            const int COUNT = 5000;

            AsyncTcpSession session = new AsyncTcpSession(iep);
            session.Connect();

            Parallel.For(0, COUNT, (int i) =>
            {
                byte[] buf = Encoding.ASCII.GetBytes(string.Format("ADD {0}\r\n", i));
                var segment = new ArraySegment<byte>(buf);
                session.Send(segment);
            });

            Thread.Sleep(1000);

            Console.WriteLine("count1 : " + server.Count1);
            Console.WriteLine("count2 : " + server.Count2);
            Assert.AreEqual(COUNT, server.Count1);
            Assert.AreEqual(COUNT, Interlocked.Read(ref server.Count2));

            session.Close();
            server.Stop();
        }

        [TestMethod]
        public void Race1()
        {
            var config = new ServerConfig
            {
                Port = 2132,
                Ip = "Any",
                MaxConnectionNumber = 1000,
                Mode = SocketMode.Tcp,
                Name = "RaceServer",
                DisableSessionSnapshot = true
            };

            RaceServer server = new RaceServer();
            server.Setup(config);
            server.Start();
            server.Count1 = 0;
            server.Count2 = 0;
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), config.Port);
            const int COUNT = 5000;

            AsyncTcpSession session1 = new AsyncTcpSession(iep);
            session1.Connect();
            AsyncTcpSession session2 = new AsyncTcpSession(iep);
            session2.Connect();
            AsyncTcpSession session3 = new AsyncTcpSession(iep);
            session3.Connect();
            AsyncTcpSession session4 = new AsyncTcpSession(iep);
            session4.Connect();

            List<AsyncTcpSession> sessions = new List<AsyncTcpSession>() { session1, session2, session3, session4 };
            Parallel.ForEach(sessions, (AsyncTcpSession s) =>
            {
                Parallel.For(0, COUNT, (int i) =>
                {
                    byte[] buf = Encoding.ASCII.GetBytes(string.Format("ADD {0}\r\n", i));
                    var segment = new ArraySegment<byte>(buf);
                    session1.Send(segment);
                });
            });
            Thread.Sleep(1000);

            Console.WriteLine("count1 : " + server.Count1);
            Console.WriteLine("count2 : " + server.Count2);
            Assert.AreEqual(COUNT * sessions.Count(), server.Count1);
            Assert.AreEqual(COUNT * sessions.Count(), Interlocked.Read(ref server.Count2));

            sessions.ForEach(x => x.Close());
            server.Stop();
        }
    }
}