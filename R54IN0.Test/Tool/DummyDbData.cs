using System;

namespace R54IN0.Test
{
    public class DummyDbData
    {
        string _testItemID;
        public string UnregisterdTestItemID
        {
            get
            {
                if (_testItemID == null)
                    _testItemID = "20920920320";
                return _testItemID;
            }
        }

        public DummyDbData Create()
        {
            ////////////INIT
            FieldWrapperDirector.Distroy();
            InventoryWrapperDirector.Distory();
            CollectionViewModelObserverSubject.Distory();
            FinderDirector.Distroy();
            StockWrapperDirector.Distory();
            LexDb.Distroy();

            ////////////INIT

            using (var db = LexDb.GetDbInstance())
            {
                db.Purge();
            }

            var e = new Employee() { Name = "지창훈" }.Save<Employee>();
            new Employee() { Name = "박재현" }.Save<Employee>();
            new Employee() { Name = "김택윤" }.Save<Employee>();
            new Employee() { Name = "김연청" }.Save<Employee>();
            new Employee() { Name = "천두관" }.Save<Employee>();
            new Employee() { Name = "이도희" }.Save<Employee>();

            var a = new Client() { Name = "야명FA", Delegator = "이양원", PhoneNumber = "053) 604-5656" }.Save<Client>();
            new Client() { Name = "예시스템", Delegator = "지창훈", PhoneNumber = "053) 587-3013" }.Save<Client>();
            new Client() { Name = "이오텍", Delegator = "김택윤", PhoneNumber = "053) 600-8220" }.Save<Client>();
            new Client() { Name = "네오테크", Delegator = "김연청", PhoneNumber = "054) 275-8403" }.Save<Client>();
            new Client() { Name = "덕성전기", Delegator = "천두관", PhoneNumber = "053) 604-4242" }.Save<Client>();
            new Client() { Name = "풍림", Delegator = "이도희", PhoneNumber = "053) 604-4321" }.Save<Client>();

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
                ID = UnregisterdTestItemID,
                Name = "테스트용 아이템정보", MakerID = hanyung.ID, CurrencyID = c.ID, MeasureID = m.ID }.Save<Item>();
            var spec = new Specification() { Remark = "테스트용 규격정보_1", Name = "테스트용 규격정보_1", ItemID = item.ID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            spec = new Specification() { Remark = "테스트용 규격정보_2", Name = "테스트용 규격정보_2", ItemID = item.ID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();

            item = new Item() { Name = "PBL", MakerID = hanyung.ID, CurrencyID = c.ID, MeasureID = m.ID }.Save<Item>();
            spec = new Specification() { Remark = "녹색", Name = "CR254-24V 녹색", ItemID = item.ID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();

            var inven = new Inventory()
            {
                Quantity = 10,
                Remark = "",
                SpecificationID = spec.ID,
                WarehouseID = w.ID,
                ItemID = item.ID
            }.Save<Inventory>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeID = e.ID,
                EnterpriseID = a.ID,
                Quantity = 30,
                ItemID = item.ID,
                SpecificationID = spec.ID,
                StockType = IOStockType.INCOMING,
                InventoryID = inven.ID
            }.Save<InOutStock>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeID = e.ID,
                EnterpriseID = a.ID,
                Quantity = 30,
                ItemID = item.ID,
                SpecificationID = spec.ID,
                StockType = IOStockType.INCOMING,
                InventoryID = inven.ID
            }.Save<InOutStock>();

            spec = new Specification() { Remark = "청색", Name = "CR254-24V 청색", ItemID = item.ID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();

            spec = new Specification() { Remark = "황색", Name = "CR254-24V 황색", ItemID = item.ID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            inven = new Inventory()
            {
                Quantity = 10,
                Remark = "",
                SpecificationID = spec.ID,
                WarehouseID = w.ID,
                ItemID = item.ID
            }.Save<Inventory>();
            item = new Item() { Name = "스위치 박스", MakerID = gunhng.ID, CurrencyID = c.ID, MeasureID = m.ID }.Save<Item>();
            spec = new Specification() { Remark = "1", Name = "KCB-304D", ItemID = item.ID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeID = e.ID,
                EnterpriseID = a.ID,
                Quantity = 30,
                ItemID = item.ID,
                SpecificationID = spec.ID,
                StockType = IOStockType.OUTGOING,
                InventoryID = inven.ID
            }.Save<InOutStock>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeID = e.ID,
                EnterpriseID = a.ID,
                Quantity = 30,
                ItemID = item.ID,
                SpecificationID = spec.ID,
                StockType = IOStockType.INCOMING,
                InventoryID = inven.ID
            }.Save<InOutStock>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeID = e.ID,
                EnterpriseID = a.ID,
                Quantity = 30,
                ItemID = item.ID,
                SpecificationID = spec.ID,
                StockType = IOStockType.INCOMING,
                InventoryID = inven.ID
            }.Save<InOutStock>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeID = e.ID,
                EnterpriseID = a.ID,
                Quantity = 30,
                ItemID = item.ID,
                SpecificationID = spec.ID,
                StockType = IOStockType.INCOMING,
                InventoryID = inven.ID
            }.Save<InOutStock>();

