using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace R54IN0.Test.New
{
    [TestClass]
    public class DummyTest
    {
        [TestMethod]
        public void CanCreate()
        {
            new Dummy().Create();
        }

        [TestMethod]
        public void TestOverlap()
        {
            var db = LexDb.GetDbInstance();
            string customer = "some customer";
            db.Save<Customer>(new Customer() { Name = customer, ID = Guid.NewGuid().ToString() });
            db.Save<Customer>(new Customer() { Name = customer, ID = Guid.NewGuid().ToString() });
            db.Save<Customer>(new Customer() { Name = customer, ID = Guid.NewGuid().ToString() });

            //overlap.Program.Do1<Customer>();

            var customers = db.LoadAll<Customer>();
            int cnt = 0;
            foreach (var item in customers)
                if (item.Name == customer)
                    cnt++;

            Assert.AreEqual(1, cnt);
        }
    }
}