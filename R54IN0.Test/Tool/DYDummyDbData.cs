using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Test
{
    public class DYDummyDbData
    {
        string[] _clientNames = new string[]
        {
            "예시스템", "엠비콘넥터", "이오텍", "네오테크", "여명", "춘일", "덕성전기", "풍림", "춘일", "영도케이블",
            "동인", "미래조명", "제이엠시스"
        };

        string[] _humanNames = new string[]
        {
            "Allene",
            "Margart",
            "Marlon",
            "Jessika",
            "Terrie",
            "Eufemia",
            "Emma",
            "Peg",
            "Wai",
            "Zack",
            "Stanford",
            "Jayme",
            "Carter",
            "Suzanna",
            "Dania",
            "Haywood",
            "Donovan",
            "Antoine",
            "Minda",
            "Aurelio",
            "Pasquale",
            "Lucretia",
            "Flavia",
            "Jospeh",
            "Carmela",
            "Nieves",
            "Ardith",
            "Evelynn",
            "Sheryl",
            "Jovita",
            "Ardis",
            "Ella",
            "Kristyn",
            "Edelmira",
            "Carmelita",
            "Laila",
            "Jeanna" };

        string[] _currencyNames = new string[]
        {
            "원",
        };

        string[] _makerNames = new string[]
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

        string[] _measureNames = new string[]
        {
            "EA",
            "BOX",
            "PIC",
            "SET",
        };

        string[] _warehouseNames = new string[]
        {
            "1층 적재 A구역",
            "1층 적재 B구역",
            "1층 적재 C구역",
            "1층 적재 D구역",
            "연구실",
            "사무실",
            "회사 창고",
        };

        Dictionary<string, string[]> _itemNames = new Dictionary<string, string[]>()
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

        Random _random = new Random((int)DateTime.Now.Ticks);

        void CreateClient()
        {
            foreach (var name in _clientNames)
            {
                new Client()
                {
                    Name = name,
                    MobileNumber = "010" + _random.Next(1000, 9990).ToString() + _random.Next(1000, 9990).ToString(),
                    PhoneNumber = _random.Next(100, 999).ToString() + _random.Next(1000, 9990).ToString(),
                    Delegator = _humanNames.Random(),
                }.Save<Client>();
            }
        }

        void CreateEmployee()
        {
            foreach (var name in _humanNames)
            {
                new Employee()
                {
                    Name = name
                }.Save<Employee>();
            }
        }

        void CreateCurrency()
        {
            foreach (var name in _currencyNames)
            {
                new Currency()
                {
                    Name = name
                }.Save<Currency>();
            }
        }

        void CreateMaker()
        {
            foreach (var name in _makerNames)
            {
                new Maker()
                {
                    Name = name
                }.Save<Maker>();
            }
        }

        void CreateMeasure()
        {
            foreach (var name in _measureNames)
            {
                new Measure()
                {
                    Name = name
                }.Save<Measure>();
            }
        }

        void CreateWarehouse()
        {
            foreach (var name in _warehouseNames)
            {
                new Warehouse()
                {
                    Name = name
                }.Save<Warehouse>();
            }
        }

        void CreatItem()
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Currency[] currencies = db.LoadAll<Currency>();
                Measure[] measures = db.LoadAll<Measure>();
                Maker[] makers = db.LoadAll<Maker>();

                foreach (var itemName in _itemNames)
                {
                    string itemname = itemName.Key;

                    var item = new Item()
                    {
                        CurrencyUUID = currencies.Random().UUID,
                        MeasureUUID = measures.Random().UUID,
                        MakerUUID = makers.Random().UUID,
                        Name = itemname
                    }.Save<Item>();

                    foreach (var specName in itemName.Value)
                    {
                        new Specification()
                        {
                            ItemUUID = item.UUID,
                            PurchaseUnitPrice = _random.Next(1000, 1000000),
                            SalesUnitPrice = _random.Next(1000, 1000000),
                            Name = specName,
                        }.Save<Specification>();
                    }
                }
            }
        }

        int CreateInventory(int min = 4, int max = 50)
        {
            int cnt = 0;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Client[] clients = db.LoadAll<Client>();
                Employee[] employees = db.LoadAll<Employee>();
                Warehouse[] warehouses = db.LoadAll<Warehouse>();
                Item[] items = db.LoadAll<Item>();
                Specification[] specs = db.LoadAll<Specification>();

                foreach(var spec in specs)
                {
                    var inventory = new Inventory()
                    {
                        ItemUUID = spec.ItemUUID,
                        Quantity = _random.Next(200, 500),
                        SpecificationUUID = spec.UUID,
                        WarehouseUUID = warehouses.Random().UUID
                    }.Save<Inventory>();

                    DateTime date = DateTime.Now;
                    for (int i = 0; i < _random.Next(min, max); i++)
                    {
                        cnt++;
                        new InOutStock()
                        {
                            Date = date.AddDays(_random.NextDouble() * 1000.0),
                            EmployeeUUID = employees.Random().UUID,
                            InventoryUUID = inventory.UUID,
                            EnterpriseUUID = clients.Random().UUID,
                            ItemUUID = spec.ItemUUID,
                            SpecificationUUID = spec.UUID,
                            Quantity = _random.Next(1, 1000),
                            StockType = _random.Next(0, 2) == 0 ? StockType.INCOMING : StockType.OUTGOING,
                            Remark = Guid.NewGuid().ToString()
                        }.Save<InOutStock>();
                    }
                }
            }
            return cnt;
        }

        public DYDummyDbData Create(int min = 4, int max = 50)
        {
            CreateClient();
            CreateEmployee();
            CreateCurrency();
            CreateMaker();
            CreateMeasure();
            CreateWarehouse();
            CreatItem();
            int cnt = CreateInventory(min, max);
            Debug.WriteLine("생성된 입출고 데이터: " + cnt);
            return this;
        }
    }
}
