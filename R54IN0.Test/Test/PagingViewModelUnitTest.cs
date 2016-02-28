using System;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using R54IN0.Server;
using MySQL.Test;
using SuperSocket.SocketBase.Config;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows;

namespace R54IN0.WPF.Test
{
    [TestFixture, RequiresSTA]
    public class PagingViewModelUnitTest
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
            new PagingViewModel();
        }

        [Test]
        public void Initialize()
        {
            PagingViewModel pvm = new PagingViewModel();
            foreach (Button btn in pvm.NumericButtons)
                Assert.AreEqual(Visibility.Collapsed, btn.Visibility);
        }

        [Test]
        public void TestNaviationSetting()
        {
            PagingViewModel pvm = new PagingViewModel();
            int count = 100;
            int rowcount = 30;
            pvm.SetNavigation(rowcount, count);

            Assert.AreEqual(1, pvm.CurrentPageNumber);
            Assert.AreEqual(rowcount, pvm.RowCount);
            Assert.AreEqual(count, pvm.Count);
            Assert.AreEqual(4, pvm.PageCount);
            Assert.AreEqual(1, pvm.ChapterCount);
            Assert.AreEqual(0, pvm.CurrentChapterIndex);
            Assert.AreEqual(Visibility.Visible, pvm.NumericButtons[0].Visibility);
            Assert.AreEqual(Visibility.Visible, pvm.NumericButtons[1].Visibility);
            Assert.AreEqual(Visibility.Visible, pvm.NumericButtons[2].Visibility);
            Assert.AreEqual(Visibility.Visible, pvm.NumericButtons[3].Visibility);
            Assert.AreEqual("1", pvm.NumericButtons[0].Content);
            Assert.AreEqual("2", pvm.NumericButtons[1].Content);
            Assert.AreEqual("3", pvm.NumericButtons[2].Content);
            Assert.AreEqual("4", pvm.NumericButtons[3].Content);
        }

        [Test]
        public void TestNavigationSettingWhenDataEmpty()
        {
            PagingViewModel pvm = new PagingViewModel();
            int count = 0;
            int rowcount = 30;
            pvm.SetNavigation(rowcount, count);

            pvm.NumericButtons.ToList().ForEach(x => Assert.AreEqual(Visibility.Collapsed, x.Visibility));
            Assert.AreEqual(0, pvm.PageCount);
            Assert.AreEqual(0, pvm.ChapterCount);
            Assert.AreEqual(0, pvm.CurrentChapterIndex);
        }

        [Test]
        public void TestActNextButton()
        {
            PagingViewModel pvm = new PagingViewModel();
            int count = 400;
            int rowcount = 30;
            pvm.SetNavigation(rowcount, count); // (... 30 * 9) 30, 30 30 30 10
            Assert.AreEqual(14, pvm.PageCount);
            Assert.AreEqual(0, pvm.CurrentChapterIndex);
            Assert.AreEqual(2, pvm.ChapterCount);
            pvm.NextCommand.Execute(null);
            Assert.AreEqual(1, pvm.CurrentChapterIndex);
            Assert.AreEqual(-1, pvm.CurrentPageNumber);
            for (int i = 0; i < 5; i++)
                Assert.AreEqual(Visibility.Visible, pvm.NumericButtons[i].Visibility);
            for (int i = 5; i < 9; i++)
                Assert.AreEqual(Visibility.Collapsed, pvm.NumericButtons[i].Visibility);

            Assert.AreEqual("10", pvm.NumericButtons[0].Content);
            Assert.AreEqual("11", pvm.NumericButtons[1].Content);
            Assert.AreEqual("12", pvm.NumericButtons[2].Content);
            Assert.AreEqual("13", pvm.NumericButtons[3].Content);
            Assert.AreEqual("14", pvm.NumericButtons[4].Content);
            Assert.AreNotEqual("15", pvm.NumericButtons[5].Content);

            pvm.NextCommand.Execute(null);
        }

        [Test]
        public void TestPreviousButton()
        {
            PagingViewModel pvm = new PagingViewModel();
            int count = 1000;
            int rowcount = 30;
            pvm.SetNavigation(rowcount, count);
            pvm.NextCommand.Execute(null);
            pvm.NextCommand.Execute(null);
            Assert.AreEqual(2, pvm.CurrentChapterIndex);

            pvm.PreviousCommand.Execute(null);  // (... 30 * 9) 30, 30, 30, 30,  30  , 30, 30, 30, 30
            Assert.AreEqual(1, pvm.CurrentChapterIndex);
            for (int i = 0; i < PagingViewModel.MAX_PAGE_SIZE; i++)
            {
                Assert.AreEqual(Visibility.Visible, pvm.NumericButtons[i].Visibility);
                Assert.AreEqual((i + 10).ToString(), pvm.NumericButtons[i].Content);
            }
            Assert.AreEqual(-1, pvm.CurrentPageNumber);
        }

        [Test]
        public void TestLastChapterMoveButton()
        {
            PagingViewModel pvm = new PagingViewModel();
            int count = 1000;
            int rowcount = 50; //page = 20 개 9 * 2 + 2 .. 19 20  chapter value = 0 1 2
            pvm.SetNavigation(rowcount, count);
            pvm.LastMoveCommand.Execute(null);

            Assert.AreEqual(pvm.CurrentChapterIndex, pvm.ChapterCount - 1);
            Assert.AreEqual(Visibility.Visible, pvm.NumericButtons[0].Visibility);
            Assert.AreEqual("19", pvm.NumericButtons[0].Content);
            Assert.AreEqual(Visibility.Visible, pvm.NumericButtons[1].Visibility);
            Assert.AreEqual("20", pvm.NumericButtons[1].Content);
            Assert.AreEqual(-1, pvm.CurrentPageNumber);
        }

        [Test]
        public void TestFirstChapterMoveButton()
        {
            PagingViewModel pvm = new PagingViewModel();
            int count = 1000;
            int rowcount = 50; //page = 20 개 9 * 2 + 2 .. 19 20  chapter value = 0 1 2
            pvm.SetNavigation(rowcount, count);
            pvm.LastMoveCommand.Execute(null);
            pvm.FirstMoveCommand.Execute(null);

            Assert.AreEqual(0, pvm.CurrentChapterIndex);
            for (int i = 0; i < PagingViewModel.MAX_PAGE_SIZE; i++)
            {
                Assert.AreEqual(Visibility.Visible, pvm.NumericButtons[i].Visibility);
                Assert.AreEqual((i + 1).ToString(), pvm.NumericButtons[i].Content);
            }
        }

        /// <summary>
        /// 뷰에서 제품별 그룹화를 선택한 후 탐색기에서 제품셀을 선택
        /// </summary>
        [Test]
        public void SelectProduct()
        {
            var sto_vm = new IOStockStatusViewModel();
            PagingViewModel pvm = sto_vm.DataGridPagingViewModel;

            sto_vm.SelectedDataGridGroupOption = IOStockStatusViewModel.DATAGRID_OPTION_PRODUCT;
            var node = sto_vm.TreeViewViewModel.SearchNodesInRoot(NodeType.PRODUCT).Random();
            sto_vm.TreeViewViewModel.AddSelectedNodes(node);

            int itemCount = sto_vm.DataGridViewModel.Items.Count();
            Assert.IsTrue(itemCount != 0);
            Assert.IsTrue(itemCount <= 50);
        }
    }
}