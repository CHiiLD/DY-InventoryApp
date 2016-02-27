using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R54IN0.Format;
using SuperSocket.SocketBase.Protocol;

namespace R54IN0.Server
{
    public class QueryInstanceCommand : SelectAllCommand
    {
        public override string Name
        {
            get
            {
                return Commands.QUERY_FORMAT;
            }
        }

        public override void ExecuteCommand(ReadOnlySession session, BinaryRequestInfo requestInfo)
        {
            ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(requestInfo.Key, requestInfo.Body);
            string formatName = pfmt.Table;
            string sql = pfmt.SQL;
            Send(session, sql, formatName);
        }
    }
}