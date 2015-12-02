using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace R54IN0.Test.new_proj
{
    [TestClass]
    public class FieldDatabaseTest
    {
        [TestMethod]
        public void CanCreateFieldDatabase()
        {
            using (var db = DatabaseDirector.GetFieldDb())
            {

            }
        }

        [TestMethod]
        public void LoadSuccessfully()
        {
            new DummyDbData().Create();

            int cnt;
            int cnt2;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                cnt = db.LoadAll<Item>().Count();
            }

            using (var db = DatabaseDirector.GetFieldDb())
            {
                cnt2 = db.SortedItemList.Count();   
            }

            Assert.AreEqual(cnt, cnt2);
        }
    }
}
