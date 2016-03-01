using log4net;
using MySql.Data.MySqlClient;
using R54IN0.Format;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Server
{
    //TODO PING PONG 1분 간격으로 전송해서 응답 없으면 섹션 해제
    public static class WriteSessionCommandExtension
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public const string DATETIME = "yyyy-MM-dd HH:mm:ss.fff";

        public static object ConvertMySQLTypeValue(this IWriteSessionCommand commandInstance, object value)
        {
            var result = value;
            if (result is DateTime)
                result = ((DateTime)value).ToString(DATETIME);
            else if (result is Enum)
                result = (int)value;
            return result;
        }

        public static List<Tuple<T1>> QueryReturnTuple<T1>(this IWriteSessionCommand commandInstance, MySqlConnection conn, string sql, params object[] args)
        {
            List<Tuple<T1>> result = new List<Tuple<T1>>();
            sql = string.Format(sql, args);
            log.Debug(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Tuple<T1> tuple = new Tuple<T1>(
                        reader.IsDBNull(0) ? default(T1) : (T1)(dynamic)reader.GetValue(0));
                    result.Add(tuple);
                }
            }
            return result;
        }

        public static List<Tuple<T1, T2>> QueryReturnTuple<T1, T2>(this IWriteSessionCommand commandInstance, MySqlConnection conn, string sql, params object[] args)
        {
            List<Tuple<T1, T2>> result = new List<Tuple<T1, T2>>();
            sql = string.Format(sql, args);
            log.Debug(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Tuple<T1, T2> tuple = new Tuple<T1, T2>(
                        reader.IsDBNull(0) ? default(T1) : (T1)(dynamic)reader.GetValue(0),
                        reader.IsDBNull(1) ? default(T2) : (T2)(dynamic)reader.GetValue(1));
                    result.Add(tuple);
                }
            }
            return result;
        }

        public static string QueryInvID(this IWriteSessionCommand commandInstance, MySqlConnection conn, string stoID)
        {
            string sql = string.Format("select InventoryID from {0} where ID = '{1}'", nameof(IOStockFormat), stoID);
            List<Tuple<string>> qtyTuples = QueryReturnTuple<string>(commandInstance, conn, sql);
            string invID = qtyTuples.Single().Item1;
            return invID;
        }

        /// <summary>
        /// InventoryFormat의 Quantity값의 갱신소식을 클라이언트들에게 전파합니다.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="invID"></param>
        /// <param name="qty"></param>
        public static void InvfQuantityBroadCast(this IWriteSessionCommand commandInstance, WriteOnlySession session, string type, string stoID, string invID = null)
        {
            WriteOnlyServer server = session.AppServer as WriteOnlyServer;
            MySqlConnection conn = server.MySQL;
            
            if (type != typeof(IOStockFormat).Name)
                return;

            if (invID == null)
                invID = QueryInvID(commandInstance, conn, stoID);
            //Quantity 계산해서 삽입
            string sql = string.Format(@"update {0} set Quantity =
                    ifnull((select sum(Quantity) from {1} where InventoryID = '{2}' and StockType = '{3}'), 0) -
                    ifnull((select sum(Quantity) from {4} where InventoryID = '{5}' and StockType = '{6}'), 0)
                    where ID = '{7}';",
                nameof(InventoryFormat),
                nameof(IOStockFormat), invID, (int)IOStockType.INCOMING,
                nameof(IOStockFormat), invID, (int)IOStockType.OUTGOING,
                invID);
            session.Logger.Debug(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                cmd.ExecuteNonQuery();

            InventoryFormat invf = null;
            sql = string.Format("select * from {0} where ID = '{1}';", nameof(InventoryFormat), invID);
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    invf = new InventoryFormat();
                    PropertyInfo[] properties = typeof(InventoryFormat).GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        string name = property.Name;
                        object value = reader[name];
                        if (DBNull.Value == value)
                            value = null;
                        property.SetValue(invf, value);
                    }
                }
            }
            if (invf == null)
                return;

            byte[] data = new ProtocolFormat(typeof(InventoryFormat)).SetInstance(invf).ToBytes(ProtocolCommand.UPDATE);
            session.Logger.DebugFormat("인벤토리 포맷의 재고수량이 변경되었습니다.(CMD: {0}, QTY: {1}, BYTE: {2})", ProtocolCommand.UPDATE, invf.Quantity, data.Length);
            foreach (WriteOnlySession s in server.GetAllSessions())
                s.Send(data, 0, data.Length);
        }
    }
}
