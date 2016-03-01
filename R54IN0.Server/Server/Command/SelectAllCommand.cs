using MySql.Data.MySqlClient;
using R54IN0.Format;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Server
{
    public class SelectAllCommand : ICommand<ReadOnlySession, BinaryRequestInfo>
    {
        public virtual string Name
        {
            get
            {
                return ProtocolCommand.SELECT_ALL;
            }
        }

        public IID GetForamt(string formatName)
        {
            IID iid = null;
            switch (formatName)
            {
                case nameof(Maker):
                    iid = new Maker(); break;
                case nameof(Measure):
                    iid = new Measure(); break;
                case nameof(Customer):
                    iid = new Customer(); break;
                case nameof(Supplier):
                    iid = new Supplier(); break;
                case nameof(Project):
                    iid = new Project(); break;
                case nameof(Product):
                    iid = new Product(); break;
                case nameof(Warehouse):
                    iid = new Warehouse(); break;
                case nameof(Employee):
                    iid = new Employee(); break;
                case nameof(InventoryFormat):
                    iid = new InventoryFormat(); break;
                case nameof(IOStockFormat):
                    iid = new IOStockFormat(); break;
                default:
                    throw new NotSupportedException();
            }
            return iid;
        }

        public virtual void ExecuteCommand(ReadOnlySession session, BinaryRequestInfo requestInfo)
        {
            ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(requestInfo.Key, requestInfo.Body);
            string formatName = pfmt.Table;
            string sql = string.Format("select * from {0};", formatName);
            Send(session, sql, formatName);
        }

        public void Send(ReadOnlySession session, string sql, string formatName)
        {
            List<object> formats = new List<object>();
            ReadOnlyServer server = session.AppServer as ReadOnlyServer;
            MySqlConnection conn = server.MySQL;

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    object format = GetForamt(formatName);
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

            byte[] data = new ProtocolFormat(formatName).SetValueList(formats).ToBytes(Name);
            session.Logger.DebugFormat("검색 결과를 클라이언트에게 전송합니다.(CMD: {0}, TYPE: {1}, BYTE SIZE: {2})", Name, formatName, data.Count());
            session.Send(data, 0, data.Length);
            //session.Logger.Debug(Encoding.UTF8.GetString(data));
        }
    }
}