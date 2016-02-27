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

namespace R54IN0.Test.Test
{
    [TestFixture]
    public class FieldManagerViewModelUnitTest
    {
        public ReadOnlyServer _readServer;
        public WriteOnlyServer _writeServer;

        [TestFixtureSetUp]
        public void ClassInitialize()
        {
            using (MySqlConnection conn = new MySqlConnection(MySqlJsonFormat.ConnectionString("mysql_connection_string.json")))
            {
                conn.Open();
                Dummy dummy = new Dummy(conn);
                dummy.Create();
            }

            string json = System.IO.File.ReadAllText("ipconfig.json");
            IPConfigJsonFormat config = JsonConvert.DeserializeObject<IPConfigJsonFormat>(json);
            _readServer = new ReadOnlyServer();
            _readServer.Setup(config.ReadServerHost, config.ReadServerPort);
            _writeServer = new WriteOnlyServer();
            _writeServer.Setup(config.WriteServerHost, config.WriteServerPort);
        }

        [SetUp]
        public void Setup()
        {
            _readServer.Start();
            _writeServer.Start();

            DataDirector.InstanceInitialzeAsync();
        }

        [TearDown]
        public void Clean()
        {
            _readServer.Stop();
            _writeServer.Stop();

            CollectionViewModelObserverSubject.Destory();
            TreeViewNodeDirector.Destroy(true);
            DataDirector.Destroy();
        }

        [Test]
        public void CanCreate()
        {
            new FieldManagerViewModel();
        }

        [Test]
        public void TestAdd()
        {
            string name = "soime";
            Maker maker = new Maker(name);
            FieldManagerViewModel vm = new FieldManagerViewModel();
            vm.AddField(maker);

            Thread.Sleep(10);

            Assert.IsTrue(vm.MakerList.Any(x => x.Name == maker.Name));
            Assert.IsTrue(vm.MakerList.Any(x => x.ID == maker.ID));
        }

        [Test]
        public void TestRemove()
        {
            FieldManagerViewModel vm = new FieldManagerViewModel();
            var maker = vm.MakerList.Random();
            vm.RemoveField(maker);

            Thread.Sleep(10);

            vm.MakerList.Any(x => x.Name == maker.Name);
            vm.MakerList.Any(x => x.ID == maker.ID);

            Assert.IsFalse(vm.MakerList.Any(x => x.Name == maker.Name));
            Assert.IsFalse(vm.MakerList.Any(x => x.ID == maker.ID));
        }
    }
}