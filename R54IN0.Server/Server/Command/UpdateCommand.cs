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
    public class UpdateCommand : ICommand<WriteOnlySession, BinaryRequestInfo>, IWriteSessionCommand
    {
        public virtual string Name
        {
            get
            {
                return ProtocolCommand.UPDATE;
            }
        }

        public virtual void ExecuteCommand(WriteOnlySession session, BinaryRequestInfo requestInfo)
        {
            ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(requestInfo.Key, requestInfo.Body);
            ExecuteCommand(session, pfmt, Update);
        }

        protected void ExecuteCommand(WriteOnlySession session, ProtocolFormat pfmt, Action<WriteOnlySession, object> action)
        {
            string formatName = pfmt.Table;
            JObject jobj = pfmt.Value as JObject;

            switch (formatName)
            {
                case nameof(Maker):
                    action(session, jobj.ToObject<Maker>());
                    break;
                case nameof(Measure):
                    action(session, jobj.ToObject<Measure>());
                    break;
                case nameof(Customer):
                    action(session, jobj.ToObject<Customer>());
                    break;
                case nameof(Supplier):
                    action(session, jobj.ToObject<Supplier>());
                    break;
                case nameof(Project):
                    action(session, jobj.ToObject<Project>());
                    break;
                case nameof(Product):
                    action(session, jobj.ToObject<Product>());
                    break;
                case nameof(Warehouse):
                    action(session, jobj.ToObject<Warehouse>());
                    break;
                case nameof(Employee):
                    action(session, jobj.ToObject<Employee>());
                    break;
                case nameof(InventoryFormat):
                    action(session, jobj.ToObject<InventoryFormat>());
                    break;
                case nameof(IOStockFormat):
                    action(session, jobj.ToObject<IOStockFormat>());
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void Update(WriteOnlySession session, object item)
        {
            WriteOnlyServer server = session.AppServer as WriteOnlyServer;
            MySqlConnection conn = server.MySQL;

            Type type = item.GetType();
            IID iid = item as IID;
            string sql = string.Format("update {0} set ", type.Name);
            StringBuilder sb = new StringBuilder(sql);

            int fieldCnt = 0;
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
                        if ((value1 == null ^ value2 == null) || (value1 != null && value2 != null && value1.ToString() != value2.ToString()))
                        {
                            sb.Append(string.Format("{0} = '{1}', ", name, value3));
                            fieldCnt++;
                        }
                    }
                }
            }
            if (fieldCnt == 0)
                return;

            sb.Remove(sb.Length - 2, 2);
            sb.Append(string.Format(" where ID = '{0}';", iid.ID));
            sql = sb.ToString();

            session.Logger.Debug(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                cmd.ExecuteNonQuery();

            byte[] data = new ProtocolFormat(type).SetInstance(item).ToBytes(ProtocolCommand.UPDATE);

            session.Logger.DebugFormat("변경된 포맷을 클라이언트들에게 알립니다.(CMD: {0}, TYPE: {1}, SIZE: {2})", Name, type, data.Length);

            foreach (WriteOnlySession s in server.GetAllSessions())
                s.Send(data, 0, data.Length);
            if (type == typeof(IOStockFormat) && (sql.Contains("Quantity") || sql.Contains("StockType") || sql.Contains("Date")))
                this.InvfQuantityBroadCast(session, type.Name, iid.ID, null);
        }
    }
}