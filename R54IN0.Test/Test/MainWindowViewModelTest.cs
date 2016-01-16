using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R54IN0.Test
{
    [TestClass]
    public class MainWindowViewModelTest
    {
        

        [Ignore]
        [TestMethod]
        public void DeleteNode()
        {
            new Dummy().Create();
            //var viewmodel = MainWindowViewModel.GetInstance();
            //var iview = viewmodel.InventoryViewModel;

            var iview = new InventoryStatusViewModel();
            var sview = new IOStockStatusViewModel();

            sview.SelectedGroupItem = IOStockStatusViewModel.GROUPITEM_DATE;
            sview.DatePickerViewModel.LastYearCommand.Execute(null);

            var productNodes = iview.TreeViewViewModel.Root.SelectMany(r => r.Descendants().Where(n => n.Type == NodeType.PRODUCT));
            var node = productNodes.Random();
            iview.TreeViewViewModel.ExecuteNodesSelectedEventCommand(new SelectionChangedCancelEventArgs(new List<TreeViewNode>() { node }, new List<TreeViewNode>()));
            Assert.IsTrue(iview.TreeViewViewModel.CanDeleteNode());
            Assert.IsTrue(iview.GetDataGridItems().Any(x => x.Product.ID == node.ProductID));
            //삭제
            iview.TreeViewViewModel.SelectedNodeDeletionCommand.Execute(null);
            //트리뷰에서 삭제 확인 
            Assert.IsTrue(iview.TreeViewViewModel.Root.SelectMany(r => r.Descendants().Where(n => node == n)).Count() == 0);
            //InventoryStatus DataGrid 에서 삭제 확인 
            Assert.IsFalse(iview.GetDataGridItems().Any(x => x.Product.ID == node.ProductID));
            //데이터베이스에서의 삭제 확인
            using (var db = LexDb.GetDbInstance())
            {
                var iofmt = db.LoadAll<IOStockFormat>();
                Assert.IsFalse(iofmt.Any(x => new ObservableIOStock(x).Inventory.Product.ID == node.ProductID));
                var ifmt = db.LoadAll<InventoryFormat>();
                Assert.IsFalse(ifmt.Any(x => x.ProductID == node.ProductID));
            }
            //관리자에서의 삭제 확인 
            Assert.IsNull(ObservableFieldDirector.GetInstance().Search<Product>(node.ProductID));
            Assert.IsNull(ObservableInventoryDirector.GetInstance().SearchAsProductID(node.ProductID));
            //입출고 현황 데이터그리드에서 확인 
            Assert.IsFalse(sview.DataGridViewModel.Items.Any(x => x.Inventory.Product.ID == node.ProductID));
        }
    }
}
