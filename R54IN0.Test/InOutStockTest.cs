using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace R54IN0.Test
{
    //[TestClass]
    //public class InOutStockTest
    //{
    //    [TestMethod]
    //    public void CanCreateDummmyDataDb()
    //    {
    //        var dummy = new DummyDbData();
    //    }

    //    [TestMethod]
    //    public void InOutStockTest()
    //    {
    //        string remark_str = "dummy";
    //        var eep = new Employee() { Name = "천두관" }.Save<Employee>();
    //        var seller = new Seller() { Name = "(주) 비즈메디코리아", Delegator = "김성남", MobileNumber = "", PhoneNumber = "" }.Save<Seller>();
    //        var meas = new Measure() { Name = "개" }.Save<Measure>();
    //        var ware = new Warehouse() { Name = "엠플래닝" }.Save<Warehouse>();
    //        var curr = new Currency() { Name = "홍콩달러" }.Save<Currency>();
    //        var item = new Item() { Name = "냅킨", CurrencyUUID = curr.UUID, MeasureUUID = meas.UUID }.Save<Item>();
    //        var spec = new Specification() { Name = "독일 직수입", ItemUUID = item.UUID, PurchaseUnitPrice = 2212, SalesUnitPrice = 3212 }.Save<Specification>();

    //        //재고관리헬퍼로 재고관리 생성
    //        var cur_stock = new InventoryHelper().Save(spec, ware, 10, remark_str); //10개 등록
    //        Assert.AreEqual(cur_stock.SpecificationUUID, spec.UUID);
    //        Assert.AreEqual(cur_stock.TraceItem().UUID, item.UUID);

    //        //재고관리 데이터 체크
    //        var inventoryRR = new InventoryPipe(cur_stock);
    //        Assert.AreEqual(inventoryRR.ItemName, item.Name);
    //        Assert.AreEqual(inventoryRR.Specification, spec.Name);
    //        Assert.AreEqual(inventoryRR.Measure, meas.Name);
    //        Assert.AreEqual(inventoryRR.Currency, curr.Name);
    //        Assert.AreEqual(inventoryRR.PurchaseUnitPrice, spec.PurchaseUnitPrice);
    //        Assert.AreEqual(inventoryRR.SalesUnitPrice, spec.SalesUnitPrice);

    //        //입고 관리 데이터 추가 20개 등록
    //        InOutStock in_stock = new InOutStockHelper().Save(StockType.IN, DateTime.Now, spec, 20, seller, eep, ware, remark_str);
    //        Assert.AreEqual(in_stock.StockType, StockType.IN);
    //        Assert.AreEqual(in_stock.ItemCount, 20);
    //        Assert.AreEqual(in_stock.EmployeeUUID, eep.UUID);
    //        Assert.AreEqual(in_stock.EnterpriseUUID, seller.UUID);
    //        Assert.AreEqual(in_stock.SpecificationUUID, spec.UUID);
    //        Assert.AreEqual(in_stock.Remark, remark_str);

    //        //현재 재고 물량과 일치하는지 확인
    //        cur_stock = DatabaseDirector.GetDbInstance().LoadByKey<Inventory>(cur_stock.UUID);
    //        Assert.AreEqual(cur_stock.SpecificationUUID, spec.UUID);
    //        Assert.AreEqual(cur_stock.TraceItem().UUID, item.UUID);
    //        Assert.AreEqual(cur_stock.ItemCount, 30);

    //        //출고 관리 데이터 추가 -15
    //        InOutStock out_stock = new InOutStockHelper().Save(StockType.OUT, DateTime.Now, spec, 15, seller, eep, ware, remark_str);
    //        Assert.AreEqual(out_stock.StockType, StockType.OUT);

    //        //현재 재고 물량과 일치하는지 확인
    //        cur_stock = DatabaseDirector.GetDbInstance().LoadByKey<Inventory>(cur_stock.UUID);
    //        Assert.AreEqual(cur_stock.SpecificationUUID, spec.UUID);
    //        Assert.AreEqual(cur_stock.TraceItem().UUID, item.UUID);
    //        Assert.AreEqual(cur_stock.ItemCount, 15);
    //    }
    //}
}