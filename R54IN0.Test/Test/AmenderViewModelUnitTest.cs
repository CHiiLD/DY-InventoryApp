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
            viewmodel.Product = ObservableFieldDirector.GetInstance().CopyObservableFields<Product>().Random();
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
            viewmodel.Product = ObservableFieldDirector.GetInstance().CopyObservableFields<Product>().Random();
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
            viewmodel.Product = ObservableFieldDirector.GetInstance().CopyObservableFields<Product>().Random();
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
            viewmodel.Product = ObservableFieldDirector.GetInstance().CopyObservableFields<Product>().Random();
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
            amender.Product = ObservableFieldDirector.GetInstance().SearchObservableField<Product>(node.ProductID);
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
            amender.Product = ObservableFieldDirector.GetInstance().SearchObservableField<Product>(node.ProductID);
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
            viewmodel.Product = ObservableFieldDirector.GetInstance().CopyObservableFields<Product>().Random();
            viewmodel.Inventory = viewmodel.InventoryList.Random();
            string client = viewmodel.ClientText = "some client";
            string warehouse = viewmodel.WarehouseText = "some warehouse";
            string employee = viewmodel.EmployeeText = "some name";

            var record = await viewmodel.RecordAsync();

            Assert.AreEqual(employee, record.Employee.Name);
            Assert.IsNotNull(ObservableFieldDirector.GetInstance().SearchObservableField<Employee>(record.Employee.ID));

            Assert.AreEqual(warehouse, record.Warehouse.Name);
            Assert.IsNotNull(ObservableFieldDirector.GetInstance().SearchObservableField<Warehouse>(record.Warehouse.ID));

            Assert.AreEqual(client, record.Supplier.Name);
            Assert.IsNotNull(ObservableFieldDirector.GetInstance().SearchObservableField<Supplier>(record.Supplier.ID));
        }

        [TestMethod]
        public async Task RecordNewInventoryThenAddedInventoryDirector()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.OUTGOING;
            viewmodel.Product = ObservableFieldDirector.GetInstance().CopyObservableFields<Product>().Random();
            viewmodel.Inventory = null;
            var text = viewmodel.SpecificationText = "new inventory";

            var record = await viewmodel.RecordAsync();

            Assert.AreEqual(text, record.Inventory.Specification);
            Assert.IsNotNull(record.Inventory.ID);
            Assert.IsNotNull(ObservableInventoryDirector.GetInstance().SearchObservableInventory(record.Inventory.ID));
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
            Assert.IsNotNull(ObservableFieldDirector.GetInstance().SearchObservableField<Product>(record.Inventory.Product.ID));
            Assert.IsNotNull(ObservableInventoryDirector.GetInstance().SearchObservableInventory(record.Inventory.ID));
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

            Assert.IsNull(ObservableFieldDirector.GetInstance().SearchObservableField<Maker>(maker.ID));
            Assert.IsNull(ObservableFieldDirector.GetInstance().SearchObservableField<Measure>(measure.ID));
            Assert.IsNull(ObservableFieldDirector.GetInstance().SearchObservableField<Supplier>(client.ID));
            Assert.IsNull(ObservableFieldDirector.GetInstance().SearchObservableField<Employee>(employee.ID));
            Assert.IsNull(ObservableFieldDirector.GetInstance().SearchObservableField<Warehouse>(warehouse.ID));
            Assert.IsNull(ObservableFieldDirector.GetInstance().SearchObservableField<Project>(project.ID));
        }

        /// <summary>
        /// 필드 삭제 체크
        /// Db 삭제 확인
        /// </summary>
        [TestMethod]
        public async Task DeleteFieldThenSyncDb()
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

            Assert.IsNull(await DbAdapter.GetInstance().SelectAsync<Maker>(maker.ID));
            Assert.IsNull(await DbAdapter.GetInstance().SelectAsync<Measure>(measure.ID));
            Assert.IsNull(await DbAdapter.GetInstance().SelectAsync<Supplier>(client.ID));
            Assert.IsNull(await DbAdapter.GetInstance().SelectAsync<Employee>(employee.ID));
            Assert.IsNull(await DbAdapter.GetInstance().SelectAsync<Warehouse>(warehouse.ID));
            Assert.IsNull(await DbAdapter.GetInstance().SelectAsync<Project>(project.ID));
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

            var inventories = ObservableInventoryDirector.GetInstance().CopyObservableInventories();
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
        public async Task DeleteFieldThenSyncDbInventoryFormat()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            var maker = viewmodel.MakerList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(maker);
            var measure = viewmodel.MeasureList.Random();
            viewmodel.ComboBoxItemDeleteCommand.Execute(measure);

            var inventories = await DbAdapter.GetInstance().SelectAllAsync<InventoryFormat>();
            Assert.IsTrue(inventories.All(x => x.MakerID != maker.ID));
            Assert.IsTrue(inventories.All(x => x.MeasureID != measure.ID));
        }

        /// <summary>
        /// 필드 삭제 체크
        /// Db 삭제 확인
        /// </summary>
        [Ignore]
        [TestMethod]
        public async Task DeleteFieldThenSyncDbIOStockFormat()
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

            var stocks = await DbAdapter.GetInstance().SelectAllAsync<IOStockFormat>();

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

        public async Task CheckDeletePerfectly<T>(T field) where T : class, IField, new()
        {
            var find = ObservableFieldDirector.GetInstance().SearchObservableField<T>(field.ID);
            Assert.IsNull(find);
            var select = await DbAdapter.GetInstance().SelectAsync<T>(field.ID);
            Assert.IsNull(select);
        }
    }
}