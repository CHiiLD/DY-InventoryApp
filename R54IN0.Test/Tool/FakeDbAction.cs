﻿using MySql.Data.MySqlClient;
using R54IN0.WPF;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.WPF.Test
{
    public class FakeDbAction : IDbAction
    {
        public const string DATETIME = "yyyy-MM-dd HH:mm:ss.fff";
        private MySqlConnection _conn;

        public EventHandler<SQLInsertEventArgs> DataInsertEventHandler { get; set; }
        public EventHandler<SQLUpdateEventArgs> DataUpdateEventHandler { get; set; }
        public EventHandler<SQLDeleteEventArgs> DataDeleteEventHandler { get; set; }

        public MySqlConnection Connection
        {
            get
            {
                return _conn;
            }
        }

        public FakeDbAction()
        {
            DataInsertEventHandler += OnDataInserted;
            DataUpdateEventHandler += OnDataUpdated;
            DataDeleteEventHandler += OnDataDeleted;
        }

        public FakeDbAction(MySqlConnection conn)
        {
            this._conn = conn;
            DataInsertEventHandler += OnDataInserted;
            DataUpdateEventHandler += OnDataUpdated;
            DataDeleteEventHandler += OnDataDeleted;
        }

        public void Close()
        {
            if (_conn != null)
                _conn.Close();
            _conn = null;
        }

        public List<Tuple<T1>> QueryReturnTuple<T1>(string sql, params object[] args)
        {
            List<Tuple<T1>> result = new List<Tuple<T1>>();
            sql = string.Format(sql, args);
            Console.WriteLine(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
            using (DbDataReader reader = cmd.ExecuteReader())
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

        private List<Tuple<T1, T2>> QueryReturnTuple<T1, T2>(string sql, params object[] args)
        {
            List<Tuple<T1, T2>> result = new List<Tuple<T1, T2>>();
            sql = string.Format(sql, args);
            Console.WriteLine(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
            using (DbDataReader reader = cmd.ExecuteReader())
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

        public void Update<TableT>(TableT item) where TableT : class, IID, new()
        {
            string sql = string.Format("update {0} set ", typeof(TableT).Name);
            StringBuilder sb = new StringBuilder(sql);

            IID iid = item;
            Type type = typeof(TableT);

            sql = string.Format("select * from {0} where ID = '{1}'", type.Name, iid.ID);
            int cnt = 0;
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
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

                        object value3 = this.ConvertSqlValue(value2);
                        if ((value1 == null ^ value2 == null) || (value1 != null && value2 != null && value1.ToString() != value2.ToString()))
                        {
                            sb.Append(string.Format("{0} = '{1}', ", name, value3));
                            cnt++;
                        }
                    }
                }
            }
            if (cnt == 0)
                return;

            sb.Remove(sb.Length - 2, 2);
            sb.Append(string.Format(" where ID = '{0}';", iid.ID));
            sql = sb.ToString();

            Console.WriteLine(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                cmd.ExecuteNonQuery();

            DataUpdateEventHandler(this, new SQLUpdateEventArgs(item));

            if (type == typeof(IOStockFormat) && (sql.Contains("Quantity") || sql.Contains("StockType") || sql.Contains("Date")))
                CalcInventoryQty<TableT>(iid.ID);
        }

        public void Delete<TableT>(string id) where TableT : class, IID, new()
        {
            string invID = null;
            string projID = null;
            if (typeof(TableT) == typeof(IOStockFormat))
            {
                List<Tuple<string, string>> tuples = QueryReturnTuple<string, string>("select InventoryID, ProjectID from {0} where ID = '{1}';", nameof(IOStockFormat), id);
                Tuple<string, string> tuple = tuples.SingleOrDefault();
                if (tuple != null)
                {
                    invID = tuple.Item1;
                    projID = tuple.Item2;
                }
            }

            string sql = string.Format("delete from {0} where ID = '{1}';", typeof(TableT).Name, id);
            Console.WriteLine(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                cmd.ExecuteNonQuery();

            SerialKiller<TableT>(id);
            DataDeleteEventHandler(this, new SQLDeleteEventArgs(typeof(TableT), id));
            CalcInventoryQty<TableT>(id, invID);
            KillProject(projID);
        }

        public void Insert<TableT>(object item) where TableT : class, IID
        {
            Insert<TableT>(item as TableT);
        }

        public void Insert<TableT>(TableT item) where TableT : IID
        {
            if (item.ID == null)
                item.ID = Guid.NewGuid().ToString();

            string sql = CreateInsertSQLClause(item);
            Console.WriteLine(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                cmd.ExecuteNonQuery();

            DataInsertEventHandler(this, new SQLInsertEventArgs(item));
            CalcInventoryQty<TableT>(item.ID);
        }

        public async Task<List<TableT>> SelectAsync<TableT>(int offset, int rowCount) where TableT : class, IID, new()
        {
            List<TableT> result = new List<TableT>();
            string sql = string.Format("select * from {0} limit {1}, {2};", typeof(TableT).Name, offset, rowCount);
            return ExecuteSelect0<TableT>(sql);
        }

        public async Task<List<TableT>> SelectAsync<TableT>() where TableT : class, IID, new()
        {
            List<TableT> result = new List<TableT>();
            string sql = string.Format("select * from {0};", typeof(TableT).Name);
            return ExecuteSelect0<TableT>(sql);
        }

        public async Task<TableT> SelectAsync<TableT>(string id) where TableT : class, IID, new()
        {
            string sql = string.Format("select * from {0} where {1} = '{2}';", typeof(TableT).Name, "ID", id);
            return ExecuteSelect1<TableT>(sql);
        }

        public async Task<List<TableT>> QueryAsync<TableT>(string sql, params object[] args) where TableT : class, IID, new()
        {
            sql = string.Format(sql, args);
            if (!sql.Contains("where") || !sql.Contains("*"))
                throw new ArgumentException();
            return ExecuteSelect0<TableT>(sql);
        }

        public async Task<List<Tuple<T1>>> QueryReturnTupleAsync<T1>(string sql, params object[] args)
        {
            return QueryReturnTuple<T1>(sql, args);
        }
        #region private method

        public void Dispose()
        {

        }

        private void CalcInventoryQty<TableT>(string stockID, string invID = null)
        {
            if (typeof(TableT) == typeof(IOStockFormat))
            {
                string sql;
                if (invID == null)
                    invID = QueryInvID(stockID);
                //Quantity 계산해서 삽입
                sql = string.Format(@"update {0} set Quantity =
                    ifnull((select sum(Quantity) from {1} where InventoryID = '{2}' and StockType = '{3}'), 0) -
                    ifnull((select sum(Quantity) from {4} where InventoryID = '{5}' and StockType = '{6}'), 0)
                    where ID = '{7}';",
                    nameof(InventoryFormat),
                    nameof(IOStockFormat), invID, (int)IOStockType.INCOMING,
                    nameof(IOStockFormat), invID, (int)IOStockType.OUTGOING,
                    invID);
                Console.WriteLine(sql);
                using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                    cmd.ExecuteNonQuery();
                //재고 수량 구해서 업데이트

                sql = string.Format("select Quantity from {0} where ID = '{1}';", nameof(InventoryFormat), invID);
                List<Tuple<int>> qtyTuples = QueryReturnTuple<int>(sql);
                int quantity = qtyTuples.Single().Item1;

                Task<InventoryFormat> task = SelectAsync<InventoryFormat>(invID);
                task.Wait();
                InventoryFormat invf = task.Result;
                DataUpdateEventHandler(this, new SQLUpdateEventArgs(invf));
            }
        }

        private string QueryInvID(string stockID)
        {
            string sql = string.Format("select InventoryID from {0} where ID = '{1}'", nameof(IOStockFormat), stockID);
            List<Tuple<string>> qtyTuples = QueryReturnTuple<string>(sql);
            string invID = qtyTuples.Single().Item1;
            return invID;
        }

        private string CreateInsertSQLClause<TableT>(TableT item)
        {
            string sql = string.Format("insert into {0} (", typeof(TableT).Name);
            StringBuilder sb0 = new StringBuilder(sql);
            StringBuilder sb1 = new StringBuilder(") values (");
            PropertyInfo[] properties = typeof(TableT).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                string name = property.Name;
                sb0.Append(name);
                sb0.Append(", ");
                object value = property.GetValue(item);
                sb1.Append('\'');
                sb1.Append(ConvertSqlValue(value));
                sb1.Append("', ");
            }
            sb0.Remove(sb0.Length - 2, 2);
            sb1.Remove(sb1.Length - 2, 2);
            sb1.Append(");");
            sb0.Append(sb1);
            sql = sb0.ToString();
            return sql;
        }

        private List<TableT> ExecuteSelect0<TableT>(string sql) where TableT : class, IID, new()
        {
            Console.WriteLine(sql);
            List<TableT> result = new List<TableT>();
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    TableT table = new TableT();
                    PropertyInfo[] properties = typeof(TableT).GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        string name = property.Name;
                        object value = reader[name];
                        if (DBNull.Value == value)
                            value = null;
                        property.SetValue(table, value);
                    }
                    result.Add(table);
                }
            }
            return result;
        }

        private TableT ExecuteSelect1<TableT>(string sql) where TableT : class, IID, new()
        {
            Console.WriteLine(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    TableT table = new TableT();
                    var properties = typeof(TableT).GetProperties();
                    foreach (var property in properties)
                    {
                        string name = property.Name;
                        object value = reader[name];
                        if (DBNull.Value == value)
                            value = null;
                        property.SetValue(table, value);
                    }
                    return table;
                }
            }
            return null;
        }

        private void SerialKiller<TableT>(string id)
        {
            Console.WriteLine(nameof(SerialKiller));
            Type type = typeof(TableT);
            if (type == typeof(Product))
                KillInventoryFormat(id);
            else if (type == typeof(InventoryFormat))
                KillIOStockFormat(id);
            else if (type != typeof(IOStockFormat))
                KillFieldFormat<TableT>(id);
        }

        private void KillFieldFormat<TableT>(string fieldID)
        {
            Console.WriteLine(nameof(KillFieldFormat));
            Type type = typeof(TableT);
            Type pType;

            if (type == typeof(Maker) || type == typeof(Measure))
                pType = typeof(InventoryFormat);
            else
                pType = typeof(IOStockFormat);
            string pName = type.Name + "ID";
            string sql = string.Format("update {0} set {1} = '{2}' where {1} = '{3}';", pType.Name, pName, null, fieldID);

            Console.WriteLine(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                cmd.ExecuteNonQuery();
        }

        private void KillInventoryFormat(string productID)
        {
            Console.WriteLine(nameof(KillInventoryFormat));
            string sql;
            List<string> invIDs = new List<string>();

            sql = string.Format("delete from {0} where InventoryID in (select ID from {1} where ProductID = '{2}');",
                nameof(IOStockFormat), nameof(InventoryFormat), productID);
            Console.WriteLine(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                cmd.ExecuteNonQuery();

            sql = string.Format("delete from {0} where ProductID = '{1}';",
                nameof(InventoryFormat), productID);
            Console.WriteLine(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                cmd.ExecuteNonQuery();
        }

        private void KillIOStockFormat(string inventoryID)
        {
            Console.WriteLine(nameof(KillIOStockFormat));
            string sql = string.Format("delete from {0} where InventoryID = '{1}';",
                        nameof(IOStockFormat), inventoryID);
            Console.WriteLine(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// projID를 가진 IOStockFormat이 하나도 없는 경우 해당 프로젝트는 삭제된다.
        /// </summary>
        /// <param name="projID"></param>
        private void KillProject(string projID)
        {
            if (string.IsNullOrEmpty(projID))
                return;
            Tuple<int> tuple = QueryReturnTuple<int>("select count(*) from {0} where ProjectID = '{1}';", nameof(IOStockFormat), projID).SingleOrDefault();
            if (tuple != null && tuple.Item1 == 0)
            {
                Delete<Project>(projID);
            }
        }

        private object ConvertSqlValue(object value)
        {
            var result = value;

            if (result is DateTime)
                result = ((DateTime)value).ToString(DATETIME);
            else if (result is Enum)
                result = (int)value;

            return result;
        }

        private void CreateTable<TableT>()
        {
            string sql = string.Format("create table if not exists {0} (", typeof(TableT).Name);
            StringBuilder sb = new StringBuilder(sql);
            var properties = typeof(TableT).GetProperties();
            int guidLength = Guid.NewGuid().ToString().Length;
            foreach (var property in properties)
            {
                if (property.PropertyType.IsNotPublic)
                    continue;
                string fieldName = property.Name;
                string fieldType = null;
                Type type = property.PropertyType;
                if (fieldName == "ID")
                    fieldType = string.Format("varchar({0}) not null unique", guidLength);
                else if (fieldName.Contains("ID"))
                    fieldType = string.Format("varchar({0})", guidLength);
                else if (type == typeof(string))
                    fieldType = "text";
                else if (type == typeof(int))
                    fieldType = "int not null default 0";
                else if (type == typeof(decimal))
                    fieldType = "numeric not null default 0";
                else if (type == typeof(DateTime))
                    fieldType = "datetime not null";
                else if (type.IsEnum)
                    fieldType = "int not null";
                sb.Append(string.Format("{0} {1}, ", fieldName, fieldType));
            }
            sb.Append(" primary key (ID)");
            sb.Append(");");
            sql = sb.ToString();

            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                cmd.ExecuteNonQuery();
        }

        private void OnDataDeleted(object sender, SQLDeleteEventArgs e)
        {
        }

        private void OnDataUpdated(object sender, SQLUpdateEventArgs e)
        {
        }

        private void OnDataInserted(object sender, SQLInsertEventArgs e)
        {
        }

        #endregion private method
    }
}