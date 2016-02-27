using NUnit.Framework;
using MySql.Data.MySqlClient;
using MySQL.Test;
using NUnit.Framework;
using R54IN0.WPF;
using System;
using System.Linq;
using R54IN0.Server;

namespace R54IN0.WPF.Test
{
    [TestFixture]
    public class InventorySearchTextBoxViewModelUnitTest
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
            new InventorySearchTextBoxViewModel();
        }

        /// <summary>
        /// 재고 현황의 재고 검색 테스트
        /// </summary>
        [Test]
        public void Search()
        {
            string product = "     스위치 ";
            string dummyName = "23094832098432";
            string somethingName = "버튼\t 단자부\n버섯\r 213o4u12oi\t";
            string specName = "KCB";

            var vm = new InventorySearchTextBoxViewModel();

            //제품 이름 검색
            vm.Text = product;
            var result = vm.Search();

            Assert.AreNotEqual(0, result.Count());
            Assert.IsTrue(result.All(x => x.Product.Name.Contains("스위치")));

            //더미로 검색하였을 때 검색 실패
            vm.Text = dummyName;
            result = vm.Search();

            Assert.AreEqual(0, result.Count());

            //여러개의 키워드를 넣었을 경우 or연산으로 쿼리
            vm.Text = somethingName;
            result = vm.Search();

            Assert.AreNotEqual(0, result.Count());
            foreach (var name in somethingName.Split(new char[] { ' ', '\t', '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries))
            {
                Assert.IsTrue
                    (
                        result.Any(x => x.Product.Name.Contains("버튼")) ||
                        result.Any(x => x.Product.Name.Contains("단자부")) ||
                        result.Any(x => x.Product.Name.Contains("버섯"))
                    );
            }

            //규격으로 검색
            vm.Text = specName;
            result = vm.Search();

            Assert.AreNotEqual(0, result.Count());
            Assert.IsTrue(result.All(x => x.Product.Name.Contains("SWTICH")));
        }
    }
}