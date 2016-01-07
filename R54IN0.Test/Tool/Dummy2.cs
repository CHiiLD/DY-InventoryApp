using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Test
{
    public class Dummy2
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

        string[] _projects = new string[] 
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

        Random _random = new Random((int)DateTime.Now.Ticks);

        void CreateClient()
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

        void CreateProject()
        {
            foreach (var name in _projects)
            {
                new Project()
                {
                    Name = name
                }.Save<Project>();
            }
        }

        public void Create()
        {
            ////////////INIT
            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            CollectionViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            StockWrapperDirector.Distory();
            LexDb.Distroy();
            ObservableInvenDirector.Distory();
            ObservableFieldDirector.Distory();

            using (var db = LexDb.GetDbInstance())
                db.Purge();

            CreateClient();
            CreateMaker();
            CreateMeasure();
            CreateWarehouse();
            CreateProject();

            using (var db = LexDb.GetDbInstance())
            {
                Maker[] makers = db.LoadAll<Maker>();
                Measure[] measures = db.LoadAll<Measure>();
                Customer[] customer = db.LoadAll<Customer>();
                Supplier[] suppliers = db.LoadAll<Supplier>();
                Project[] proejcts = db.LoadAll<Project>();

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
                        int cnt = _random.Next(4, 20);

                        for(int i = 0; i < cnt; i ++)
                        {
                            var date1 = DateTime.Now.AddDays(-600.0 / (i + 1));
                            var date2 = date1.AddMilliseconds(1);

                            var isfmt = new InoutStockFormat()
                            {
                                SupplierID = suppliers.Random().ID,
                                Date = date1,
                                InventoryItemID = ifmt.ID,
                                ProjectID = proejcts.Random().ID,
                                Quantity = _random.Next(10, 100),
                                StockType = StockType.INCOMING,
                                UnitPrice = (int)((_random.NextDouble() + 0.5) * _random.Next(1000, 100000)),
                            }.Save<InoutStockFormat>();
                            qty += isfmt.Quantity;

                            isfmt = new InoutStockFormat()
                            {
                                CustomerID = customer.Random().ID,
                                Date = date2,
                                InventoryItemID = ifmt.ID,
                                ProjectID = proejcts.Random().ID,
                                Quantity = _random.Next(1, qty),
                                StockType = StockType.OUTGOING,
                                UnitPrice = (int)((_random.NextDouble() + 0.5) * _random.Next(1000, 100000)),
                            }.Save<InoutStockFormat>();
                            qty -= isfmt.Quantity;
                        }
                        ifmt.Quantity = qty;
                        ifmt.Save<InventoryFormat>();
                    }
                }
            }
        }
    }
}
