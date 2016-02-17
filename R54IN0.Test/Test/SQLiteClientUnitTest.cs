using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System;
using System.Linq;

namespace R54IN0.Test
{
    [TestClass]
    public class SQLiteServerUnitTest
    {
        [TestMethod]
        public void TestField()
        {
            SQLiteClient server = new SQLiteClient();
            server.Open();
            server.DropAllTableThenReCreateTable();
            //insert
            Customer customer = new Customer();
            string id = customer.ID = Guid.NewGuid().ToString();
            string name = customer.Name = "some customer";
            server.Insert(customer);
            //select
            Customer result = server.Select<Customer>(id);
            Assert.AreEqual(id, result.ID);
            Assert.AreEqual(name, result.Name);
            //udpate
            string newname = "new customer";
            result.Name = newname;
            server.Update(result, nameof(customer.Name));

            result = server.Select<Customer>(id);
            Assert.AreEqual(id, result.ID);
            Assert.AreEqual(newname, result.Name);
            //delete
            server.Delete(result);
            result = server.Select<Customer>(id);
            Assert.IsNull(result);

            server.Close();
        }

        [TestMethod]
        public void TestInventoryFormat()
        {
            SQLiteClient server = new SQLiteClient();
            server.Open();
            server.DropAllTableThenReCreateTable();

            InventoryFormat fmt = new InventoryFormat();
            //insert
            string id = fmt.ID = Guid.NewGuid().ToString();
            int qty = fmt.Quantity = 101221210;
            server.Insert(fmt);

            //select
            InventoryFormat result = server.Select<InventoryFormat>(id);
            Assert.AreEqual(id, result.ID);
            Assert.AreEqual(qty, result.Quantity);

            server.Close();
        }

        [TestMethod]
        public void TestIOStockFormat()
        {
            SQLiteClient server = new SQLiteClient();
            server.Open();
            server.DropAllTableThenReCreateTable();

            IOStockFormat fmt = new IOStockFormat();
            //insert
            string id = fmt.ID = Guid.NewGuid().ToString();
            decimal price = fmt.UnitPrice = 100000000000000L;
            DateTime date = fmt.Date = DateTime.Now;
            IOStockType type = fmt.StockType = IOStockType.INCOMING;
            server.Insert(fmt);

            //select
            IOStockFormat result = server.Select<IOStockFormat>(id);
            Assert.AreEqual(id, result.ID);
            Assert.AreEqual(price, result.UnitPrice);
            Assert.IsTrue(date.ToString(SQLiteClient.DATETIME) == result.Date.ToString(SQLiteClient.DATETIME));
            Assert.AreEqual(type, result.StockType);

            server.Close();
        }

        /// <summary>
        /// desc 검사
        /// </summary>
        [TestMethod]
        public void TestOrderBy()
        {
            new Dummy().Create();
            var ddr = DataDirector.GetInstance();
            var db = ddr.DB;

            var inv = ddr.CopyInventories().Random();

            var stos = db.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}';", inv.ID);
            var lsto = db.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' order by Date desc limit 1;", inv.ID).Single();

            var l = stos.OrderBy(x => x.Date).Last();

            Assert.AreEqual(l.ID, lsto.ID);
        }

        /// <summary>
        /// 마지막 입출고 데이터의 리메인수량과 인벤토리 수량 일치 검사
        /// </summary>
        [TestMethod]
        public void TestSyncQuantity()
        {
            new Dummy().Create();
            var ddr = DataDirector.GetInstance();
            var db = ddr.DB;

            var inv = ddr.CopyInventories().Random();
            var cur_tqty = inv.Quantity;

            var lsto = db.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' order by Date desc limit 1;", inv.ID).Single();

            Assert.AreEqual(cur_tqty, lsto.RemainingQuantity);
        }

        /// <summary>
        /// 새로운 입출고 데이터를 넣고 수량이 증가되었는지 확인
        /// </summary>
        [TestMethod]
        public void InsertNewIOStockFormatThenSyncInventoryQtyAndRemainQty()
        {
            new Dummy().Create();
            InsertNewIOStockFormat(new DateTime(2010, 1, 1));
            InsertNewIOStockFormat(new DateTime(2011, 1, 1));
            InsertNewIOStockFormat(new DateTime(2012, 1, 1));
            InsertNewIOStockFormat(new DateTime(2013, 1, 1));
            InsertNewIOStockFormat(new DateTime(2014, 1, 1));
            InsertNewIOStockFormat(new DateTime(2015, 1, 1));
            InsertNewIOStockFormat(new DateTime(2016, 1, 1));
            InsertNewIOStockFormat(new DateTime(2017, 1, 1));
        }

