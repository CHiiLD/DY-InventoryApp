using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace R54IN0.Test
{
    [TestClass]
    public class ObserableInventoryTest
    {
        [TestMethod]
        public void CanCreate()
        {
            InventoryFormat fmt = new InventoryFormat();
            ObservableInventory inven = new ObservableInventory();
            inven = new ObservableInventory(fmt);
        }

        /// <summary>
        /// InventoryItem를 생성하여 속성을 채우고 DataDirector에 추가한다. (동시에 Db에 저장하는지 확인)
        /// DataDirector객체를 파괴 후, 다시 생성하여 InventoryItem의 ID로 DB에 저장된 InventoryItem객체를 불러올 수 있는지 검사
        /// </summary>
        [TestMethod]
        public void LoadProperties()
        {
            using (var db = LexDb.GetDbInstance())
                db.Purge();
            ObservableInventory oinven = new ObservableInventory();
            oinven.Measure = new Observable<Measure>() { Name = "EA" };
            oinven.Product = new Observable<Product>() { Name = "product name" };
            oinven.Memo = "memo";
            oinven.Quantity = 123;
            oinven.Specification = "product's specification name(standard)";

            InventoryFormat invenFormat;
            using (var db = LexDb.GetDbInstance())
                invenFormat = db.LoadByKey<InventoryFormat>(oinven.ID);

            ObservableInventory newOinven = new ObservableInventory(invenFormat);
            Assert.AreEqual(oinven.ID, newOinven.ID);
            Assert.AreEqual(oinven.Measure.ID, newOinven.Measure.ID);
            Assert.AreEqual(oinven.Product.ID, newOinven.Product.ID);
            Assert.AreEqual(oinven.Memo, newOinven.Memo);
            Assert.AreEqual(oinven.Quantity, newOinven.Quantity);
            Assert.AreEqual(oinven.Specification, newOinven.Specification);
        }
    }
}