            inven = new Inventory()
            {
                Quantity = 10,
                Remark = "",
                SpecificationID = spec.ID,
                WarehouseID = w.ID,
                ItemID = item.ID
            }.Save<Inventory>();
            spec = new Specification() { Remark = "2", Name = "KCB-303D", ItemID = item.ID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            
            inven = new Inventory()
            {
                Quantity = 10,
                Remark = "",
                SpecificationID = spec.ID,
                WarehouseID = w.ID,
                ItemID = item.ID
            }.Save<Inventory>();
            spec = new Specification() { Remark = "3", Name = "KCB-302D", ItemID = item.ID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            new Inventory()
            {
                Quantity = 10,
                Remark = "",
                SpecificationID = spec.ID,
                WarehouseID = w.ID,
                ItemID = item.ID
            }.Save<Inventory>();
            spec = new Specification() { Remark = "4", Name = "KCB-301D", ItemID = item.ID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            inven = new Inventory()
            {
                Quantity = 10,
                Remark = "",
                SpecificationID = spec.ID,
                WarehouseID = w.ID,
                ItemID = item.ID
            }.Save<Inventory>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeID = e.ID,
                EnterpriseID = a.ID,
                Quantity = 30,
                ItemID = item.ID,
                SpecificationID = spec.ID,
                StockType = IOStockType.OUTGOING,
                InventoryID = inven.ID
            }.Save<InOutStock>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeID = e.ID,
                EnterpriseID = a.ID,
                Quantity = 30,
                ItemID = item.ID,
                SpecificationID = spec.ID,
                StockType = IOStockType.INCOMING,
                InventoryID = inven.ID
            }.Save<InOutStock>();

            spec = new Specification() { Remark = "5", Name = "KCB-254D", ItemID = item.ID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            new Inventory()
            {
                Quantity = 10,
                Remark = "",
                SpecificationID = spec.ID,
                WarehouseID = w.ID,
                ItemID = item.ID
            }.Save<Inventory>();
            spec = new Specification() { Remark = "6", Name = "KCB-253D", ItemID = item.ID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            inven = new Inventory()
            {
                Quantity = 10,
                Remark = "",
                SpecificationID = spec.ID,
                WarehouseID = w.ID,
                ItemID = item.ID
            }.Save<Inventory>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeID = e.ID,
                EnterpriseID = a.ID,
                Quantity = 30,
                ItemID = item.ID,
                SpecificationID = spec.ID,
                StockType = IOStockType.INCOMING,
                InventoryID = inven.ID
            }.Save<InOutStock>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeID = e.ID,
                EnterpriseID = a.ID,
                Quantity = 30,
                ItemID = item.ID,
                SpecificationID = spec.ID,
                StockType = IOStockType.OUTGOING,
                InventoryID = inven.ID
            }.Save<InOutStock>();

            item = new Item() { Name = "단자부", MakerID = shunaider.ID, CurrencyID = c.ID, MeasureID = m.ID }.Save<Item>();
            spec = new Specification() { Remark = "7", Name = "2B 접점", ItemID = item.ID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            inven = new Inventory()
            {
                Quantity = 10,
                Remark = "",
                SpecificationID = spec.ID,
                WarehouseID = w.ID,
                ItemID = item.ID
            }.Save<Inventory>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeID = e.ID,
                EnterpriseID = a.ID,
                Quantity = 30,
                ItemID = item.ID,
                SpecificationID = spec.ID,
                StockType = IOStockType.INCOMING,
                InventoryID = inven.ID
            }.Save<InOutStock>();

            new InOutStock()
            {
                Date = DateTime.Now,
                EmployeeID = e.ID,
                EnterpriseID = a.ID,
                Quantity = 30,
                ItemID = item.ID,
                SpecificationID = spec.ID,
                StockType = IOStockType.OUTGOING,
                InventoryID = inven.ID
            }.Save<InOutStock>();

            //스트레스 테스트
#if false
            for (int i = 0; i < 1000; i++)
            {
                new InOutStock()
                {
                    Date = DateTime.Now,
                    EmployeeID = e.ID,
                    EnterpriseID = a.ID,
                    Quantity = 30,
                    ItemID = item.ID,
                    SpecificationID = spec.ID,
                    StockType = StockType.OUT,
                    WarehouseID = w.ID
                }.Save<InOutStock>();
            }
#endif

            return this;
        }
    }
}
