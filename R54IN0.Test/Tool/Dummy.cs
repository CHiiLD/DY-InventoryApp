using R54IN0.WPF;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace R54IN0.Test
{
    public class Dummy
    {
        private string[] _clientNames = new string[]
        {
            "예시스템", "엠비콘넥터", "이오텍", "네오테크", "여명", "춘일", "덕성전기", "풍림", "춘일", "영도케이블",
            "동인", "미래조명", "제이엠시스"
        };

        private string[] _humanNames = new string[]
        {
            "Emma",
            "Peg",
            "Wai",
            "Zack",
            "Stanford",
            "Jayme",
            "Carter",
            "Laila",
            "Jeanna" };

        private string[] _makerNames = new string[]
        {
            "LG",
            "선트로닉스",
            "LSIS",
            "미쯔비시",
            "한영",
            "위너스",
            "캐논",
            "슈나이더",
            "건홍",
            "세기비즈",
            "동아",
            "금호전기",
            "장안",
            "KD POWER",
            "번개표",
            "Q-LIGHT",
        };

        private string[] _measureNames = new string[]
        {
            "EA",
            "BOX",
            "PIC",
            "SET",
        };

        private string[] _warehouseNames = new string[]
        {
            "1층 적재 A구역",
            "1층 적재 B구역",
            "1층 적재 C구역",
            "1층 적재 D구역",
            "연구실",
            "사무실",
            "회사 창고",
        };

        private Dictionary<string, string[]> _itemNames = new Dictionary<string, string[]>()
        {
            { "플러그", new string[] { "EPP2-21010" } },
            { "커버", new string[] { "HP119E", "HP111", "HP117" } },
            { "버섯형 누름 버튼", new string[] { "ZB5 AC3" } },
            { "버튼 단자부", new string[] { "ZB5-AZ101", "ZB5-AZ104" } },
            { "설렉터 스위치", new string[] { "ZB5AS844" } },
            { "Key 스위치", new string[] { "ZB5 AG4" } },
            { "PBL", new string[] { "ZB5 AW233" } },
            { "단자부", new string[] { "ZB5 AW343", "ZB5 AWBB451" } },
            { "SWTICH BOX", new string[] {
                "KCB-304D", "KCB-303D", "KCB-302D", "KCB-301D",
                "KCB-300D", "KCB-393D", "KCB-200D", "KCB-232D",
                "KCB-104D", "KCB-103D", "KCB-102D", "KCB-101D",} },
            { "등기구", new string[] { "LFX12", "LFX06" } },
            { "판넬 직부등", new string[] { "KCL-100 삼파장" } },
        };

        private string[] _projects = new string[]
        {
            "DY1234",
            "DY1235",
            "DY1236",
            "DY1237",
            "DY1238",
            "DY1239",
            "DY111234",
            "DY1231234",
            "DY3331234",
            "DY3211234",
            "DY12321234",
        };

        private Random _random = new Random((int)DateTime.Now.Ticks);

        private void CreateClient()
        {
            foreach (var name in _clientNames)
            {
                new Customer()
                {
                    Name = name,
                }.Save<Customer>();
                new Supplier()
                {
                    Name = name,
                }.Save<Supplier>();
            }
        }

        private void CreateMaker()
        {
            foreach (var name in _makerNames)
            {
                new Maker()
                {
                    Name = name
                }.Save<Maker>();
            }
        }

        private void CreateMeasure()
        {
            foreach (var name in _measureNames)
            {
                new Measure()
                {
                    Name = name
                }.Save<Measure>();
            }
        }

        private void CreateWarehouse()
        {
            foreach (var name in _warehouseNames)
            {
                new Warehouse()
                {
                    Name = name
                }.Save<Warehouse>();
            }
        }

        private void CreateProject()
        {
            foreach (var name in _projects)
            {
                new Project()
                {
                    Name = name
                }.Save<Project>();
            }
        }

        private void CreateEmployee()
        {
            foreach (var name in _humanNames)
            {
                new Employee()
                {
                    Name = name
                }.Save<Employee>();
            }
        }

        private void CreateData()
        {
            var makers = InventoryDataCommander.GetInstance().DB.Select<Maker>();
            var measures = InventoryDataCommander.GetInstance().DB.Select<Measure>();
            var customer = InventoryDataCommander.GetInstance().DB.Select<Customer>();
            var suppliers = InventoryDataCommander.GetInstance().DB.Select<Supplier>();
            var proejcts = InventoryDataCommander.GetInstance().DB.Select<Project>();
            var employees = InventoryDataCommander.GetInstance().DB.Select<Employee>();
            var warehouse = InventoryDataCommander.GetInstance().DB.Select<Warehouse>();

            foreach (var item in _itemNames)
            {
                var p = new Product()
                {
                    Name = item.Key
                }.Save<Product>();
                foreach (var item2 in item.Value)
                {
                    InventoryFormat ifmt = new InventoryFormat()
                    {
                        ProductID = p.ID,
                        Specification = item2,
                        Quantity = 0,
                        MeasureID = measures.Random().ID,
                        MakerID = makers.Random().ID
                    }.Save<InventoryFormat>();

                    int qty = 0;
                    int cnt = _random.Next(2, 5);

                    for (int i = 0; i < cnt; i++)
                    {
                        var date1 = DateTime.Now.AddDays(-600.0 / cnt * (cnt - i)).AddMilliseconds(-1000);
                        var date2 = date1.AddMilliseconds(500);
                        var price1 = ((int)((_random.NextDouble() + 0.5) * _random.Next(1000, 100000))) / 1000 * 1000;
                        var price2 = ((int)((_random.NextDouble() + 0.5) * _random.Next(1000, 100000))) / 1000 * 1000;

                        var incoming = _random.Next(10, 100);
                        var isfmt = new IOStockFormat()
                        {
                            SupplierID = suppliers.Random().ID,
                            Date = date1,
                            InventoryID = ifmt.ID,
                            WarehouseID = warehouse.Random().ID,
                            Quantity = incoming,
                            RemainingQuantity = qty + incoming,
                            StockType = IOStockType.INCOMING,
                            EmployeeID = employees.Random().ID,
                            UnitPrice = price1,
                        }.Save<IOStockFormat>();
                        qty += isfmt.Quantity;

                        var outgoing = _random.Next(1, qty);

                        isfmt = new IOStockFormat()
                        {
                            CustomerID = customer.Random().ID,
                            Date = date2,
                            InventoryID = ifmt.ID,
                            ProjectID = proejcts.Random().ID,
                            Quantity = outgoing,
                            RemainingQuantity = qty - outgoing,
                            StockType = IOStockType.OUTGOING,
                            EmployeeID = employees.Random().ID,
                            UnitPrice = price2,
                        }.Save<IOStockFormat>();
                        qty -= isfmt.Quantity;
                    }
                    ifmt.Quantity = qty;
                    InventoryDataCommander.GetInstance().DB.Update(ifmt, "Quantity");
                }
            }
        }

        public void Create()
        {
            InventoryDataCommander.Destroy();
            CollectionViewModelObserverSubject.Destory();
            MainWindowViewModel.Destory();
            TreeViewNodeDirector.Destroy();

            string connstr = string.Format("Data Source={0}", SQLiteServer.DATASOURCE);
            var conn = new SQLiteConnection(connstr);
            conn.Open();
            string sql = "select name from sqlite_master;";
            Console.WriteLine(sql);
            List<string> tablenames = new List<string>();
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
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
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    cmd.ExecuteNonQuery();
            }
            conn.Close();

            CreateClient();
            CreateMaker();
            CreateMeasure();
            CreateWarehouse();
            CreateProject();
            CreateEmployee();
            CreateData();

            InventoryDataCommander.Destroy();
        }
    }
}