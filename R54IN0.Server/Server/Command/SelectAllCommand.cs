using MySql.Data.MySqlClient;
using R54IN0.Format;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Server.Server.Command
{
    public class SelectAllCommand : ICommand<ReadOnlySession, BinaryRequestInfo>
    {
        public string Name
        {
            get
            {
                return ReceiveName.SELECT_ALL;
            }
        }

        public object GetForamt(string formatName)
        {
            switch (formatName)
            {
                case nameof(Maker):
                    return new Maker();
            }
            return null;
        }

        public void ExecuteCommand(ReadOnlySession session, BinaryRequestInfo requestInfo)
        {
            session.Logger.DebugFormat("{0}-{1} CommandExecute", session.AppServer.Name, Name);

            ProtocolFormat pfmt = ProtocolFormat.ToFormat(requestInfo.Key, requestInfo.Body);

            string table = pfmt.Table;
            string sql = string.Format("select * from {0};", table);
            List<object> formats = new List<object>();

            ReadOnlyServer server = session.AppServer as ReadOnlyServer;
            MySqlConnection conn = server.MySQL;

            Console.WriteLine(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    object format = GetForamt(table);
                    PropertyInfo[] properties = format.GetType().GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        string name = property.Name;
                        object value = reader[name];
                        if (DBNull.Value == value)
                            value = null;
                        property.SetValue(format, value);
                    }
                    formats.Add(format);
                }
            }

            byte[] response = new ProtocolFormat(table, formats).ToByteArray(Name);
            session.Send(response, 0, response.Length);
        }
    }
}
