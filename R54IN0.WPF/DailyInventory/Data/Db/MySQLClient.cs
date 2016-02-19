using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace R54IN0.WPF
{
    public class MySQLClient
    {
        public const string DATETIME = "yyyy-MM-dd HH:mm:ss.fff";
        private MySqlConnection _conn;

        public EventHandler<SQLInsertEventArgs> DataInsertEventHandler;
        public EventHandler<SQLUpdateEventArgs> DataUpdateEventHandler;
        public EventHandler<SQLDeleteEventArgs> DataDeleteEventHandler;

        public MySqlConnection Connection
        {
            get
            {
                return _conn;
            }
        }

        public MySQLClient()
        {
            DataInsertEventHandler += OnDataInserted;
            DataUpdateEventHandler += OnDataUpdated;
            DataDeleteEventHandler += OnDataDeleted;
        }

        public MySQLClient(MySqlConnection conn)
        {
            this._conn = conn;
            DataInsertEventHandler += OnDataInserted;
            DataUpdateEventHandler += OnDataUpdated;
            DataDeleteEventHandler += OnDataDeleted;
        }

        public bool Open()
        {
            _conn = new MySqlConnection("Server=localhost;Database=test_inventory;Uid=root;Pwd=f54645464");
            _conn.Open();

            //Table 생성
            CreateTable<InventoryFormat>();
            CreateTable<IOStockFormat>();
            CreateTable<Customer>();
            CreateTable<Employee>();
            CreateTable<Maker>();
            CreateTable<Measure>();
            CreateTable<Product>();
            CreateTable<Project>();
            CreateTable<Supplier>();
            CreateTable<Warehouse>();
            return true;
        }

        public void Close()
        {
            if (_conn != null)
                _conn.Close();
            _conn = null;
        }

        public List<TableT> Select<TableT>() where TableT : class, IID, new()
        {
            List<TableT> result = new List<TableT>();
            string sql = string.Format("select * from {0};", typeof(TableT).Name);
            return ExecuteSelect0<TableT>(sql);
        }

        public TableT Select<TableT>(string id) where TableT : class, IID, new()
        {
            string sql = string.Format("select * from {0} where {1} = '{2}';", typeof(TableT).Name, "ID", id);
            return ExecuteSelect1<TableT>(sql);
        }

        public List<TableT> Query<TableT>(string sql, params object[] args) where TableT : class, IID, new()
        {
            sql = string.Format(sql, args);
            if (!sql.Contains("where") || !sql.Contains("*"))
                throw new ArgumentException();
            return ExecuteSelect0<TableT>(sql);
        }

        public string Update<TableT>(string id, string property, object value) where TableT : class, IID, new()
        {
            Update<TableT>(id, new Dictionary<string, object>() { { property, value } });
            return id;
        }

        public string Update<TableT>(string id, IDictionary<string, object> content) where TableT : class, IID, new()
        {
            string sql = string.Format("update {0} set ", typeof(TableT).Name);
            StringBuilder sb = new StringBuilder(sql);
            foreach (var c in content)
            {
                string name = c.Key;
                object value = ConvertSqlValue(c.Value);
                sb.Append(string.Format("{0} = '{1}', ", name, value));
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(string.Format(" where ID = '{0}';", id));
            sql = sb.ToString();

            Console.WriteLine(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                cmd.ExecuteNonQuery();

            if (typeof(TableT) == typeof(IOStockFormat))
            {
                if (content.Keys.Contains("Quantity") || content.Keys.Contains("StockType"))
                    CalcInvenQty<TableT>(id);
            }
            return id;
        }

        public void Delete<TableT>(string id) where TableT : class, IID, new()
        {
            string invID = null;
            if (typeof(TableT) == typeof(IOStockFormat))
                invID = QueryInvID(id);

            string sql = string.Format("delete from {0} where ID = '{1}';", typeof(TableT).Name, id);
            Console.WriteLine(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                cmd.ExecuteNonQuery();

            SerialKiller<TableT>(id);
            DataDeleteEventHandler(this, new SQLDeleteEventArgs(typeof(TableT), id));
            CalcInvenQty<TableT>(id, invID);
        }

        public string Insert<TableT>(object item) where TableT : class, IID, new()
        {
            return Insert<TableT>(item as TableT);
        }

        public string Insert<TableT>(TableT item) where TableT : IID, new()
        {
            if (item.ID == null)
                item.ID = Guid.NewGuid().ToString();

            string sql = CreateInsertSQLClause(item);
            Console.WriteLine(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                cmd.ExecuteNonQuery();

            DataInsertEventHandler(this, new SQLInsertEventArgs(item));
            CalcInvenQty<TableT>(item.ID);
            return item.ID;
        }

        #region private method

        private void CalcInvenQty<TableT>(string stockID, string invID = null)
        {
            if (typeof(TableT) == typeof(IOStockFormat))
            {
                string sql;
                if (invID == null)
                    invID = QueryInvID(stockID);
                int quantity = 0;
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
                Console.WriteLine(sql);
                using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        quantity = reader.GetInt32(0);
                }
                DataUpdateEventHandler(this, new SQLUpdateEventArgs(typeof(InventoryFormat), invID, "Quantity", quantity));
            }
        }

        public string QueryInvID(string stockID)
        {
            string invID = null;
            string sql = string.Format("select InventoryID from {0} where ID = '{1}'", nameof(IOStockFormat), stockID);
            Console.WriteLine(sql);
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    invID = reader.GetString(0);
            }
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
                        if (value.GetType() == typeof(DBNull))
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
                        if (value.GetType() == typeof(DBNull))
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

        private void KillFieldFormat<TableT>(string id)
        {
            Console.WriteLine(nameof(KillFieldFormat));
            Type type = typeof(TableT);
            Type pType;

            if (type == typeof(Maker) || type == typeof(Measure))
                pType = typeof(InventoryFormat);
            else
                pType = typeof(IOStockFormat);
            string pName = type.Name + "ID";
            string sql = string.Format("update {0} set {1} = '{2}' where {1} = '{3}';", pType.Name, pName, null, id);

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