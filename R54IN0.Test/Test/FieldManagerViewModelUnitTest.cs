using System;
using MySql.Data.MySqlClient;
using MySQL.Test;
using R54IN0.WPF;
using System.Linq;
using NUnit.Framework;
using R54IN0.Server;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using SuperSocket.SocketBase.Config;

namespace R54IN0.WPF.Test
{
    [TestFixture]
    public class FieldManagerViewModelUnitTest
    {
        MySqlConnection _conn;

        [TestFixtureSetUp]
        public void TestSetUp()
        {
            _conn = new MySqlConnection(MySqlJsonFormat.ConnectionString("mysql_connection_string.json"));
            _conn.Open();
            Dummy dummy = new Dummy(_conn);
            dummy.Create();
        }

        [TestFixtureTearDown]
        public void TestTearDown()
        {
            _conn.Close();
        }

        [SetUp]
        public void Setup()
        {
            IDbAction dbAction = new FakeDbAction(_conn);
            DataDirector.IntializeInstance(dbAction);
        }

        [TearDown]
        public void TearDown()
        {
            CollectionViewModelObserverSubject.Destory();
            TreeViewNodeDirector.Destroy(true);
            DataDirector.Destroy();
        }

        [Test]
        public void CanCreate()
        {
            DataDirector ddr = DataDirector.GetInstance();
            new FieldManagerViewModel();
        }

        [Test]
        public void CanAdd()
        {
            string name = "soime";
            Maker maker = new Maker(name);
            FieldManagerViewModel vm = new FieldManagerViewModel();
            vm.AddField(maker);
            Assert.IsTrue(vm.MakerList.Any(x => x.Name == maker.Name));
            Assert.IsTrue(vm.MakerList.Any(x => x.ID == maker.ID));
        }

        [Test]
        public void CanRemove()
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