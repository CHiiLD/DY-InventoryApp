using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SuperSocketTest
{
    [TestClass]
    public class SuperSocketUnitTest
    {
        [TestMethod]
        public async Task ServerWorkTest()
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
            IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), config.Port);

            using (TcpClient client = new TcpClient())
            {
                client.Connect(serverAddress);
                var stream = client.GetStream();
                byte[] buf = Encoding.ASCII.GetBytes("ECHO 1234\r\n");
                await stream.WriteAsync(buf, 0, buf.Length);
                int size = await stream.ReadAsync(buf, 0, buf.Length);
                string receData = Encoding.ASCII.GetString(buf, 0, size);
                Assert.AreEqual("1234\r\n", receData);
            }

            server.Stop();
        }
    }
}
