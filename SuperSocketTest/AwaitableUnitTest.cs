using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperSocket.SocketBase.Config;
using System.Net;
using System.Net.Sockets;
using SuperSocket.SocketBase;
using System.Text;
using Dawn.Net.Sockets;
using System.Threading.Tasks;

namespace SuperSocketTest
{
    [TestClass]
    public class AwaitableUnitTest
    {
        [TestMethod]
        public async Task SocketAwaitableCommunityTest()
        {
            var config = new ServerConfig
            {
                Port = 2012,
                Ip = "Any",
                MaxConnectionNumber = 1000,
                Mode = SocketMode.Tcp,
                Name = "SomeServer"
            };

            SomeServer server = new SomeServer();
            server.Setup(config);
            server.Start();
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), config.Port);

            using (Socket s = new Socket(iep.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                SocketAwaitable a = new SocketAwaitable();
                a.RemoteEndPoint = iep;
                await s.ConnectAsync(a);

                a = new SocketAwaitable();
                byte[] buf = Encoding.ASCII.GetBytes("ECHO 1234\r\n");
                a.Buffer = new ArraySegment<byte>(buf);
                await s.SendAsync(a);

                await s.ReceiveAsync(a);
                string receData = Encoding.ASCII.GetString(a.Transferred.Array, 0, a.Transferred.Count);
                Assert.AreEqual("1234\r\n", receData);
            }

            server.Stop();
        }
    }
}
