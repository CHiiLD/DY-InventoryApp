using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperSocket.ClientEngine;
using System.Net;
using System.Threading.Tasks;

namespace SuperSocketTest
{
    [TestClass]
    public class SuperSocektClientUnitTest
    {
        [TestMethod]
        public void ConnectionFailTest()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3032);
            AsyncTcpSession session = new AsyncTcpSession(iep);
            session.Connect();

            Task.Delay(1000).Wait();
        }
    }
}