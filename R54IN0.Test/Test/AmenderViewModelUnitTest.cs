using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace R54IN0.Test
{
    [TestClass]
    public class AmenderViewModelUnitTest
    {
        /// <summary>
        /// 새로운 입출고 데이터를 등록하였을 경우 입출고 수량이 더해진 만큼 재고수량 값이 변동되어야 한다.
        /// </summary>
        [TestMethod]
        public async Task RecordNewItemThenInvenQuantitySameRemainQty()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.INCOMING;
            viewmodel.Product = ObservableFieldDirector.GetInstance().Copy<Product>().Random();
            var selectedInven = viewmodel.Inventory = viewmodel.InventoryList.Random();
            int inQty = selectedInven.Quantity;
            int icQty = viewmodel.Quantity = 10;

            var record = await viewmodel.RecordAsync();

            Assert.AreEqual(record.Inventory.Quantity, inQty + icQty);
        }

        /// <summary>
        /// 기존의 데이터를 불러와서 각 항목을 채움 물론 입출고 별로 달리
        /// 채우는 것들 ..
        /// 수량, 가격
        /// 입출고처,
        /// 창고 or 프로젝트 이름
        /// 적은 사람
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void ClickLoadButton()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.INCOMING;
            viewmodel.Product = ObservableFieldDirector.GetInstance().Copy<Product>().Random();
            viewmodel.Inventory = viewmodel.InventoryList.Random();

            viewmodel.LoadLastRecordCommand.Execute(null);

            Assert.IsNotNull(viewmodel.Client);
            Assert.IsNotNull(viewmodel.Warehouse);
            Assert.IsNotNull(viewmodel.Employee);
            Assert.IsNull(viewmodel.Project);
        }

        /// <summary>
        /// AmenderView에서 새로운 프로젝트를 등록하였다면 ProjectListView에도 추가가 되어야 한다.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateNewProjectThenAddedProjectListViewModelItems()
        {
            new Dummy().Create();
            IOStockStatusViewModel iosViewModel = new IOStockStatusViewModel();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(iosViewModel);
            viewmodel.StockType = IOStockType.OUTGOING;
            viewmodel.Product = ObservableFieldDirector.GetInstance().Copy<Product>().Random();
            viewmodel.Inventory = viewmodel.InventoryList.Random();
            var name = viewmodel.ProjectText = "DY=NEW=FACE";

            var record = await viewmodel.RecordAsync();

            Assert.IsTrue(iosViewModel.ProjectListBoxViewModel.Items.Contains(record.Project));
        }

        [TestMethod]
        public void ClickLoadButton2()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.OUTGOING;
            viewmodel.Product = ObservableFieldDirector.GetInstance().Copy<Product>().Random();
            viewmodel.Inventory = viewmodel.InventoryList.Random();

            viewmodel.LoadLastRecordCommand.Execute(null);

            Assert.IsNotNull(viewmodel.Client);
            Assert.IsNotNull(viewmodel.Project);
            Assert.IsNotNull(viewmodel.Employee);
            Assert.IsNull(viewmodel.Warehouse);
        }

        /// <summary>
        /// 입고 기록을 추가할 때 추가정보를 방치하면 Null로 저장됨
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RecordNewIOStockThenCheckNullProperties()
        {
            new Dummy().Create();
            IOStockStatusViewModel status = new IOStockStatusViewModel();
            var amender = RecordNewIOStock(status);
            var node = status.TreeViewViewModel.SelectedNodes.Single();
            amender.StockType = IOStockType.OUTGOING;
            amender.Product = ObservableFieldDirector.GetInstance().Search<Product>(node.ProductID);
            amender.Inventory = amender.InventoryList.Random();
            var record = await amender.RecordAsync();
            var item = status.DataGridViewModel.Items.Where(x => x.ID == record.ID).Single();

            Assert.IsNull(item.Project);
            Assert.IsNull(item.Warehouse);
            Assert.IsNull(item.Customer);
            Assert.IsNull(item.Supplier);
            Assert.IsNull(item.Employee);
        }

        /// <summary>
        /// 출고 기록을 추가할 때 추가정보를 방치하면 Null로 저장됨
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RecordNewIOStockThenCheckNullProperties2()
        {
            new Dummy().Create();
            IOStockStatusViewModel status = new IOStockStatusViewModel();
            var amender = RecordNewIOStock(status);
            var node = status.TreeViewViewModel.SelectedNodes.Single();
            amender.StockType = IOStockType.INCOMING;
            amender.Product = ObservableFieldDirector.GetInstance().Search<Product>(node.ProductID);
            amender.Inventory = amender.InventoryList.Random();
            var record = await amender.RecordAsync();
            var item = status.DataGridViewModel.Items.Where(x => x.ID == record.ID).Single();

            Assert.IsNull(item.Project);
            Assert.IsNull(item.Warehouse);
            Assert.IsNull(item.Customer);
            Assert.IsNull(item.Supplier);
            Assert.IsNull(item.Employee);
        }

        public IOStockDataAmenderViewModel RecordNewIOStock(IOStockStatusViewModel status)
        {
            status.SelectedGroupItem = IOStockStatusViewModel.GROUPITEM_PRODUCT;
            var node = status.TreeViewViewModel.Root.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).Random();
            status.TreeViewViewModel.SelectedNodes.Add(node);
            status.OnTreeViewNodesSelected(status.TreeViewViewModel, new PropertyChangedEventArgs("SelectedNodes"));
            IOStockDataAmenderViewModel amender = new IOStockDataAmenderViewModel(status);
            return amender;
        }

        /// <summary>
        /// 사람, 창고, 입고처를 추가하였다면 Director에도 데이터가 동기화 되어야 한다.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RecordNewIOStockThenSyncDirector()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.INCOMING;
            viewmodel.Product = ObservableFieldDirector.GetInstance().Copy<Product>().Random();
            viewmodel.Inventory = viewmodel.InventoryList.Random();
            string client = viewmodel.ClientText = "some client";
            string warehouse = viewmodel.WarehouseText = "some warehouse";
            string employee = viewmodel.EmployeeText = "some name";

            var record = await viewmodel.RecordAsync();

            Assert.AreEqual(employee, record.Employee.Name);
            Assert.IsNotNull(ObservableFieldDirector.GetInstance().Search<Employee>(record.Employee.ID));

            Assert.AreEqual(warehouse, record.Warehouse.Name);
            Assert.IsNotNull(ObservableFieldDirector.GetInstance().Search<Warehouse>(record.Warehouse.ID));

            Assert.AreEqual(client, record.Supplier.Name);
            Assert.IsNotNull(ObservableFieldDirector.GetInstance().Search<Supplier>(record.Supplier.ID));
        }

        [TestMethod]
        public async Task RecordNewInventoryThenAddedInventoryDirector()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.OUTGOING;
            viewmodel.Product = ObservableFieldDirector.GetInstance().Copy<Product>().Random();
            viewmodel.Inventory = null;
            var text = viewmodel.SpecificationText = "new inventory";

            var record = await viewmodel.RecordAsync();

            Assert.AreEqual(text, record.Inventory.Specification);
            Assert.IsNotNull(record.Inventory.ID);
            Assert.IsNotNull(ObservableInventoryDirector.GetInstance().Search(record.Inventory.ID));
        }

        [TestMethod]
        public async Task AddedNewProductAndInventory()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.OUTGOING;
            string productText = viewmodel.ProductText = "some product";
            string specText = viewmodel.SpecificationText = "some spec";

            var record = await viewmodel.RecordAsync();

            Assert.IsNotNull(record.Inventory);
            Assert.IsNotNull(record.Inventory.Product);
            Assert.AreEqual(productText, record.Inventory.Product.Name);
            Assert.AreEqual(specText, record.Inventory.Specification);
            Assert.IsNotNull(ObservableFieldDirector.GetInstance().Search<Product>(record.Inventory.Product.ID));
            Assert.IsNotNull(ObservableInventoryDirector.GetInstance().Search(record.Inventory.ID));
        }
    }
}