using MySql.Data.MySqlClient;
using R54IN0.Format;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Server
{
    public class SelectCommand : SelectAllCommand
    {
        public override string Name
        {
            get
            {
                return ProtocolCommand.SELECT_ONE;
            }
        }

        public override void ExecuteCommand(ReadOnlySession session, BinaryRequestInfo requestInfo)
        {
            ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(requestInfo.Key, requestInfo.Body);
            string formatName = pfmt.Table;
            string sql = string.Format("select * from {0} where ID = '{1}';", formatName, pfmt.ID);
            Send(session, sql, formatName);
        }
    }
}