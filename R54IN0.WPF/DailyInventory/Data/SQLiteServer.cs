using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Text;

namespace R54IN0
{
    public class SQLiteServer
    {
        public const string DATASOURCE = "inventory.db";
        public const string DATETIME = "yyyy-MM-dd HH:mm:ss.fff";
        private SQLiteConnection _conn;

        public void Close()
        {
            if (_conn != null)
            {
                _conn.Dispose();
                _conn = null;
            }
        }

        public bool Open()
        {
            string connstr = string.Format("Data Source={0}", DATASOURCE);
            var conn = new SQLiteConnection(connstr);
            _conn = conn.OpenAndReturn();
            if (_conn != null)
            {
                CreateTables();
            }
            return _conn != null;
        }

        void CreateTables()
        {
            CreateTable<InventoryFormat>();
            CreateTable<IOStockFormat>();

            CreateTable<Customer>();
            CreateTable<Employee>();
            CreateTable<Maker>();
            CreateTable<Maker>();
            CreateTable<Measure>();
            CreateTable<Product>();
            CreateTable<Project>();
            CreateTable<Supplier>();
            CreateTable<Warehouse>();
        }

        public void DropAllTableThenReCreateTable()
        {
            //pragma table_info(sqlite_master);
            string sql = "select name from sqlite_master;";
            Console.WriteLine(sql);
            List<string> tablenames = new List<string>();
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
            using (SQLiteDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string name = rdr["name"] as string;
                    tablenames.Add(name);
                }
            }

            foreach (var name in tablenames)
            {
                sql = string.Format("drop table {0};", name);
                Console.WriteLine(sql);
                using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
                    cmd.ExecuteNonQuery();
            }

            CreateTables();
        }

        public void Insert<TableT>(object item) where TableT : class, IID
        {
            Insert<TableT>(item as TableT);
        }

