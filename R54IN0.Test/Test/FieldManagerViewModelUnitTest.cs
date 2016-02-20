using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using MySQL.Test;
using R54IN0.WPF;
using System.Linq;

namespace R54IN0.Test.Test
{
    [TestClass]
    public class FieldManagerViewModelUnitTest
    {
        private static MySqlConnection _conn;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Console.WriteLine(nameof(ClassInitialize));
            Console.WriteLine(context.TestName);

            _conn = new MySqlConnection(ConnectingString.KEY);
            _conn.Open();

            Dummy dummy = new Dummy(_conn);
            dummy.Create();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Console.WriteLine(nameof(ClassCleanup));
            _conn.Close();
            _conn = null;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            MySqlConnection conn = DataDirector.GetInstance().DB.Connection;
            using (MySqlCommand cmd = new MySqlCommand("begin work;", conn))
                cmd.ExecuteNonQuery();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            MySqlConnection conn = DataDirector.GetInstance().DB.Connection;
            using (MySqlCommand cmd = new MySqlCommand("rollback;", conn))
                cmd.ExecuteNonQuery();

            CollectionViewModelObserverSubject.Destory();
            TreeViewNodeDirector.Destroy(true);
            DataDirector.Destroy();
        }

        [TestMethod]
        public void CanCreate()
        {
            new FieldManagerViewModel();
        }

        [TestMethod]
        public void TestAdd()
        {
            string name = "soime";
            Observable<Maker> maker = new Observable<Maker>(name);
            FieldManagerViewModel vm = new FieldManagerViewModel();
            vm.AddField(maker);

            Assert.IsTrue(vm.MakerList.Any(x => x.Name == maker.Name));
            Assert.IsTrue(vm.MakerList.Any(x => x.ID == maker.ID));
        }

        [TestMethod]
        public void TestRemove()
        {
            FieldManagerViewModel vm = new FieldManagerViewModel();
            var maker = vm.MakerList.Random();
            vm.RemoveField(maker);

            vm.MakerList.Any(x => x.Name == maker.Name);
            vm.MakerList.Any(x => x.ID == maker.ID);

            Assert.IsFalse(vm.MakerList.Any(x => x.Name == maker.Name));
            Assert.IsFalse(vm.MakerList.Any(x => x.ID == maker.ID));
        }
    }
}