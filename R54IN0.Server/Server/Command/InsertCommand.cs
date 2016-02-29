using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using R54IN0.Format;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Server
{
    public class InsertCommand : UpdateCommand
    {
        public override string Name
        {
            get
            {
                return ProtocolCommand.INSERT;
            }
        }

        public override void ExecuteCommand(WriteOnlySession session, BinaryRequestInfo requestInfo)
        {
            ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(requestInfo.Key, requestInfo.Body);
            ExecuteCommand(session, pfmt, Insert);
        }

        private void Insert(WriteOnlySession session, object item)
        {
            Type type = item.GetType();
            IID iid = item as IID;
            string sql = string.Format("insert into {0} (", type.Name);
            StringBuilder sb0 = new StringBuilder(sql);
            StringBuilder sb1 = new StringBuilder(") values (");
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                string name = property.Name;
                sb0.Append(name);
                sb0.Append(", ");
                object value = property.GetValue(item);
                sb1.Append('\'');
                sb1.Append(this.ConvertMySQLTypeValue(value));
                sb1.Append("', ");
            }
            sb0.Remove(sb0.Length - 2, 2);
            sb1.Remove(sb1.Length - 2, 2);
            sb1.Append(");");
            sb0.Append(sb1);
            sql = sb0.ToString();

            WriteOnlyServer server = session.AppServer as WriteOnlyServer;
            MySqlConnection conn = server.MySQL;
            session.Logger.Debug(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                cmd.ExecuteNonQuery();

            byte[] data = new ProtocolFormat(type).SetInstance(item).ToBytes(ProtocolCommand.INSERT);

            session.Logger.DebugFormat("추가된 포맷을 클라이언트들에게 알립니다.(TYPE: {0}, SIZE: {1})", type, data.Length);

            foreach (WriteOnlySession s in server.GetAllSessions())
                s.Send(data, 0, data.Length);

            this.InvfQuantityBroadCast(session, type.Name, iid.ID);
        }
    }
}