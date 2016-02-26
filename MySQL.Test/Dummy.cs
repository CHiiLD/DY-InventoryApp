using MySql.Data.MySqlClient;
using R54IN0;
using System;
using System.Collections.Generic;
using System.Text;

namespace MySQL.Test
{
    public partial class Dummy
    {
        private MySqlConnection _conn;

        public Dummy(MySqlConnection conn)
        {
            this._conn = conn;
        }

        public void Create()
        {
            Console.WriteLine("더미 데이터 생성");
            InitTables();
            SetData();
        }

        private void SetData()
        {
            Insert(nameof(Customer), _clientNames);
            Insert(nameof(Supplier), _clientNames);
            Insert(nameof(Employee), _humanNames);
            Insert(nameof(Maker), _makerNames);
            Insert(nameof(Measure), _measureNames);
            Insert(nameof(Project), _projectNames);
            Insert(nameof(Warehouse), _warehouseNames);
            InsertInven();
            InsertIOStock();
        }

        private void InitTables()
        {
            List<string> tables = new List<string>();
            using (MySqlCommand showCmd = new MySqlCommand("show tables;", _conn))
            using (MySqlDataReader reader = showCmd.ExecuteReader())
            {
                while (reader.Read())
                    tables.Add(reader.GetString(0));
            }
            foreach (var table in tables)
            {
                string sql = string.Format("drop tables {0};", table);
                using (MySqlCommand dropCmd = new MySqlCommand(sql, _conn))
                    dropCmd.ExecuteNonQuery();
            }
            //Table 생성
            CreateTable<InventoryFormat>(_conn);
            CreateTable<IOStockFormat>(_conn);
            CreateTable<Customer>(_conn);
            CreateTable<Employee>(_conn);
            CreateTable<Maker>(_conn);
            CreateTable<Measure>(_conn);
            CreateTable<Product>(_conn);
            CreateTable<Project>(_conn);
            CreateTable<Supplier>(_conn);
            CreateTable<Warehouse>(_conn);
        }

        private void CreateTable<TableT>(MySqlConnection conn)
        {
            string sql = string.Format("create table {0} (", typeof(TableT).Name);
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

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                cmd.ExecuteNonQuery();
        }

        private void Insert(string tablename, IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                string sql = string.Format("insert into {0} ({1}, {2}) values ('{3}', '{4}');", tablename,
                   "ID", "Name", Guid.NewGuid().ToString(), name);
                using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                    cmd.ExecuteNonQuery();
            }
        }

        private List<string> Select(string tablename)
        {
            List<string> list = new List<string>();
            string sql = string.Format("select ID from {0};", tablename);
            using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    list.Add(reader.GetString(0));
            }
            return list;
        }

        private void InsertIOStock()
        {
            var customer = Select(nameof(Customer));
            var suppliers = Select(nameof(Supplier));
            var projects = Select(nameof(Project));
            var employees = Select(nameof(Employee));
            var warehouse = Select(nameof(Warehouse));
            var invens = Select(nameof(InventoryFormat));

            var r = new Random();
            const string DATETIME = "yyyy-MM-dd HH:mm:ss.fff";

            foreach (var inven in invens)
            {
                int count = r.Next(5, 10);
                string sql;
                for (int i = 0; i < count; i++)
                {
                    var date1 = DateTime.Now.AddDays(-600.0 / (count - i + 1)).AddDays(-1);
                    var date2 = date1.AddMilliseconds(1000);

                    sql = string.Format(
                        @"insert into {0} (ID, InventoryID, SupplierID, WarehouseID, EmployeeID, Quantity, Date, StockType, UnitPrice)
                        values ('{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');",
                        nameof(IOStockFormat), Guid.NewGuid().ToString(), inven, suppliers.Random(), warehouse.Random(), employees.Random(),
                        r.Next(1, 10), date1.ToString(DATETIME), (int)IOStockType.INCOMING, 1000);

                    using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                        cmd.ExecuteNonQuery();

                    sql = string.Format(
                        @"insert into {0} (ID, InventoryID, CustomerID, ProjectID, EmployeeID, Quantity, Date, StockType, UnitPrice)
                        values ('{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');",
                        nameof(IOStockFormat), Guid.NewGuid().ToString(), inven, customer.Random(), projects.Random(), employees.Random(),
                        r.Next(1, 10), date2.ToString(DATETIME), (int)IOStockType.OUTGOING, 1200);

                    using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                        cmd.ExecuteNonQuery();
                }

                sql = string.Format("update {0} set Quantity = (select sum(Quantity) from {1} where InventoryID = '{2}' and StockType = '{3}') - (select sum(Quantity) from {4} where InventoryID = '{5}' and StockType = '{6}') where ID = '{7}';",
                    nameof(InventoryFormat),
                    nameof(IOStockFormat), inven, (int)IOStockType.INCOMING,
                    nameof(IOStockFormat), inven, (int)IOStockType.OUTGOING,
                    inven);
                using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                    cmd.ExecuteNonQuery();
            }
        }

        private void InsertInven()
        {
            var makers = Select(nameof(Maker));
            var measures = Select(nameof(Measure));

            foreach (var product in _productNames)
            {
                string prodID = Guid.NewGuid().ToString();
                string sql = string.Format("insert into {0} ({1}, {2}) values ('{3}', '{4}');", nameof(Product),
                  "ID", "Name", prodID, product.Key);
                using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                    cmd.ExecuteNonQuery();

                foreach (var inventory in product.Value)
                {
                    string invID = Guid.NewGuid().ToString();
                    sql = string.Format("insert into {0} (ID, ProductID, MakerID, MeasureID, Quantity, Specification) values ('{1}', '{2}', '{3}', '{4}', '{5}', '{6}');",
                    nameof(InventoryFormat), invID, prodID, makers.Random(), measures.Random(), 0, inventory);
                    using (MySqlCommand cmd = new MySqlCommand(sql, _conn))
                        cmd.ExecuteNonQuery();
                }
            }
        }
    }
}