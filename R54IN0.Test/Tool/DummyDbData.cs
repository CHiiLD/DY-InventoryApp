using System;

namespace R54IN0.Test
{
    public class DummyDbData
    {
        string _testItemUUID;
        public string UnregisterdTestItemUUID
        {
            get
            {
                if (_testItemUUID == null)
                    _testItemUUID = "20920920320";
                return _testItemUUID;
            }
        }

        public DummyDbData Create()
        {
            ////////////INIT

            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            ViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            IOStockWrapperDirector.Distory();

            ////////////INIT

            using (var db = DatabaseDirector.GetDbInstance())
            {
                db.Purge();
            }

            var e = new Employee() { Name = "지창훈" }.Save<Employee>();
            new Employee() { Name = "박재현" }.Save<Employee>();
            new Employee() { Name = "김택윤" }.Save<Employee>();
            new Employee() { Name = "김연청" }.Save<Employee>();
            new Employee() { Name = "천두관" }.Save<Employee>();
            new Employee() { Name = "이도희" }.Save<Employee>();

            var a = new Account() { Name = "야명FA", Delegator = "이양원", PhoneNumber = "053) 604-5656" }.Save<Account>();
            new Account() { Name = "예시스템", Delegator = "지창훈", PhoneNumber = "053) 587-3013" }.Save<Account>();
            new Account() { Name = "이오텍", Delegator = "김택윤", PhoneNumber = "053) 600-8220" }.Save<Account>();
            new Account() { Name = "네오테크", Delegator = "김연청", PhoneNumber = "054) 275-8403" }.Save<Account>();
            new Account() { Name = "덕성전기", Delegator = "천두관", PhoneNumber = "053) 604-4242" }.Save<Account>();
            new Account() { Name = "풍림", Delegator = "이도희", PhoneNumber = "053) 604-4321" }.Save<Account>();

            var m = new Measure() { Name = "EA" }.Save<Measure>();
            new Measure() { Name = "SET" }.Save<Measure>();

            new Warehouse() { Name = "1층" }.Save<Warehouse>();
            var w = new Warehouse() { Name = "연구실" }.Save<Warehouse>();

            var c = new Currency() { Name = "원" }.Save<Currency>();
            new Currency() { Name = "미국달러" }.Save<Currency>();

            var gunhng = new Maker() { Name = "간흥" }.Save<Maker>();
            var hanyung = new Maker() { Name = "한영일넉스" }.Save<Maker>();
            var shunaider = new Maker() { Name = "스나이더" }.Save<Maker>();

            int pPrice = 1000;
            int sPrice = 1400;

            var item = new Item() {
                UUID = UnregisterdTestItemUUID,
                Name = "테스트용 아이템정보", MakerUUID = hanyung.UUID, CurrencyUUID = c.UUID, MeasureUUID = m.UUID }.Save<Item>();
            var spec = new Specification() { Remark = "테스트용 규격정보_1", Name = "테스트용 규격정보_1", ItemUUID = item.UUID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            spec = new Specification() { Remark = "테스트용 규격정보_2", Name = "테스트용 규격정보_2", ItemUUID = item.UUID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();

            item = new Item() { Name = "PBL", MakerUUID = hanyung.UUID, CurrencyUUID = c.UUID, MeasureUUID = m.UUID }.Save<Item>();
            spec = new Specification() { Remark = "녹색", Name = "CR254-24V 녹색", ItemUUID = item.UUID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();

            new Inventory()
            {
                ItemCount = 10,
                Remark = "",
                SpecificationUUID = spec.UUID,
                WarehouseUUID = w.UUID,
                ItemUUID = item.UUID
            }.Save<Inventory>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeUUID = e.UUID,
                EnterpriseUUID = a.UUID,
                ItemCount = 30,
                ItemUUID = item.UUID,
                SpecificationUUID = spec.UUID,
                StockType = StockType.IN,
                WarehouseUUID = w.UUID
            }.Save<InOutStock>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeUUID = e.UUID,
                EnterpriseUUID = a.UUID,
                ItemCount = 30,
                ItemUUID = item.UUID,
                SpecificationUUID = spec.UUID,
                StockType = StockType.IN,
                WarehouseUUID = w.UUID
            }.Save<InOutStock>();

            spec = new Specification() { Remark = "청색", Name = "CR254-24V 청색", ItemUUID = item.UUID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();

            spec = new Specification() { Remark = "황색", Name = "CR254-24V 황색", ItemUUID = item.UUID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            new Inventory()
            {
                ItemCount = 10,
                Remark = "",
                SpecificationUUID = spec.UUID,
                WarehouseUUID = w.UUID,
                ItemUUID = item.UUID
            }.Save<Inventory>();
            item = new Item() { Name = "스위치 박스", MakerUUID = gunhng.UUID, CurrencyUUID = c.UUID, MeasureUUID = m.UUID }.Save<Item>();
            spec = new Specification() { Remark = "1", Name = "KCB-304D", ItemUUID = item.UUID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeUUID = e.UUID,
                EnterpriseUUID = a.UUID,
                ItemCount = 30,
                ItemUUID = item.UUID,
                SpecificationUUID = spec.UUID,
                StockType = StockType.OUT,
                WarehouseUUID = w.UUID
            }.Save<InOutStock>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeUUID = e.UUID,
                EnterpriseUUID = a.UUID,
                ItemCount = 30,
                ItemUUID = item.UUID,
                SpecificationUUID = spec.UUID,
                StockType = StockType.IN,
                WarehouseUUID = w.UUID
            }.Save<InOutStock>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeUUID = e.UUID,
                EnterpriseUUID = a.UUID,
                ItemCount = 30,
                ItemUUID = item.UUID,
                SpecificationUUID = spec.UUID,
                StockType = StockType.IN,
                WarehouseUUID = w.UUID
            }.Save<InOutStock>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeUUID = e.UUID,
                EnterpriseUUID = a.UUID,
                ItemCount = 30,
                ItemUUID = item.UUID,
                SpecificationUUID = spec.UUID,
                StockType = StockType.IN,
                WarehouseUUID = w.UUID
            }.Save<InOutStock>();

            new Inventory()
            {
                ItemCount = 10,
                Remark = "",
                SpecificationUUID = spec.UUID,
                WarehouseUUID = w.UUID,
                ItemUUID = item.UUID
            }.Save<Inventory>();
            spec = new Specification() { Remark = "2", Name = "KCB-303D", ItemUUID = item.UUID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            new Inventory()
            {
                ItemCount = 10,
                Remark = "",
                SpecificationUUID = spec.UUID,
                WarehouseUUID = w.UUID,
                ItemUUID = item.UUID
            }.Save<Inventory>();
            spec = new Specification() { Remark = "3", Name = "KCB-302D", ItemUUID = item.UUID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            new Inventory()
            {
                ItemCount = 10,
                Remark = "",
                SpecificationUUID = spec.UUID,
                WarehouseUUID = w.UUID,
                ItemUUID = item.UUID
            }.Save<Inventory>();
            spec = new Specification() { Remark = "4", Name = "KCB-301D", ItemUUID = item.UUID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            new Inventory()
            {
                ItemCount = 10,
                Remark = "",
                SpecificationUUID = spec.UUID,
                WarehouseUUID = w.UUID,
                ItemUUID = item.UUID
            }.Save<Inventory>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeUUID = e.UUID,
                EnterpriseUUID = a.UUID,
                ItemCount = 30,
                ItemUUID = item.UUID,
                SpecificationUUID = spec.UUID,
                StockType = StockType.OUT,
                WarehouseUUID = w.UUID
            }.Save<InOutStock>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeUUID = e.UUID,
                EnterpriseUUID = a.UUID,
                ItemCount = 30,
                ItemUUID = item.UUID,
                SpecificationUUID = spec.UUID,
                StockType = StockType.IN,
                WarehouseUUID = w.UUID
            }.Save<InOutStock>();

            spec = new Specification() { Remark = "5", Name = "KCB-254D", ItemUUID = item.UUID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            new Inventory()
            {
                ItemCount = 10,
                Remark = "",
                SpecificationUUID = spec.UUID,
                WarehouseUUID = w.UUID,
                ItemUUID = item.UUID
            }.Save<Inventory>();
            spec = new Specification() { Remark = "6", Name = "KCB-253D", ItemUUID = item.UUID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            new Inventory()
            {
                ItemCount = 10,
                Remark = "",
                SpecificationUUID = spec.UUID,
                WarehouseUUID = w.UUID,
                ItemUUID = item.UUID
            }.Save<Inventory>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeUUID = e.UUID,
                EnterpriseUUID = a.UUID,
                ItemCount = 30,
                ItemUUID = item.UUID,
                SpecificationUUID = spec.UUID,
                StockType = StockType.IN,
                WarehouseUUID = w.UUID
            }.Save<InOutStock>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeUUID = e.UUID,
                EnterpriseUUID = a.UUID,
                ItemCount = 30,
                ItemUUID = item.UUID,
                SpecificationUUID = spec.UUID,
                StockType = StockType.OUT,
                WarehouseUUID = w.UUID
            }.Save<InOutStock>();

            item = new Item() { Name = "단자부", MakerUUID = shunaider.UUID, CurrencyUUID = c.UUID, MeasureUUID = m.UUID }.Save<Item>();
            spec = new Specification() { Remark = "7", Name = "2B 접점", ItemUUID = item.UUID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            new Inventory()
            {
                ItemCount = 10,
                Remark = "",
                SpecificationUUID = spec.UUID,
                WarehouseUUID = w.UUID,
                ItemUUID = item.UUID
            }.Save<Inventory>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeUUID = e.UUID,
                EnterpriseUUID = a.UUID,
                ItemCount = 30,
                ItemUUID = item.UUID,
                SpecificationUUID = spec.UUID,
                StockType = StockType.IN,
                WarehouseUUID = w.UUID
            }.Save<InOutStock>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeUUID = e.UUID,
                EnterpriseUUID = a.UUID,
                ItemCount = 30,
                ItemUUID = item.UUID,
                SpecificationUUID = spec.UUID,
                StockType = StockType.OUT,
                WarehouseUUID = w.UUID
            }.Save<InOutStock>();

            //스트레스 테스트
#if false
            for (int i = 0; i < 1000; i++)
            {
                new InOutStock()
                {
                    Date = DateTime.Now,
                    EmployeeUUID = e.UUID,
                    EnterpriseUUID = a.UUID,
                    ItemCount = 30,
                    ItemUUID = item.UUID,
                    SpecificationUUID = spec.UUID,
                    StockType = StockType.OUT,
                    WarehouseUUID = w.UUID
                }.Save<InOutStock>();
            }
#endif

            return this;
        }
    }
}