        /// <summary>
        /// 새로운 입출고 데이터를 넣고 전체수량 증가 확인 
        /// </summary>
        /// <param name="date"></param>
        void InsertNewIOStockFormat(DateTime date)
        {
            var ddr = DataDirector.GetInstance();
            var db = ddr.DB;

            var inv = ddr.CopyInventories().Random();
            var cur_tqty = inv.Quantity;

            var lsto = db.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' order by Date desc limit 1;", inv.ID).Single();
            var copy = new IOStockFormat(lsto);
            copy.StockType = IOStockType.INCOMING;
            copy.ID = Guid.NewGuid().ToString();
            copy.Quantity = 1;
            copy.Date = date;
            copy.RemainingQuantity = 0;

            db.Insert<IOStockFormat>(copy);

            var invfmt = db.Select<InventoryFormat>(inv.ID);
            Assert.AreEqual(cur_tqty + copy.Quantity, invfmt.Quantity);
        }

        /// <summary>
        /// 기존의 입출고 데이터 수량 변경하고 적용 확인 
        /// </summary>
        [TestMethod]
        public void UpdateIOStockFormatThenSyncInventoryQuantity()
        {
            new Dummy().Create();
            var ddr = DataDirector.GetInstance();
            var db = ddr.DB;

            var inv = ddr.CopyInventories().Random();
            var cur_tqty = inv.Quantity;

            Console.WriteLine(nameof(UpdateIOStockFormatThenSyncInventoryQuantity));

            var stos = db.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' and StockType = 1;", inv.ID);
            foreach (var s in stos)
            {
                db.Update<IOStockFormat>(s, nameof(s.Quantity), s.Quantity + 1);
                var c_inv = db.Select<InventoryFormat>(s.InventoryID);
                Assert.AreEqual(cur_tqty + 1, c_inv.Quantity);
                cur_tqty += 1;
            }
        }

        /// <summary>
        /// 입출고 데이터 수량 변경 후 전체 수량 동기화 확인 (출고) 
        /// </summary>
        [TestMethod]
        public void UpdateIOStockFormatThenSyncInventoryQuantity2()
        {
            new Dummy().Create();
            var ddr = DataDirector.GetInstance();
            var db = ddr.DB;

            var inv = ddr.CopyInventories().Random();
            var cur_tqty = inv.Quantity;

            Console.WriteLine(nameof(UpdateIOStockFormatThenSyncInventoryQuantity));

            var stos = db.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' and StockType = 2;", inv.ID);
            foreach (var s in stos)
            {
                db.Update<IOStockFormat>(s, nameof(s.Quantity), s.Quantity + 1);
                var c_inv = db.Select<InventoryFormat>(s.InventoryID);
                Assert.AreEqual(cur_tqty - 1, c_inv.Quantity);
                cur_tqty -= 1;
            }
        }

        /// <summary>
        /// 인벤토리 삭제 후 관련 데이터 소멸을 확인 
        /// </summary>
        [TestMethod]
        public void DeleteInventoryThenSyncIOStockFormat()
        {
            new Dummy().Create();
            var ddr = DataDirector.GetInstance();
            var db = ddr.DB;

            var inv = ddr.CopyInventories().Random();

            db.Delete<InventoryFormat>(inv.Format);
            var stos = db.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}';", inv.ID);

