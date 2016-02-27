using log4net;
using MySql.Data.MySqlClient;
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

        public static object ConvertMySQLTypeValue(this IWriteSessionCommand writeSessionCmd, object value)
        {
            var result = value;
            if (result is DateTime)
                result = ((DateTime)value).ToString(DATETIME);
            else if (result is Enum)
                result = (int)value;
            return result;
        }

        public static List<Tuple<T1>> QueryReturnTuple<T1>(this IWriteSessionCommand writeSessionCmd, MySqlConnection conn, string sql, params object[] args)
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

        public static List<Tuple<T1, T2>> QueryReturnTuple<T1, T2>(this IWriteSessionCommand writeSessionCmd, MySqlConnection conn, string sql, params object[] args)
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

        public static string QueryInvID(this IWriteSessionCommand writeSessionCmd, MySqlConnection conn, string stoID)
        {
            string sql = string.Format("select InventoryID from {0} where ID = '{1}'", nameof(IOStockFormat), stoID);
            List<Tuple<string>> qtyTuples = QueryReturnTuple<string>(writeSessionCmd, conn, sql);
            string invID = qtyTuples.Single().Item1;
            return invID;
        }

        public static void CalcInventoryFormatQty(this IWriteSessionCommand writeSessionCmd, MySqlConnection conn, string type, string stoID, string invID = null)
        {
            if (type == typeof(IOStockFormat).Name)
            {
                string sql;
                if (invID == null)
                    invID = QueryInvID(writeSessionCmd, conn, stoID);

                //Quantity 계산해서 삽입
                sql = string.Format(@"update {0} set Quantity =
                    ifnull((select sum(Quantity) from {1} where InventoryID = '{2}' and StockType = '{3}'), 0) -
                    ifnull((select sum(Quantity) from {4} where InventoryID = '{5}' and StockType = '{6}'), 0)
                    where ID = '{7}';",
                    nameof(InventoryFormat),
                    nameof(IOStockFormat), invID, (int)IOStockType.INCOMING,
                    nameof(IOStockFormat), invID, (int)IOStockType.OUTGOING,
                    invID);
                log.Debug(sql);
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    cmd.ExecuteNonQuery();
                //재고 수량 구해서 업데이트

                sql = string.Format("select Quantity from {0} where ID = '{1}';", nameof(InventoryFormat), invID);
                List<Tuple<int>> qtyTuples = QueryReturnTuple<int>(writeSessionCmd, conn, sql);
                int quantity = qtyTuples.Single().Item1;
                //DataUpdateEventHandler(this, new SQLUpdateEventArgs(typeof(InventoryFormat), invID, "Quantity", quantity));
            }
        }
    }
}
