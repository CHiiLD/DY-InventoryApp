using log4net;
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
    public class DeleteCommand : ICommand<WriteOnlySession, BinaryRequestInfo>, IWriteSessionCommand
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string Name
        {
            get
            {
                return ProtocolCommand.DELETE;
            }
        }

        public void ExecuteCommand(WriteOnlySession session, BinaryRequestInfo requestInfo)
        {
            ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(requestInfo.Key, requestInfo.Body);
            Delete(session, pfmt.Table, pfmt.ID);
        }

        private void Delete(WriteOnlySession session, string type, string id)
        {
            WriteOnlyServer server = session.AppServer as WriteOnlyServer;
            MySqlConnection conn = server.MySQL;

            string invID = null;
            string projID = null;
            if (type == typeof(IOStockFormat).Name)
            {
                List<Tuple<string, string>> tuples = this.QueryReturnTuple<string, string>(conn, 
                    "select InventoryID, ProjectID from {0} where ID = '{1}';", nameof(IOStockFormat), id);
                Tuple<string, string> tuple = tuples.SingleOrDefault();
                if (tuple != null)
                {
                    invID = tuple.Item1;
                    projID = tuple.Item2;
                }
            }

            string sql = string.Format("delete from {0} where ID = '{1}';", type, id);
            session.Logger.Debug(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                cmd.ExecuteNonQuery();

            SerialKiller(conn, type, id);

            byte[] data = new ProtocolFormat(type).SetID(id).ToBytes(ProtocolCommand.DELETE);
            foreach (WriteOnlySession s in server.GetAllSessions())
                s.Send(data, 0, data.Length);

            this.CalcInventoryFormatQty(conn, type, id, invID);
            KillProject(session, conn, projID);
        }

        private void SerialKiller(MySqlConnection conn, string type, string id)
        {
            log.Debug(nameof(SerialKiller));
            if (type == typeof(Product).Name)
                KillInventoryFormat(conn, id);
            else if (type == typeof(InventoryFormat).Name)
                KillIOStockFormat(conn, id);
            else if (type != typeof(IOStockFormat).Name)
                KillFieldFormat(conn, type, id);
        }

        private void KillFieldFormat(MySqlConnection conn, string type, string fieldID)
        {
            log.Debug(nameof(KillFieldFormat));
            string pType;
            if (type == typeof(Maker).Name || type == typeof(Measure).Name)
                pType = typeof(InventoryFormat).Name;
            else
                pType = typeof(IOStockFormat).Name;
            string pName = type + "ID";
            string sql = string.Format("update {0} set {1} = '{2}' where {1} = '{3}';", pType, pName, null, fieldID);

            log.Debug(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                cmd.ExecuteNonQuery();
        }

        private void KillInventoryFormat(MySqlConnection conn, string productID)
        {
            log.Debug(nameof(KillInventoryFormat));
            string sql;
            List<string> invIDs = new List<string>();

            sql = string.Format("delete from {0} where InventoryID in (select ID from {1} where ProductID = '{2}');",
                nameof(IOStockFormat), nameof(InventoryFormat), productID);
            log.Debug(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                cmd.ExecuteNonQuery();

            sql = string.Format("delete from {0} where ProductID = '{1}';",
                nameof(InventoryFormat), productID);
            log.Debug(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                cmd.ExecuteNonQuery();
        }

        private void KillIOStockFormat(MySqlConnection conn, string inventoryID)
        {
            log.Debug(nameof(KillIOStockFormat));
            string sql = string.Format("delete from {0} where InventoryID = '{1}';",
                        nameof(IOStockFormat), inventoryID);
            log.Debug(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// projID를 가진 IOStockFormat이 하나도 없는 경우 해당 프로젝트는 삭제된다.
        /// </summary>
        /// <param name="projID"></param>
        private void KillProject(WriteOnlySession session, MySqlConnection conn, string projID)
        {
            if (string.IsNullOrEmpty(projID))
                return;
            Tuple<int> tuple = this.QueryReturnTuple<int>(conn, "select count(*) from {0} where ProjectID = '{1}';", nameof(IOStockFormat), projID).SingleOrDefault();
            if (tuple != null && tuple.Item1 == 0)
            {
                Delete(session, typeof(Project).Name, projID);
            }
        }
    }
}
