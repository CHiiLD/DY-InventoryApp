using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace SuperSocketTest
{
    public class ECHO : CommandBase<SomeSession, StringRequestInfo>
    {
        public override void ExecuteCommand(SomeSession session, StringRequestInfo requestInfo)
        {
            session.Send(requestInfo.Body);
        }
    }
}
