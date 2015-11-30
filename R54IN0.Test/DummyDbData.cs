using System;

namespace R54IN0.Test
{
    public class DummyDbData
    {
        public void Create()
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                db.Purge();
            }
            //DatabaseDirector.DistroyDbInstance();

            var e = new Employee() { Name = "지창훈" }.Save<Employee>();
            new Employee() { Name = "박재현" }.Save<Employee>();
            new Employee() { Name = "김택윤" }.Save<Employee>();
            new Employee() { Name = "김연청" }.Save<Employee>();
            new Employee() { Name = "천두관" }.Save<Employee>();
            new Employee() { Name = "이도희" }.Save<Employee>();

            var a = new Account() { Name = "여명FA", Delegator = "이양원", PhoneNumber = "053) 604-5656" }.Save<Account>();
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

            var gunhng = new Maker() { Name = "건흥" }.Save<Maker>();
            var hanyung = new Maker() { Name = "한영넉스" }.Save<Maker>();
            var shunaider = new Maker() { Name = "슈나이더" }.Save<Maker>();

            int pPrice = 1000;
            int sPrice = 1400;

            var item = new Item() { Name = "PBL", MakerUUID = hanyung.UUID, CurrencyUUID = c.UUID, MeasureUUID = m.UUID }.Save<Item>();
            var spec = new Specification() { Remark = "녹색", Name = "CR254-24V 녹색", ItemUUID = item.UUID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
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
            spec = new Specification() { Remark="1", Name = "KCB-304D", ItemUUID = item.UUID, PurchaseUnitPrice = pPrice, SalesUnitPrice = sPrice }.Save<Specification>();
            
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

            //item = new Item() { Name = "DRM04-5", MakerUUID = shunaider.UUID, CurrencyUUID = c.UUID, MeasureUUID = m.UUID }.Save<Item>();
            //new Inventory()
            //{
            //    ItemCount = 10,
            //    Remark = "",
            //    WarehouseUUID = w.UUID,
            //    ItemUUID = item.UUID
            //}.Save<Inventory>();
            //item = new Item() { Name = "작은판넬", MakerUUID = shunaider.UUID, CurrencyUUID = c.UUID, MeasureUUID = m.UUID }.Save<Item>();
            //new Inventory()
            //{
            //    ItemCount = 10,
            //    Remark = "작은판넬",
            //    ItemUUID = item.UUID
            //}.Save<Inventory>();
            //new Item() { Name = "SWITCH", MakerUUID = shunaider.UUID, CurrencyUUID = c.UUID, MeasureUUID = m.UUID }.Save<Item>();
            //new Item() { Name = "콘센트", MakerUUID = shunaider.UUID, CurrencyUUID = c.UUID, MeasureUUID = m.UUID }.Save<Item>();
#if false
            //기본정보 등록 테스트
            new Employee() { Name = "지창훈" }.Save<Employee>();
            new Employee() { Name = "박재현" }.Save<Employee>();
            new Employee() { Name = "김택윤" }.Save<Employee>();
            new Employee() { Name = "김연청" }.Save<Employee>();
            var eep = new Employee() { Name = "천두관" }.Save<Employee>();
            new Employee() { Name = "이도희" }.Save<Employee>();

            new Account() { Name = "(법)부천종합법률사무소", Delegator = "이양원", MobileNumber = "", PhoneNumber = "" }.Save<Account>();
            new Account() { Name = "(사)한국관세무역개발원 인천공항", Delegator = "정세화", MobileNumber = "", PhoneNumber = "" }.Save<Account>();
            new Account() { Name = "(사)한국의료기기산업협회", Delegator = "윤대영", MobileNumber = "", PhoneNumber = "" }.Save<Account>();
            var seller = new Account() { Name = "(주) 비즈메디코리아", Delegator = "김성남", MobileNumber = "", PhoneNumber = "" }.Save<Account>();
            new Account() { Name = "(주) 엠에스비", Delegator = "황선숭", MobileNumber = "010-821-6980", PhoneNumber = "032-821-6980" }.Save<Account>();
            new Account() { Name = "(주) 오토매틱코리아", Delegator = "이식영", MobileNumber = "010-466-7981", PhoneNumber = "02-466-7981" }.Save<Account>();

            var meas = new Measure() { Name = "개" }.Save<Measure>();
            new Measure() { Name = "박스" }.Save<Measure>();
            new Measure() { Name = "10kg" }.Save<Measure>();

            new Warehouse() { Name = "양성공장" }.Save<Warehouse>();
            var ware1 = new Warehouse() { Name = "엠플래닝" }.Save<Warehouse>();
            var ware2 = new Warehouse() { Name = "직영창고" }.Save<Warehouse>();

            var curr = new Currency() { Name = "홍콩달러" }.Save<Currency>();

            //물품 및 규격 등록
            var item = new Item() { Name = "다후다", CurrencyUUID = curr.UUID, MeasureUUID = meas.UUID }.Save<Item>();
            new Specification() { Name = "안감A", ItemUUID = item.UUID, PurchaseUnitPrice = 1000, SalesUnitPrice = 1505 }.Save<Specification>();
            new Specification() { Name = "안감B", ItemUUID = item.UUID, PurchaseUnitPrice = 1100, SalesUnitPrice = 1700 }.Save<Specification>();
            new Specification() { Name = "안감C", ItemUUID = item.UUID, PurchaseUnitPrice = 2100, SalesUnitPrice = 4000 }.Save<Specification>();
            new Specification() { Name = "안감D", ItemUUID = item.UUID, PurchaseUnitPrice = 1020, SalesUnitPrice = 2105 }.Save<Specification>();
            new Specification() { Name = "안감E", ItemUUID = item.UUID, PurchaseUnitPrice = 1140, SalesUnitPrice = 1400 }.Save<Specification>();
            new Specification() { Name = "안감F", ItemUUID = item.UUID, PurchaseUnitPrice = 2200, SalesUnitPrice = 4100 }.Save<Specification>();

            new Item() { Name = "牛肉粉", CurrencyUUID = curr.UUID, MeasureUUID = meas.UUID }.Save<Item>();

            item = new Item() { Name = "원두커피", CurrencyUUID = curr.UUID, MeasureUUID = meas.UUID }.Save<Item>();
            new Specification() { Name = "브라질", ItemUUID = item.UUID, PurchaseUnitPrice = 1000, SalesUnitPrice = 1505 }.Save<Specification>();
            new Specification() { Name = "슈프리모", ItemUUID = item.UUID, PurchaseUnitPrice = 1100, SalesUnitPrice = 1700 }.Save<Specification>();
            new Specification() { Name = "모카 하라", ItemUUID = item.UUID, PurchaseUnitPrice = 2100, SalesUnitPrice = 3000 }.Save<Specification>();

            item = new Item() { Name = "머플러세트", CurrencyUUID = curr.UUID, MeasureUUID = meas.UUID }.Save<Item>();
            new Specification() { Name = "에바주니", ItemUUID = item.UUID, PurchaseUnitPrice = 1000, SalesUnitPrice = 1505 }.Save<Specification>();
            new Specification() { Name = "샤넬", ItemUUID = item.UUID, PurchaseUnitPrice = 1100, SalesUnitPrice = 1700 }.Save<Specification>();
            new Specification() { Name = "쉐어드 무스탕", ItemUUID = item.UUID, PurchaseUnitPrice = 2100, SalesUnitPrice = 3000 }.Save<Specification>();

            item = new Item() { Name = "냅킨", CurrencyUUID = curr.UUID, MeasureUUID = meas.UUID }.Save<Item>();
            new Specification() { Name = "독일 직수입", ItemUUID = item.UUID, PurchaseUnitPrice = 2212, SalesUnitPrice = 3212 }.Save<Specification>();
#endif
        }
    }
}
