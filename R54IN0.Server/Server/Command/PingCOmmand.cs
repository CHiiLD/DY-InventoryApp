using R54IN0.Format;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Server
{
    public class PingCommand : ICommand<ReadOnlySession, BinaryRequestInfo>
    {
        public string Name
        {
            get
            {
                return ReceiveName.PING;
            }
        }

        public void ExecuteCommand(ReadOnlySession session, BinaryRequestInfo requestInfo)
        {
            ProtocolFormat pfmt = ProtocolFormat.ToFormat(requestInfo.Key, requestInfo.Body);
            byte[] bytes = new ProtocolFormat().SetPing(pfmt.Ping).ToBytes(ReceiveName.PONG);
            session.Send(bytes, 0, bytes.Length);
        }
    }
}