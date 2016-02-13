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
        public void RecordNewItemThenInvenQuantitySameRemainQty()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.INCOMING;
            viewmodel.Product = InventoryDataCommander.GetInstance().CopyFields<Product>().Random();
            var selectedInven = viewmodel.Inventory = viewmodel.InventoryList.Random();
            int inQty = selectedInven.Quantity;
            int icQty = viewmodel.Quantity = 10;

            var record =  viewmodel.Record();

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
            viewmodel.Product = InventoryDataCommander.GetInstance().CopyFields<Product>().Random();
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
        public void CreateNewProjectThenAddedProjectListViewModelItems()
        {
            new Dummy().Create();
            IOStockStatusViewModel iosViewModel = new IOStockStatusViewModel();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(iosViewModel);
            viewmodel.StockType = IOStockType.OUTGOING;
            viewmodel.Product = InventoryDataCommander.GetInstance().CopyFields<Product>().Random();
            viewmodel.Inventory = viewmodel.InventoryList.Random();
            var name = viewmodel.ProjectText = "DY=NEW=FACE";

            var record = viewmodel.Record();

            Assert.IsTrue(iosViewModel.ProjectListBoxViewModel.Items.Contains(record.Project));
        }

        [TestMethod]
        public void ClickLoadButton2()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.OUTGOING;
            viewmodel.Product = InventoryDataCommander.GetInstance().CopyFields<Product>().Random();
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
        public void RecordNewIOStockThenCheckNullProperties()
        {
            new Dummy().Create();
            IOStockStatusViewModel status = new IOStockStatusViewModel();
            var amender = RecordNewIOStock(status);
            var node = status.TreeViewViewModel.SelectedNodes.Single();
            amender.StockType = IOStockType.OUTGOING;
            amender.Product = InventoryDataCommander.GetInstance().SearchField<Product>(node.ObservableObjectID);
            amender.Inventory = amender.InventoryList.Random();
            var record = amender.Record();
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
        public void RecordNewIOStockThenCheckNullProperties2()
        {
            new Dummy().Create();
            IOStockStatusViewModel status = new IOStockStatusViewModel();
            var amender = RecordNewIOStock(status);
            var node = status.TreeViewViewModel.SelectedNodes.Single();
            amender.StockType = IOStockType.INCOMING;
            amender.Product = InventoryDataCommander.GetInstance().SearchField<Product>(node.ObservableObjectID);
            amender.Inventory = amender.InventoryList.Random();
            var record = amender.Record();
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
        public void RecordNewIOStockThenSyncDirector()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.INCOMING;
            viewmodel.Product = InventoryDataCommander.GetInstance().CopyFields<Product>().Random();
            viewmodel.Inventory = viewmodel.InventoryList.Random();
            string client = viewmodel.ClientText = "some client";
            string warehouse = viewmodel.WarehouseText = "some warehouse";
            string employee = viewmodel.EmployeeText = "some name";

            var record = viewmodel.Record();

            Assert.AreEqual(employee, record.Employee.Name);
            Assert.IsNotNull(InventoryDataCommander.GetInstance().SearchField<Employee>(record.Employee.ID));

            Assert.AreEqual(warehouse, record.Warehouse.Name);
            Assert.IsNotNull(InventoryDataCommander.GetInstance().SearchField<Warehouse>(record.Warehouse.ID));

            Assert.AreEqual(client, record.Supplier.Name);
            Assert.IsNotNull(InventoryDataCommander.GetInstance().SearchField<Supplier>(record.Supplier.ID));
        }

        [TestMethod]
        public void RecordNewInventoryThenAddedInventoryDirector()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.OUTGOING;
            viewmodel.Product = InventoryDataCommander.GetInstance().CopyFields<Product>().Random();
            viewmodel.Inventory = null;
            var text = viewmodel.SpecificationText = "new inventory";

            var record = viewmodel.Record();

            Assert.AreEqual(text, record.Inventory.Specification);
            Assert.IsNotNull(record.Inventory.ID);
            Assert.IsNotNull(InventoryDataCommander.GetInstance().SearchInventory(record.Inventory.ID));
        }

        [TestMethod]
        public void AddedNewProductAndInventory()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.OUTGOING;
            string productText = viewmodel.ProductText = "some product";
            string specText = viewmodel.SpecificationText = "some spec";

            var record = viewmodel.Record();

            Assert.IsNotNull(record.Inventory);
            Assert.IsNotNull(record.Inventory.Product);
            Assert.AreEqual(productText, record.Inventory.Product.Name);
            Assert.AreEqual(specText, record.Inventory.Specification);
            Assert.IsNotNull(InventoryDataCommander.GetInstance().SearchField<Product>(record.Inventory.Product.ID));
            Assert.IsNotNull(InventoryDataCommander.GetInstance().SearchInventory(record.Inventory.ID));
        }

        /// <summary>
        /// 필드 삭제 체크
        /// ComboBox Item 삭제 확인 
        /// </summary>
        [TestMethod]
        public void DeleteMakerField()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            var maker = viewmodel.MakerList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(maker);
            var measure = viewmodel.MeasureList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(measure);
            var client = viewmodel.ClientList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(client);
            var employee = viewmodel.EmployeeList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(employee);
            var warehouse = viewmodel.WarehouseList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(warehouse);
            var project = viewmodel.ProjectList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(project);

            Assert.IsFalse(viewmodel.MakerList.Contains(maker));
            Assert.IsFalse(viewmodel.MeasureList.Contains(measure));
            Assert.IsFalse(viewmodel.ClientList.Contains(client));
            Assert.IsFalse(viewmodel.EmployeeList.Contains(employee));
            Assert.IsFalse(viewmodel.WarehouseList.Contains(warehouse));
            Assert.IsFalse(viewmodel.ProjectList.Contains(project));
        }

        /// <summary>
        /// 필드 삭제 체크
        /// Director 삭제 확인
        /// </summary>
        [TestMethod]
        public void DeleteFieldThenSyncFieldDirector()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            var maker = viewmodel.MakerList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(maker);
            var measure = viewmodel.MeasureList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(measure);
            var client = viewmodel.ClientList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(client);
            var employee = viewmodel.EmployeeList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(employee);
            var warehouse = viewmodel.WarehouseList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(warehouse);
            var project = viewmodel.ProjectList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(project);

            Assert.IsNull(InventoryDataCommander.GetInstance().SearchField<Maker>(maker.ID));
            Assert.IsNull(InventoryDataCommander.GetInstance().SearchField<Measure>(measure.ID));
            Assert.IsNull(InventoryDataCommander.GetInstance().SearchField<Supplier>(client.ID));
            Assert.IsNull(InventoryDataCommander.GetInstance().SearchField<Employee>(employee.ID));
            Assert.IsNull(InventoryDataCommander.GetInstance().SearchField<Warehouse>(warehouse.ID));
            Assert.IsNull(InventoryDataCommander.GetInstance().SearchField<Project>(project.ID));
        }

        /// <summary>
        /// 필드 삭제 체크
        /// Db 삭제 확인
        /// </summary>
        [TestMethod]
        public void DeleteFieldThenSyncDb()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            var maker = viewmodel.MakerList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(maker);
            var measure = viewmodel.MeasureList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(measure);
            var client = viewmodel.ClientList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(client);
            var employee = viewmodel.EmployeeList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(employee);
            var warehouse = viewmodel.WarehouseList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(warehouse);
            var project = viewmodel.ProjectList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(project);

            Assert.IsNull(InventoryDataCommander.GetInstance().DB.Select<Maker>(nameof(maker.ID), maker.ID));
            Assert.IsNull(InventoryDataCommander.GetInstance().DB.Select<Measure>(nameof(maker.ID), measure.ID));
            Assert.IsNull(InventoryDataCommander.GetInstance().DB.Select<Supplier>(nameof(maker.ID), client.ID));
            Assert.IsNull(InventoryDataCommander.GetInstance().DB.Select<Employee>(nameof(maker.ID), employee.ID));
            Assert.IsNull(InventoryDataCommander.GetInstance().DB.Select<Warehouse>(nameof(maker.ID), warehouse.ID));
            Assert.IsNull(InventoryDataCommander.GetInstance().DB.Select<Project>(nameof(maker.ID), project.ID));
        }

        /// <summary>
        /// 필드 삭제 체크
        /// Db 삭제 확인
        /// </summary>
        [TestMethod]
        public void DeleteFieldThenSyncComboBoxItems()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            var maker = viewmodel.MakerList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(maker);
            var measure = viewmodel.MeasureList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(measure);
            var client = viewmodel.ClientList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(client);
            var employee = viewmodel.EmployeeList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(employee);
            var warehouse = viewmodel.WarehouseList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(warehouse);
            var project = viewmodel.ProjectList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(project);

            Assert.IsTrue(viewmodel.MakerList.All(x => x != maker));
            Assert.IsTrue(viewmodel.MeasureList.All(x => x != measure));
            Assert.IsTrue(viewmodel.ClientList.All(x => x != client));
            Assert.IsTrue(viewmodel.EmployeeList.All(x => x != employee));
            Assert.IsTrue(viewmodel.WarehouseList.All(x => x != warehouse));
            Assert.IsTrue(viewmodel.ProjectList.All(x => x != project));
        }

        /// <summary>
        /// 필드 삭제 체크
        /// Inventory Format 체크
        /// </summary>
        [TestMethod]
        public void DeleteFieldThenSyncInventoryDirector()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            var maker = viewmodel.MakerList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(maker);
            var measure = viewmodel.MeasureList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(measure);

            var inventories = InventoryDataCommander.GetInstance().CopyInventories();
            foreach (var inventory in inventories)
            {
                Assert.IsTrue(inventory.Maker == null || inventory.Maker.ID != maker.ID);
                Assert.IsTrue(inventory.Measure == null || inventory.Measure.ID != measure.ID);
            }
        }

        // <summary>
        /// 필드 삭제 체크
        /// Db Inventory Format 체크
        /// </summary>
        [TestMethod]
        public void DeleteFieldThenSyncDbInventoryFormat()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            var maker = viewmodel.MakerList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(maker);
            var measure = viewmodel.MeasureList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(measure);

            var inventories = InventoryDataCommander.GetInstance().DB.Select<InventoryFormat>();
            Assert.IsTrue(inventories.All(x => x.MakerID != maker.ID));
            Assert.IsTrue(inventories.All(x => x.MeasureID != measure.ID));
        }

        /// <summary>
        /// 필드 삭제 체크
        /// Db 삭제 확인
        /// </summary>
        [Ignore]
        [TestMethod]
        public void DeleteFieldThenSyncDbIOStockFormat()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            var client = viewmodel.ClientList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(client);
            var employee = viewmodel.EmployeeList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(employee);
            var warehouse = viewmodel.WarehouseList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(warehouse);
            var project = viewmodel.ProjectList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(project);

            var stocks = InventoryDataCommander.GetInstance().DB.Select<IOStockFormat>();

            Assert.IsTrue(stocks.All(x => x.SupplierID != client.ID));
            Assert.IsTrue(stocks.All(x => x.EmployeeID != employee.ID));
            Assert.IsTrue(stocks.All(x => x.WarehouseID != warehouse.ID));
            Assert.IsTrue(stocks.All(x => x.ProjectID != project.ID));
        }

        /// <summary>
        /// 필드 삭제 체크
        /// Db 삭제 확인
        /// </summary>
        [TestMethod]
        public void DeleteFieldThenSyncIOStockStatusViewModel()
        {
            new Dummy().Create();
            IOStockStatusViewModel status = new IOStockStatusViewModel();
            status.DatePickerViewModel.LastYearCommand.Execute(null);

            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(status);
            var client = viewmodel.ClientList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(client);
            var employee = viewmodel.EmployeeList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(employee);
            var warehouse = viewmodel.WarehouseList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(warehouse);
            var project = viewmodel.ProjectList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(project);

            var items = status.DataGridViewModel.Items;
            foreach (var item in items)
            {
                Assert.IsTrue(item.Supplier == null || item.Supplier.ID != client.ID);
                Assert.IsTrue(item.Employee == null || item.Employee.ID != employee.ID);
                Assert.IsTrue(item.Warehouse == null || item.Warehouse.ID != warehouse.ID);
                Assert.IsTrue(item.Project == null || item.Project.ID != project.ID);
            }
        }

        [TestMethod]
        public void DeleteFieldThenSyncInventoryStatusViewModel()
        {
            new Dummy().Create();
            InventoryStatusViewModel isvm = new InventoryStatusViewModel();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            var maker = viewmodel.MakerList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(maker);
            var measure = viewmodel.MeasureList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(measure);

            var items = isvm.GetDataGridItems();
            Assert.IsTrue(items.All(x => maker != x.Maker));
            Assert.IsTrue(items.All(x => measure != x.Measure));
        }

        public void CheckDeletePerfectly<T>(T field) where T : class, IField, new()
        {
            var find = InventoryDataCommander.GetInstance().SearchField<T>(field.ID);
            Assert.IsNull(find);
            var select = InventoryDataCommander.GetInstance().DB.Select<T>(nameof(field.ID), field.ID);
            Assert.IsNull(select);
        }

        //[TestMethod]
        //public void TotalPriceTest()
        //{
        //    new Dummy().Create();
        //    int unitPrice = 1000;
        //    int quantity = 2;

        //    IOStockStatusViewModel stViewModel = new IOStockStatusViewModel();
        //    stViewModel.DatePickerViewModel.TodayCommand.Execute(null);

        //    var items = stViewModel.DataGridViewModel.Items;

        //    IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(stViewModel);
        //    viewmodel.ProductText = "new product";
        //    viewmodel.SpecificationText = "new specification";
        //    viewmodel.UnitPrice = unitPrice;
        //    viewmodel.Quantity = quantity;

        //    viewmodel.RecordAsync();

        //    var item = items[0];
        //    Assert.AreEqual(item.UnitPrice, unitPrice);
        //    Assert.AreEqual(item.Inventory.Quantity, quantity);
        //    Assert.AreEqual(item.TotalPrice, item.UnitPrice * item.Inventory.Quantity);

        //}
    }
}