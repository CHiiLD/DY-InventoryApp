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
    public class UpdateCommand : InsertCommand, IWriteSessionCommand
    {
        public override string Name
        {
            get
            {
                return ProtocolCommand.UPDATE;
            }
        }

        public override void ExecuteCommand(WriteOnlySession session, BinaryRequestInfo requestInfo)
        {
            ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(requestInfo.Key, requestInfo.Body);
            ExecuteCommand(session, pfmt, Update);
        }

        public void Update(WriteOnlySession session, object item)
        {
            WriteOnlyServer server = session.AppServer as WriteOnlyServer;
            MySqlConnection conn = server.MySQL;

            Type type = item.GetType();
            IID iid = item as IID;
            string sql = string.Format("update {0} set ", type.Name);
            StringBuilder sb = new StringBuilder(sql);

            sql = string.Format("select * from {0} where ID = '{1}'", type.Name, iid.ID);
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string name = reader.GetName(i);
                        object value1 = reader.GetValue(i);
                        if (DBNull.Value == value1)
                            value1 = null;

                        PropertyInfo info = type.GetProperty(name);
                        object value2 = info.GetValue(item);
                        if (value2 is Enum)
                            value2 = (int)value2;

                        object value3 = this.ConvertMySQLTypeValue(value2);
                        if (value1 == null ^ value2 == null)
                            sb.Append(string.Format("{0} = '{1}', ", name, value3));
                        else if (value1 != null && value2 != null && value1.ToString() != value2.ToString())
                            sb.Append(string.Format("{0} = '{1}', ", name, value3)); //TODO여기 부분 잘 작동되는지 확인하기 InventoryFormat, IOStockFormat
                    }
                }
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(string.Format(" where ID = '{0}';", iid.ID));
            sql = sb.ToString();

            session.Logger.Debug(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                cmd.ExecuteNonQuery();

            byte[] data = new ProtocolFormat(type).SetInstance(item).ToBytes(ProtocolCommand.UPDATE);
            foreach (WriteOnlySession s in server.GetAllSessions())
            {
                if(s != session)
                    s.Send(data, 0, data.Length);
            }
            if (type == typeof(IOStockFormat))
            {
                if (sql.Contains("Quantity") || sql.Contains("StockType"))
                    this.CalcInventoryFormatQty(conn, type.Name, iid.ID);
            }
        }
    }
}