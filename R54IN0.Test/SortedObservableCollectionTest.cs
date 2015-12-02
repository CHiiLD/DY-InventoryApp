using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

namespace R54IN0.Test
{
    [TestClass]
    public class SortedObservableCollectionTest
    {
        [TestMethod]
        public void CanCreateSortedObservableCollection()
        {
            new SortedObservableCollection<string>();
        }

        [TestMethod]
        public void CanCreateSortedObservableCollection2()
        {
            List<string> list = new List<string>()
            { "a", "c", "b", "d", "f", "e" };
            var sbc = new SortedObservableCollection<string>(list);

            Assert.AreEqual("a", sbc[0]);
            Assert.AreEqual("b", sbc[1]);
            Assert.AreEqual("c", sbc[2]);
            Assert.AreEqual("d", sbc[3]);
            Assert.AreEqual("e", sbc[4]);
            Assert.AreEqual("f", sbc[5]);
        }

        [TestMethod]
        public void CanCreateSortedObservableCollection3()
        {
            List<TesT> list = new List<TesT>()
            {
                new TesT() { K = "a"},
                new TesT() { K = "d"},
                new TesT() { K = "c"},
                new TesT() { K = "b"}
            };
            var sbc = new SortedObservableCollection<TesT>(list);

            Assert.AreEqual("a", sbc[0].K);
            Assert.AreEqual("b", sbc[1].K);
            Assert.AreEqual("c", sbc[2].K);
            Assert.AreEqual("d", sbc[3].K);
        }

        public class TesT : IComparable
        {
            public string K { get; set; }

            public int CompareTo(object obj)
            {
                var test = obj as TesT;
                return string.Compare(K, test.K);
            }
        }

        [TestMethod]
        public void AddRemoveContains()
        {
            var coll = new SortedObservableCollection<string>();
            coll.Add("a");
            coll.Add("b");
            coll.Add("c");
            coll.Add("d");
            coll.Add("e");
            coll.Add("f");
            coll.Remove("f");
            Assert.IsTrue(!coll.Contains("f"));
        }

        [TestMethod]
        public void OverlapRemoveTest()
        {
            var coll = new SortedObservableCollection<string>();
            coll.Add("b");
            coll.Add("e");
            coll.Add("a");
            coll.Add("a");

            Assert.AreEqual(3, coll.Count());

            Assert.AreEqual("a", coll[0]);
            Assert.AreEqual("b", coll[1]);
            Assert.AreEqual("e", coll[2]);
        }

        [Ignore]
        [TestMethod]
        public void BinSearchTest()
        {
            var coll = new SortedObservableCollection<string>();
            coll.Add("a");
            coll.Add("b");
            coll.Add("c");
            coll.Add("d");
            coll.Add("e");
            coll.Add("f");
            coll.Add("t");

            //Assert.AreEqual(0, coll.BinarySearch("a"));
            //Assert.AreEqual(1, coll.BinarySearch("b"));
            //Assert.AreEqual(5, coll.BinarySearch("f"));
            //Assert.AreEqual(3, coll.BinarySearch("d"));
            //Assert.AreEqual(4, coll.BinarySearch("e"));
            //Assert.AreEqual(-1, coll.BinarySearch("z"));
        }
    }
}