            Assert.AreEqual(0, stos.Count());
        }

        /// <summary>
        /// 입출고 포맷 삭제 후 수량 감소 확인 
        /// </summary>
        [TestMethod]
        public void DeleteIOStockFormatThenSyncQuantity()
        {
            new Dummy().Create();
            var ddr = DataDirector.GetInstance();
            var db = ddr.DB;

            var inv = ddr.CopyInventories().Random();
            var stos = db.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' and StockType = 1;", inv.ID);
            var st = stos.Random();

            var inv2 = db.Select<InventoryFormat>(st.InventoryID);
            db.Delete<IOStockFormat>(st);
            var inv3 = db.Select<InventoryFormat>(st.InventoryID);

            Assert.AreEqual(inv2.Quantity - st.Quantity, inv3.Quantity);
            Assert.AreEqual(inv.Quantity, inv3.Quantity);
        }

        /// <summary>
        /// 입출고 포맷 삭제 후 수량 증가 확인
        /// </summary>
        [TestMethod]
        public void DeleteIOStockFormatThenSyncQuantity2()
        {
            new Dummy().Create();
            var ddr = DataDirector.GetInstance();
            var db = ddr.DB;

            var inv = ddr.CopyInventories().Random();
            var stos = db.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' and StockType = 2;", inv.ID);
            var st = stos.Random();

            var inv2 = db.Select<InventoryFormat>(st.InventoryID);
            db.Delete<IOStockFormat>(st);
            var inv3 = db.Select<InventoryFormat>(st.InventoryID);

            Assert.AreEqual(inv2.Quantity + st.Quantity, inv3.Quantity);
            Assert.AreEqual(inv.Quantity, inv3.Quantity);
        }

        public void CheckReteinQty(InventoryFormat inven)
        {
            var iosfmts = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' order by Date;"
                , inven.ID);
            Assert.AreNotEqual(0, iosfmts.Count());

            Console.WriteLine(nameof(CheckReteinQty));
            IOStockFormat near = null;
            foreach (var fmt in iosfmts)
            {
                int remainQty = 0;
                int iosQty = fmt.Quantity;
                if (fmt.StockType == IOStockType.OUTGOING)
                    iosQty = -iosQty;
                if (near != null)
                    remainQty = near.RemainingQuantity;
                int exp = remainQty + iosQty;
                Console.WriteLine("{0} {1} = {2}", remainQty, iosQty, fmt.RemainingQuantity);
                Assert.AreEqual(fmt.RemainingQuantity, exp);
                near = fmt;
            }
            var lastObj = iosfmts.Last();
            var oIOStock = new ObservableIOStock(lastObj);
            Assert.AreEqual(oIOStock.RemainingQuantity, oIOStock.Inventory.Quantity);
        }

        [TestMethod]
        public void RemainQtyOfLastIOStockIsSameInventoryQty()
        {
            new Dummy().Create();
            var ddr = DataDirector.GetInstance();
            var db = ddr.DB;

            var inv = ddr.CopyInventories().Random();
            var cur_tqty = inv.Quantity;

            var lsto = db.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' order by Date desc limit 1;", inv.ID).Single();
            var copy = new IOStockFormat(lsto);
            copy.StockType = IOStockType.INCOMING;
            copy.ID = Guid.NewGuid().ToString();
            copy.Quantity = 1;
            copy.RemainingQuantity = 0;
            copy.Date = new DateTime(2015, 1, 1);

            db.Insert<IOStockFormat>(copy);

            lsto = db.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' order by Date desc limit 1;", inv.ID).Single();
            Assert.AreEqual(cur_tqty + copy.Quantity, lsto.RemainingQuantity);
        }

        [TestMethod]
        public void InsertThenSyncRemainQty()
        {
            new Dummy().Create();
            var ddr = DataDirector.GetInstance();
            var db = ddr.DB;

            var inv = ddr.CopyInventories().Random();
            var lsto = db.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' order by Date desc limit 1;", inv.ID).Single();
            var copy = new IOStockFormat(lsto);
            copy.StockType = IOStockType.INCOMING;
            copy.ID = Guid.NewGuid().ToString();
            copy.Quantity = 10;
            copy.Date = new DateTime(2015, 1, 1);

            db.Insert<IOStockFormat>(copy);

            CheckReteinQty(inv.Format);
        }

        [TestMethod]
        public void InsertThenSyncRemainQty2()
        {
            new Dummy().Create();
            var ddr = DataDirector.GetInstance();
            var db = ddr.DB;

            var inv = ddr.CopyInventories().Random();
            var iosfmts = db.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' order by Date;", inv.ID);
            var copy = new IOStockFormat(iosfmts.Random());
            copy.StockType = IOStockType.INCOMING;
            copy.ID = Guid.NewGuid().ToString();
            copy.Quantity = 10;
            copy.Date = iosfmts.ElementAt(iosfmts.Count() / 2).Date.AddDays(1);

            db.Insert<IOStockFormat>(copy);

            CheckReteinQty(inv.Format);
        }

        [TestMethod]
        public void DeleteThenSyncRemainQty()
        {
            new Dummy().Create();
            var ddr = DataDirector.GetInstance();
            var db = ddr.DB;

            var inv = ddr.CopyInventories().Random();
            var iosfmts = db.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' order by Date;", inv.ID);

            db.Delete<IOStockFormat>(iosfmts.ElementAt(iosfmts.Count() / 2));

            CheckReteinQty(inv.Format);
        }

        [TestMethod]
        public void UpdateThenSyncRemainQty()
        {
            new Dummy().Create();
            var ddr = DataDirector.GetInstance();
            var db = ddr.DB;

            var inv = ddr.CopyInventories().Random();
            var iosfmts = db.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' order by Date;", inv.ID);
            var f =  iosfmts.ElementAt(iosfmts.Count() / 2);
            f.Quantity = f.Quantity + 10;

            db.Update<IOStockFormat>(f, nameof(f.Quantity));

            CheckReteinQty(inv.Format);
        }
    }
}