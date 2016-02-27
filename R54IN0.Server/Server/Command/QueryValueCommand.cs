using MySql.Data.MySqlClient;
using R54IN0.Format;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Server
{
    public class QueryValueCommand : ICommand<ReadOnlySession, BinaryRequestInfo>
    {
        public string Name
        {
            get
            {
                return Commands.QUERY_VALUE;
            }
        }

        public virtual void ExecuteCommand(ReadOnlySession session, BinaryRequestInfo requestInfo)
        {
            ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(requestInfo.Key, requestInfo.Body);
            string formatName = pfmt.Table;
            string sql = pfmt.SQL;
            Send(session, sql, formatName);
        }

        public void Send(ReadOnlySession session, string sql, string formatName)
        {
            List<object> formats = new List<object>();
            ReadOnlyServer server = session.AppServer as ReadOnlyServer;
            MySqlConnection conn = server.MySQL;

            List<object> values = new List<object>();
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    values.Add(reader.GetValue(0) == DBNull.Value ? null : reader.GetValue(0));
            }

            byte[] response = new ProtocolFormat().SetValueList(values).ToBytes(Name);
            session.Send(response, 0, response.Length);
        }
    }
}