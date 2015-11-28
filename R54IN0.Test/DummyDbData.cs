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

            //기본정보 등록 테스트
            new Employee() { Name = "지창훈" }.Save<Employee>();
            new Employee() { Name = "박재현" }.Save<Employee>();
            new Employee() { Name = "김택윤" }.Save<Employee>();
            new Employee() { Name = "김연청" }.Save<Employee>();
            var eep = new Employee() { Name = "천두관" }.Save<Employee>();
            new Employee() { Name = "이도희" }.Save<Employee>();

            new Seller() { Name = "(법)부천종합법률사무소", Delegator = "이양원", MobileNumber = "", PhoneNumber = "" }.Save<Seller>();
            new Seller() { Name = "(사)한국관세무역개발원 인천공항", Delegator = "정세화", MobileNumber = "", PhoneNumber = "" }.Save<Seller>();
            new Seller() { Name = "(사)한국의료기기산업협회", Delegator = "윤대영", MobileNumber = "", PhoneNumber = "" }.Save<Seller>();
            var seller = new Seller() { Name = "(주) 비즈메디코리아", Delegator = "김성남", MobileNumber = "", PhoneNumber = "" }.Save<Seller>();
            new Seller() { Name = "(주) 엠에스비", Delegator = "황선숭", MobileNumber = "010-821-6980", PhoneNumber = "032-821-6980" }.Save<Seller>();
            new Seller() { Name = "(주) 오토매틱코리아", Delegator = "이식영", MobileNumber = "010-466-7981", PhoneNumber = "02-466-7981" }.Save<Seller>();

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
        }
    }
}
