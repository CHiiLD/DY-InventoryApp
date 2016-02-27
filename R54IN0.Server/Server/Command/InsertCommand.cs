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
    public class InsertCommand : ICommand<WriteOnlySession, BinaryRequestInfo>, IWriteSessionCommand
    {
        public virtual string Name
        {
            get
            {
                return Commands.INSERT;
            }
        }

        public virtual void ExecuteCommand(WriteOnlySession session, BinaryRequestInfo requestInfo)
        {
            ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(requestInfo.Key, requestInfo.Body);
            ExecuteCommand(session, pfmt, Insert);
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
            Console.WriteLine(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                cmd.ExecuteNonQuery();

            byte[] data = new ProtocolFormat(type).SetInstance(item).ToBytes(Commands.INSERT);
            foreach (WriteOnlySession s in server.GetAllSessions())
                s.Send(data, 0, data.Length);

            this.CalcInventoryFormatQty(conn, type.Name, iid.ID);
        }
    }
}