using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SuperSocketTest
{
    public class ADD : StringCommandBase<RaceSession>
    {
        public override void ExecuteCommand(RaceSession session, StringRequestInfo requestInfo)
        {
            Console.WriteLine("\n=BEGIN==============================!!");
            RaceServer server = session.AppServer as RaceServer;
            server.Count1 += 1;
            Interlocked.Add(ref server.Count2, 1);

            string body = requestInfo.Body;
            int i = int.Parse(body);
            Console.WriteLine(i.ToString().PadLeft(20));

            Console.WriteLine("================================END=!!\n");
        }
    }

    public class RaceSession : AppSession<RaceSession>
    {
    }

    public class RaceServer : AppServer<RaceSession>
    {
        public int Count1;
        public long Count2;
    }
}