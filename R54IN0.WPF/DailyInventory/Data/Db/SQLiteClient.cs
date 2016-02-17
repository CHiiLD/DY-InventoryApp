using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace R54IN0.WPF
{
    public partial class SQLiteClient
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

        private void CreateTables()
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
            if (item.ID == null)
                throw new NotSupportedException();

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

            if (DataInsertEventHandler != null)
                DataInsertEventHandler(this, new SQLInsertEventArgs(item));

            if (item is IOStockFormat)
                CalAddedFormatQty(item as IOStockFormat);
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

        public TableT Select<TableT>(string id) where TableT : class, IID, new()
        {
            string sql = string.Format("select * from {0} where {1} = '{2}';", typeof(TableT).Name, "ID", id);
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
            if (!sql.Contains("where") || !sql.Contains("*"))
                throw new ArgumentException();

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

        public void Update<TableT>(TableT item, bool handled = false) where TableT : class, IID
        {
            string sql = string.Format("update {0} set ", typeof(TableT).Name);
            StringBuilder sb = new StringBuilder(sql);
            var properties = typeof(TableT).GetProperties();
            Dictionary<string, object> update = new Dictionary<string, object>();
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
                update.Add(fieldName, value);
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(string.Format(" where {0} = '{1}';", nameof(item.ID), item.ID));
            sql = sb.ToString();

            ExecuteUpdate((object)item, sql, update, handled);
        }

        public void Update<TableT>(TableT item, string propertyName, object setValue, bool handled = false) where TableT : class, IID
        {
            item.GetType().GetProperty(propertyName).SetValue(item, setValue);
            Update<TableT>(item, propertyName, handled);
        }

        public void Update<TableT>(TableT item, string propertyName, bool handled = false) where TableT : class, IID
        {
            object value = typeof(TableT).GetProperty(propertyName).GetValue(item);
            if (value != null && value.GetType().IsEnum)
                value = (int)value;
            else if (value != null && value.GetType() == typeof(DateTime))
                value = ((DateTime)value).ToString(DATETIME);

            string sql = string.Format("update {0} set {1} = '{2}' where {3} = '{4}';", typeof(TableT).Name, propertyName, value, nameof(item.ID), item.ID);
            ExecuteUpdate((object)item, sql, new Dictionary<string, object>() { { propertyName, value } }, handled);
        }

        private void ExecuteUpdate(object item, string sql, Dictionary<string, object> update, bool handled)
        {
            if (item is IOStockFormat && update.Keys.Any(x => x == "Quantity"))
                CalModifiedFormatQty(item as IOStockFormat);

            Console.WriteLine(sql);
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
                cmd.ExecuteNonQuery();

            if (DataUpdateEventHandler != null && handled)
                DataUpdateEventHandler(this, new SQLUpdateEventArgs(item, update));
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

            if (DataDeleteEventHandler != null)
                DataDeleteEventHandler(this, new SQLDeleteEventArgs(typeof(TableT), item.ID));

            if (item is Product) //제품을 삭제하고자 한다면 관련 인벤토리 데이터를 삭제해야 할 터이고
            {
                Product product = item as Product;
                var inventories = Query<InventoryFormat>("select * from {0} where ProductID = '{1}';", typeof(InventoryFormat).Name, product.ID);
                inventories.ToList().ForEach(x => Delete<InventoryFormat>(x));
            }
            else if (item is InventoryFormat) //이어서 인벤토리 데이터를 삭제하고자 할 경우 이와 관련된 입춮고 데이터를 삭제해야함
            {
                InventoryFormat inv = item as InventoryFormat;
                var ioss = Query<IOStockFormat>("select * from {0} where InventoryID = '{1}';", typeof(IOStockFormat).Name, inv.ID);
                sql = string.Format("delete from {0} where InventoryID = '{1}'", typeof(IOStockFormat).Name, inv.ID);

                using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
                    cmd.ExecuteNonQuery();

                var ids = ioss.Select(x => x.ID).ToList(); //iostock id list
                if (DataDeleteEventHandler != null)
                    DataDeleteEventHandler(this, new SQLDeleteEventArgs(typeof(IOStockFormat), ids));
            }
            else if (item is Maker)
            {
                var inventories = Query<InventoryFormat>("select * from {0} where MakerID = '{1}'", typeof(InventoryFormat).Name, item.ID);
                inventories.ToList().ForEach(x => Update<InventoryFormat>(x, nameof(x.MakerID), null, true));
            }
            else if (item is Measure)
            {
                var inventories = Query<InventoryFormat>("select * from {0} where MeasureID = '{1}'", typeof(InventoryFormat).Name, item.ID);
                inventories.ToList().ForEach(x => Update<InventoryFormat>(x, nameof(x.MeasureID), null, true));
            }
            else if (item is IOStockFormat)
            {
                CalDeletedFormatQty(item as IOStockFormat);
            }
        }

        /// <summary>
        /// 새로운 IOStockFormat을 추가한 경우 잔여수량과 현재 재고량 다시 계산
        /// </summary>
        /// <param name="iosfmt"></param>
        private void CalAddedFormatQty(IOStockFormat iosfmt)
        {
            InventoryFormat infmt = Select<InventoryFormat>(iosfmt.InventoryID);
            if (infmt == null)
                return;

            int qty = iosfmt.Quantity;
            IOStockFormat near = null;

            IEnumerable<IOStockFormat> queryResult =
                Query<IOStockFormat>("select * from {0} where {1} = '{2}' and {3} < '{4}' order by {5} desc limit 1;",
                typeof(IOStockFormat).Name, nameof(iosfmt.InventoryID), iosfmt.InventoryID, nameof(iosfmt.Date), iosfmt.Date.ToString(DATETIME), nameof(iosfmt.Date));
            if (queryResult.Count() == 1)
                near = queryResult.Single();

            qty = iosfmt.StockType == IOStockType.OUTGOING ? -qty : qty;
            iosfmt.RemainingQuantity = (near == null) ? qty : near.RemainingQuantity + qty;

            Update<InventoryFormat>(infmt, nameof(infmt.Quantity), infmt.Quantity + qty, true);
            UpdateIOStockFormatRemainQty(iosfmt, iosfmt.RemainingQuantity);

            IEnumerable<IOStockFormat> iofmts = Query<IOStockFormat>("select * from {0} where {1} = '{2}' and {3} > '{4}' order by {5};",
                nameof(IOStockFormat), nameof(iosfmt.InventoryID), iosfmt.InventoryID, nameof(iosfmt.Date), iosfmt.Date.ToString(DATETIME), nameof(iosfmt.Date));

            if (qty != 0 && iofmts.Count() != 0)
            {
                foreach (IOStockFormat f in iofmts)
                    UpdateIOStockFormatRemainQty(f, f.RemainingQuantity + qty);
            }
        }

        private void CalModifiedFormatQty(IOStockFormat iosfmt)
        {
            InventoryFormat infmt = Select<InventoryFormat>(iosfmt.InventoryID);
            IOStockFormat origin = Select<IOStockFormat>(iosfmt.ID);

            int oQty = origin.Quantity;
            int fqty = iosfmt.Quantity;
            fqty = iosfmt.StockType == IOStockType.OUTGOING ? -fqty : fqty;
            oQty = iosfmt.StockType == IOStockType.OUTGOING ? oQty : -oQty;
            iosfmt.RemainingQuantity = origin.RemainingQuantity + fqty + oQty;
            Update<InventoryFormat>(infmt, nameof(infmt.Quantity), infmt.Quantity + fqty + oQty, true);
            UpdateIOStockFormatRemainQty(iosfmt, iosfmt.RemainingQuantity);

            IEnumerable<IOStockFormat> formats = Query<IOStockFormat>("select * from {0} where {1} = '{2}' and {3} > '{4}' order by {5};",
                nameof(IOStockFormat), nameof(iosfmt.InventoryID), iosfmt.InventoryID, nameof(iosfmt.Date), iosfmt.Date.ToString(DATETIME), nameof(iosfmt.Date));

            if (fqty != 0 && formats.Count() != 0)
            {
                foreach (IOStockFormat f in formats)
                    UpdateIOStockFormatRemainQty(f, f.RemainingQuantity + fqty + oQty);
            }
        }

        private void CalDeletedFormatQty(IOStockFormat iosfmt)
        {
            InventoryFormat infmt = Select<InventoryFormat>(iosfmt.InventoryID);
            if (infmt == null)
                return;

            int qty = iosfmt.Quantity;
            qty = iosfmt.StockType == IOStockType.OUTGOING ? qty : -qty;
            Update<InventoryFormat>(infmt, nameof(infmt.Quantity), infmt.Quantity + qty, true);

            ///잔여 수량 동기화 및 저장
            IEnumerable<IOStockFormat> formats = Query<IOStockFormat>("select * from {0} where {1} = '{2}' and {3} > '{4}' order by {5};",
                typeof(IOStockFormat).Name, nameof(iosfmt.InventoryID), iosfmt.InventoryID, nameof(iosfmt.Date), iosfmt.Date.ToString(DATETIME), nameof(iosfmt.Date));

            if (qty != 0 && formats.Count() != 0)
            {
                foreach (IOStockFormat f in formats)
                    UpdateIOStockFormatRemainQty(f, f.RemainingQuantity + qty);
            }
        }

        private void UpdateIOStockFormatRemainQty(IOStockFormat iosfmt, int qty)
        {
            string sql = string.Format("update {0} set {1} = '{2}' where {3} = '{4}';",
                nameof(IOStockFormat), nameof(iosfmt.RemainingQuantity), qty, nameof(iosfmt.ID), iosfmt.ID);

            Console.WriteLine(sql);

            using (SQLiteCommand cmd = new SQLiteCommand(sql, _conn))
                cmd.ExecuteNonQuery();

            if (DataUpdateEventHandler != null)
                DataUpdateEventHandler(this, new SQLUpdateEventArgs(iosfmt, nameof(iosfmt.RemainingQuantity), qty));
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