        public void Insert<TableT>(TableT item) where TableT : class, IID
        {
            if (string.IsNullOrEmpty(item.ID))
                item.ID = Guid.NewGuid().ToString();

            string sql = string.Format("insert into {0} (", typeof(TableT).Name);
            StringBuilder sb = new StringBuilder(sql);
            StringBuilder valueSb = new StringBuilder(") values (");
            var properties = typeof(TableT).GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType.IsNotPublic)
                    continue;
                string fieldName = property.Name;
                sb.Append(fieldName);
                sb.Append(", ");
                object value = property.GetValue(item);
                valueSb.Append('\'');
                if (value is DateTime)
                    valueSb.Append(((DateTime)value).ToString(DATETIME));
                else if (value is Enum)
                    valueSb.Append((int)value);
                else
                    valueSb.Append(value);
                valueSb.Append("', ");
            }
            sb.Remove(sb.Length - 2, 2);
            valueSb.Remove(valueSb.Length - 2, 2);
            valueSb.Append(");");
            sb.Append(valueSb);
            sql = sb.ToString();
            Console.WriteLine(sql);

            using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
                cmd.ExecuteNonQuery();
        }

        public IEnumerable<TableT> Select<TableT>() where TableT : class, IID, new()
        {
            string sql = string.Format("select * from {0};", typeof(TableT).Name);
            Console.WriteLine(sql);
            List<TableT> result = new List<TableT>();
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
            using (SQLiteDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    TableT table = new TableT();
                    var properties = typeof(TableT).GetProperties();
                    foreach (var property in properties)
                    {
                        if (property.PropertyType.IsNotPublic)
                            continue;
                        string fieldName = property.Name;
                        object value = rdr[fieldName];
                        property.SetValue(table, value);
                    }
                    result.Add(table);
                }
            }
            return result;
        }

        public TableT Select<TableT>(string idColName, string id) where TableT : class, IID, new()
        {
            string sql = string.Format("select * from {0} where {1} = '{2}';", typeof(TableT).Name, idColName, id);
            Console.WriteLine(sql);
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
            using (SQLiteDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    TableT table = new TableT();
                    var properties = typeof(TableT).GetProperties();
                    foreach (var property in properties)
                    {
                        if (property.PropertyType.IsNotPublic)
                            continue;
                        string fieldName = property.Name;
                        object value = rdr[fieldName];
                        property.SetValue(table, value);
                    }
                    return table;
                }
            }
            return null;
        }

        public IEnumerable<TableT> Query<TableT>(string format, params object[] args) where TableT : class, IID, new()
        {
            return Query<TableT>(string.Format(format, args));
        }

        public IEnumerable<TableT> Query<TableT>(string sql) where TableT : class, IID, new()
        {
            Console.WriteLine(sql);
            List<TableT> result = new List<TableT>();
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
            using (SQLiteDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    TableT table = new TableT();
                    var properties = typeof(TableT).GetProperties();
                    foreach (var property in properties)
                    {
                        if (property.PropertyType.IsNotPublic)
                            continue;
                        string fieldName = property.Name;
                        object value = rdr[fieldName];
                        property.SetValue(table, value);
                    }
                    result.Add(table);
                }
            }
            return result;
        }

        public void Update<TableT>(TableT item) where TableT : class, IID
        {
            string sql = string.Format("update {0} set ", typeof(TableT).Name);
            StringBuilder sb = new StringBuilder(sql);
            var properties = typeof(TableT).GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType.IsNotPublic)
                    continue;
                string fieldName = property.Name;
                object value = property.GetValue(item);

                if (value != null && value.GetType().IsEnum)
                    value = (int)value;
                else if (value != null && value.GetType() == typeof(DateTime))
                    value = ((DateTime)value).ToString(DATETIME);

                sb.Append(string.Format("{0} = '{1}', ", fieldName, value));
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(string.Format(" where {0} = '{1}';", nameof(item.ID), item.ID));
            sql = sb.ToString();
            Console.WriteLine(sql);
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
                cmd.ExecuteNonQuery();
        }

        public void Update<TableT>(TableT item, string propertyName) where TableT : class, IID
        {
            object value = typeof(TableT).GetProperty(propertyName).GetValue(item);
            if (value.GetType().IsEnum)
                value = (int)value;
            else if (value.GetType() == typeof(DateTime))
                value = ((DateTime)value).ToString(DATETIME);

            string sql = string.Format("update {0} set {1} = '{2}' where {3} = '{4}';", typeof(TableT).Name, propertyName, value, nameof(item.ID), item.ID);
            Console.WriteLine(sql);
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
                cmd.ExecuteNonQuery();
        }

        public void Delete<TableT>(object item) where TableT : class, IID
        {
            Delete<TableT>(item as TableT);
        }

        public void Delete<TableT>(TableT item) where TableT : class, IID
        {
            string sql = string.Format("delete from {0} where {1} = '{2}';", typeof(TableT).Name, nameof(item.ID), item.ID);
            Console.WriteLine(sql);
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
                cmd.ExecuteNonQuery();
        }

        private void CreateTable<TableT>()
        {
            string sql = string.Format("create table if not exists {0} (", typeof(TableT).Name);
            StringBuilder sb = new StringBuilder(sql);
            var properties = typeof(TableT).GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType.IsNotPublic)
                    continue;
                string fieldName = property.Name;
                string fieldType = null;
                Type type = property.PropertyType;
                if (type == typeof(string))
                    fieldType = "text";
                else if (type == typeof(int))
                    fieldType = "int";
                else if (type == typeof(decimal))
                    fieldType = "numeric";
                else if (type == typeof(DateTime))
                    fieldType = "datetime";
                else if (type.IsEnum)
                    fieldType = "int";
                Debug.Assert(fieldType != null);
                sb.Append(string.Format("{0} {1}, ", fieldName, fieldType));
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(");");
            sql = sb.ToString();
            Console.WriteLine(sql);
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
                cmd.ExecuteNonQuery();
        }
    }
}