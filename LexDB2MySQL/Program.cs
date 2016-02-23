using Lex.Db;
using MySql.Data.MySqlClient;
using R54IN0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LexDB2MySQL
{
    class Program
    {
        public const string DATETIME = "yyyy-MM-dd HH:mm:ss.fff";

        static DbInstance LoadLexDb(string lexdbPath)
        {
            DbInstance me = new DbInstance(lexdbPath);
            me.Map<Employee>().Automap(i => i.ID).
            WithIndex("IsDeleted", i => i.IsDeleted).
            WithIndex("Name", i => i.Name);
            me.Map<Maker>().Automap(i => i.ID).
                WithIndex("IsDeleted", i => i.IsDeleted).
            WithIndex("Name", i => i.Name);
            me.Map<Measure>().Automap(i => i.ID).
                WithIndex("IsDeleted", i => i.IsDeleted).
            WithIndex("Name", i => i.Name);
            me.Map<Warehouse>().Automap(i => i.ID).
                WithIndex("IsDeleted", i => i.IsDeleted).
            WithIndex("Name", i => i.Name);
            me.Map<Project>().Automap(i => i.ID).
                WithIndex("IsDeleted", i => i.IsDeleted).
            WithIndex("Name", i => i.Name);
            me.Map<Product>().Automap(i => i.ID).
                WithIndex("IsDeleted", i => i.IsDeleted).
            WithIndex("Name", i => i.Name);
            me.Map<Customer>().Automap(i => i.ID).
                WithIndex("IsDeleted", i => i.IsDeleted).
            WithIndex("Name", i => i.Name);
            me.Map<Supplier>().Automap(i => i.ID).
                WithIndex("IsDeleted", i => i.IsDeleted).
            WithIndex("Name", i => i.Name);

            me.Map<InventoryFormat>().Automap(i => i.ID).
            WithIndex("MeasureID", i => i.MeasureID).
            WithIndex("ProductID", i => i.ProductID).
            WithIndex("Specification", i => i.Specification).
            WithIndex("Memo", i => i.Memo).
            WithIndex("MakerID", i => i.MakerID).
            WithIndex("Quantity", i => i.Quantity);

            me.Map<IOStockFormat>().Automap(i => i.ID).
            WithIndex("CustomerID", i => i.CustomerID).
            WithIndex("SupplierID", i => i.SupplierID).
            WithIndex("Date", i => i.Date).
            WithIndex("InventoryID", i => i.InventoryID).
            WithIndex("Memo", i => i.Memo).
            WithIndex("WarehouseID", i => i.WarehouseID).
            WithIndex("EmployeeID", i => i.EmployeeID).
            WithIndex("ProjectID", i => i.ProjectID).
            WithIndex("Quantity", i => i.Quantity).
            WithIndex("StockType", i => i.StockType).
            WithIndex("RemainingQuantity", i => i.RemainingQuantity).
            WithIndex("UnitPrice", i => i.UnitPrice);

            me.Initialize();

            return me;
        }

        static object ConvertSqlValue(object value)
        {
            var result = value;

            if (result is DateTime)
                result = ((DateTime)value).ToString(DATETIME);
            else if (result is Enum)
                result = (int)value;

            return result;
        }

        static string CreateInsertSQLClause<TableT>(TableT item)
        {
            string sql = string.Format("insert into {0} (", typeof(TableT).Name);
            StringBuilder sb0 = new StringBuilder(sql);
            StringBuilder sb1 = new StringBuilder(") values (");
            PropertyInfo[] properties = typeof(TableT).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                string name = property.Name;
                if (name == "IsDeleted" || name == "RemainingQuantity")
                    continue;

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

        static void Insert<TableT>(MySqlConnection conn, DbInstance db) where TableT : class, new()
        {
            var items = db.Table<TableT>().LoadAll();
            foreach (var item in items)
            {
                string sql = CreateInsertSQLClause(item);
                Console.WriteLine(sql);
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    cmd.ExecuteNonQuery();

                if (typeof(TableT) == typeof(InventoryFormat))
                {
                    string invID = ((IID)item).ID;

                    sql = string.Format(@"update {0} set Quantity =
                    ifnull((select sum(Quantity) from {1} where InventoryID = '{2}' and StockType = '{3}'), 0) -
                    ifnull((select sum(Quantity) from {4} where InventoryID = '{5}' and StockType = '{6}'), 0)
                    where ID = '{7}';",
                    nameof(InventoryFormat),
                    nameof(IOStockFormat), invID, (int)IOStockType.INCOMING,
                    nameof(IOStockFormat), invID, (int)IOStockType.OUTGOING,
                    invID);
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        cmd.ExecuteNonQuery();
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Please input MySqlConnection constructor argument");
            string connstr = Console.ReadLine();

            MySqlConnection conn = new MySqlConnection(connstr);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection fail");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("MySql connection success");

            /////////////////////////////////////////////////////////////////

            string lexdbPath = null;
            do
            {
                Console.WriteLine("Please input LexDb directory path");
                lexdbPath = Console.ReadLine();
                if (!System.IO.Directory.Exists(lexdbPath))
                {
                    Console.WriteLine("Can't found file!");
                    continue;
                }
            } while (false);

            DbInstance db = null;
            try
            {
                db = LoadLexDb(lexdbPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Lex DB Initialize fail");
                Console.ReadKey();
                conn.Close();
                return;
            }
            /////////////////////////////////////////////////////////////////

            Insert<Customer>(conn, db);
            Insert<Employee>(conn, db);
            Insert<Maker>(conn, db);
            Insert<Measure>(conn, db);
            Insert<Product>(conn, db);
            Insert<Project>(conn, db);
            Insert<Supplier>(conn, db);
            Insert<Warehouse>(conn, db);
            Insert<InventoryFormat>(conn, db);
            Insert<IOStockFormat>(conn, db);

            conn.Close();
            db.Dispose();

            Console.WriteLine("Success");
            Console.ReadKey();
        }
    }
}
