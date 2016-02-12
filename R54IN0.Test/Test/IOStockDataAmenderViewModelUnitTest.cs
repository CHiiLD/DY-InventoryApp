using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace R54IN0.Test.New
{
    [TestClass]
    public class IOStockDataAmenderViewModelTest
    {
        /// <summary>
        /// 생성자 검사
        /// </summary>
        [TestMethod]
        public void CanCreate()
        {
            new Dummy().Create();
            new IOStockDataAmenderViewModel();
            using (var db = LexDb.GetDbInstance())
            {
                var fmt = db.LoadAll<IOStockFormat>().Random();
                new IOStockDataAmenderViewModel(new ObservableIOStock(fmt));
            }
        }

        /// <summary>
        /// 새로운 제품과 새로운 규격을 설정하여 추가한다.
        /// </summary>
        [TestMethod]
        public async Task Add()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            InventoryStatusViewModel inventoryStatusViewModel = new InventoryStatusViewModel();
            IOStockStatusViewModel iostockStatusViewModel = new IOStockStatusViewModel();
            //날짜를 선택
            iostockStatusViewModel.SelectedGroupItem = IOStockStatusViewModel.GROUPITEM_DATE;
            iostockStatusViewModel.DatePickerViewModel.TodayCommand.Execute(null); //올해 버튼을 클릭
            Assert.IsFalse(viewmodel.IsReadOnlyProductTextBox); //제품 이름 변경 가능
            Assert.IsTrue(viewmodel.ProductSearchCommand.CanExecute(null)); //제품 선택 가능
            //정보 입력
            viewmodel.StockType = IOStockType.INCOMING;
            viewmodel.ProductText = "some product";
            Assert.IsTrue(viewmodel.Product == null);
            var spec = viewmodel.SpecificationText = "some specification";
            var specMemo = viewmodel.SpecificationMemo = "some specification meno";
            var maker = viewmodel.MakerText = "some maker";
            var measure = viewmodel.MeasureText = "some measure";
            var client = viewmodel.ClientText = "some client";
            var warehouse = viewmodel.WarehouseText = "some warehouse";
            var project = viewmodel.ProjectText = "some project";
            var employee = viewmodel.EmployeeText = "some name";
            var memo = viewmodel.Memo = "some memo";
            var qty = viewmodel.Quantity = 666;
            var price = viewmodel.UnitPrice = 6666;
            var remain = viewmodel.RemainingQuantity;

            //저장
            var iostock = await viewmodel.RecordAsync();
            Assert.IsTrue(iostock.StockType == IOStockType.INCOMING);
            Assert.IsTrue(iostock.Supplier != null);
            Assert.IsTrue(iostock.Customer == null);
            //입출고 데이터그리드에 추가되었는지 확인
            Assert.IsTrue(iostockStatusViewModel.DataGridViewModel.Items.Any(x => x.ID == iostock.ID));
            var inoutStock = iostockStatusViewModel.DataGridViewModel.Items.Where(x => x.ID == iostock.ID).Single();
            //재고 데이터그리드에 추가되었는지 확인
            Assert.IsTrue(inventoryStatusViewModel.GetDataGridItems().Any(x => x.ID == iostock.Inventory.ID));
            var inventory = inventoryStatusViewModel.GetDataGridItems().Where(x => x.ID == iostock.Inventory.ID).Single();
            var oid = InventoryDataCommander.GetInstance();
            //검사
            Assert.IsNotNull(oid.SearchObservableInventory(iostock.Inventory.ID)); //inventory director에 추가되었는지 확인한다.
            Assert.AreEqual(spec, iostock.Inventory.Specification);
            Assert.AreEqual(specMemo, iostock.Inventory.Memo);
            Assert.AreEqual(maker, iostock.Inventory.Maker.Name);
            Assert.AreEqual(measure, iostock.Inventory.Measure.Name);
            Assert.AreEqual(client, iostock.Supplier.Name);
            Assert.AreEqual(warehouse, iostock.Warehouse.Name);
            Assert.AreEqual(employee, iostock.Employee.Name);
            Assert.AreEqual((int)qty, iostock.Quantity);
            Assert.AreEqual(memo, iostock.Memo);
            Assert.AreEqual(price, iostock.UnitPrice);
            Assert.AreEqual(qty, iostock.RemainingQuantity);
            Assert.AreEqual((int)qty, iostock.Inventory.Quantity);
            var ofd = InventoryDataCommander.GetInstance();
            Assert.IsNotNull(ofd.SearchObservableField<Maker>(iostock.Inventory.Maker.ID));
            Assert.IsNotNull(ofd.SearchObservableField<Measure>(iostock.Inventory.Measure.ID));
        }

        /// <summary>
        /// 기존의 제품의 새로운 규격을 사용하여 입고 기록을 만든다.
        /// </summary>
        [TestMethod]
        public async Task Add2()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            //설정
            viewmodel.StockType = IOStockType.INCOMING;
            var product = viewmodel.Product = InventoryDataCommander.GetInstance().CopyObservableFields<Product>().Random();
            var inven = viewmodel.Inventory = viewmodel.InventoryList.Random();
            var specMemo = viewmodel.SpecificationMemo = "some memo";
            var maker = viewmodel.Maker = viewmodel.MakerList.Random();
            var measure = viewmodel.Measure = viewmodel.MeasureList.Random();
            var client = viewmodel.Client = viewmodel.ClientList.Random();
            var warehouse = viewmodel.Warehouse = viewmodel.WarehouseList.Random();
            var employee = viewmodel.Employee = viewmodel.EmployeeList.Random();
            var memo = viewmodel.Memo = "some memo";
            viewmodel.UnitPrice = 6666;
            var stock = await viewmodel.RecordAsync();
            //검사
            Assert.AreEqual(product, stock.Inventory.Product);
            Assert.AreEqual(inven.ID, stock.Inventory.ID);
            Assert.AreEqual(specMemo, stock.Inventory.Memo);
            Assert.AreEqual(maker, stock.Inventory.Maker);
            Assert.AreEqual(measure, stock.Inventory.Measure);
            Assert.AreEqual(warehouse, stock.Warehouse);
            Assert.AreEqual(employee, stock.Employee);
            Assert.AreEqual(memo, stock.Memo);
        }

        /// <summary>
        /// 기존의 제품을 사용하되, 새로운 규격을 사용하여 새로운 입고 기록을 만든다.
        /// </summary>
        [TestMethod]
        public async Task Add3()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            //설정
            var product = viewmodel.Product = InventoryDataCommander.GetInstance().CopyObservableFields<Product>().Random();
            viewmodel.Inventory = null;
            var specificationName = viewmodel.SpecificationText = "some specification";
            var specMemo = viewmodel.SpecificationMemo = "some specification memo";
            var maker = viewmodel.Maker = viewmodel.MakerList.Random();
            var measure = viewmodel.Measure = viewmodel.MeasureList.Random();
            var client = viewmodel.Client = viewmodel.ClientList.Random();
            var warehouse = viewmodel.Warehouse = viewmodel.WarehouseList.Random();
            var employee = viewmodel.Employee = viewmodel.EmployeeList.Random();
            var memo = viewmodel.Memo = "some memo";
            viewmodel.UnitPrice = 6666;
            var obIOStock = await viewmodel.RecordAsync();
            //검사
            Assert.AreEqual(product, obIOStock.Inventory.Product);
            Assert.AreEqual(specificationName, obIOStock.Inventory.Specification);
            Assert.AreEqual(specMemo, obIOStock.Inventory.Memo);
            Assert.AreEqual(maker, obIOStock.Inventory.Maker);
            Assert.AreEqual(measure, obIOStock.Inventory.Measure);
            Assert.AreEqual(warehouse, obIOStock.Warehouse);
            Assert.AreEqual(employee, obIOStock.Employee);
            Assert.AreEqual(memo, obIOStock.Memo);
        }

        /// <summary>
        /// 추가가 아닌 기존의 IOStockFormat을 수정하고자 할 때
        /// </summary>
        [TestMethod]
        public async Task Modify()
        {
            new Dummy().Create();
            IOStockFormat fmt = null;
            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadAll<IOStockFormat>().Random();
            IOStockType type = fmt.StockType;
            IOStockStatusViewModel iostockStatusViewModel = new IOStockStatusViewModel();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(iostockStatusViewModel, new ObservableIOStock(fmt));

            Assert.AreEqual(type, viewmodel.StockType);
            string id = fmt.ID;
            var clientText = viewmodel.ClientText = "SOME CLIENT";
            viewmodel.Client = null;
            var warehouseText = viewmodel.WarehouseText = "SOME WAREHOUSE";
            viewmodel.Warehouse = null;
            var employeeText = viewmodel.EmployeeText = "SOME EMPLOYEE";
            viewmodel.Employee = null;
            var projectText = viewmodel.ProjectText = "SOME PROJECT";
            viewmodel.Project = null;
            var memo = viewmodel.Memo = "SOME MEMO";
            var specificationMemo = viewmodel.SpecificationMemo = "SOME SPEC_MEMO";
            var makerText = viewmodel.MakerText = "SOME MAKER";
            viewmodel.Maker = null;
            var measureText = viewmodel.MeasureText = "SOME MEASURE";
            viewmodel.Measure = null;
            var qty = viewmodel.Quantity = 1111;
            var price = viewmodel.UnitPrice = 30302;

            await viewmodel.RecordAsync();

            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadByKey<IOStockFormat>(fmt.ID);
            ObservableIOStock stock = new ObservableIOStock(fmt);

            if (stock.StockType == IOStockType.INCOMING)
            {
                Assert.AreEqual(clientText, stock.Supplier.Name);
                Assert.AreEqual(warehouseText, stock.Warehouse.Name);
            }
            else if (stock.StockType == IOStockType.OUTGOING)
            {
                Assert.AreEqual(clientText, stock.Customer.Name);
                Assert.AreEqual(projectText, stock.Project.Name);
            }
            Assert.AreEqual(employeeText, stock.Employee.Name);
            Assert.AreEqual(memo, stock.Memo);
            Assert.AreEqual(makerText, stock.Inventory.Maker.Name);
            Assert.AreEqual(measureText, stock.Inventory.Measure.Name);
            Assert.AreEqual(type, stock.StockType);

            var ofd = InventoryDataCommander.GetInstance();
            Assert.IsNotNull(ofd.SearchObservableField<Maker>(stock.Inventory.Maker.ID));
            Assert.IsNotNull(ofd.SearchObservableField<Measure>(stock.Inventory.Measure.ID));
        }

        [TestMethod]
        public async Task Modify2()
        {
            new Dummy().Create();
            IOStockFormat fmt = null;
            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadAll<IOStockFormat>().Random();
            var viewmodel = new IOStockDataAmenderViewModel(new ObservableIOStock(fmt));

            var specMemo = viewmodel.SpecificationMemo = "SOME SPECIFICATION MEMO";
            var maker = viewmodel.Maker = viewmodel.MakerList.Random();
            var measure = viewmodel.Measure = viewmodel.MeasureList.Random();
            var client = viewmodel.Client = viewmodel.ClientList.Random();
            var warehouse = viewmodel.Warehouse = viewmodel.WarehouseList.Random();
            var employee = viewmodel.Employee = viewmodel.EmployeeList.Random();
            var memo = viewmodel.Memo = "MEMO";
            var price = viewmodel.UnitPrice = 7777;
            var stock = await viewmodel.RecordAsync();

            Assert.AreEqual(specMemo, stock.Inventory.Memo);
            Assert.AreEqual(maker, stock.Inventory.Maker);
            Assert.AreEqual(measure, stock.Inventory.Measure);
            Assert.AreEqual(warehouse, stock.Warehouse);
            Assert.AreEqual(employee, stock.Employee);
            Assert.AreEqual(memo, stock.Memo);
            Assert.AreEqual(price, stock.UnitPrice);
        }

        [TestMethod]
        public async Task Modify3()
        {
            new Dummy().Create();
            IOStockFormat fmt = null;
            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadAll<IOStockFormat>().Random();
            var viewmodel = new IOStockDataAmenderViewModel(new ObservableIOStock(fmt));

            var inven = InventoryDataCommander.GetInstance().SearchObservableInventory(fmt.InventoryID);
            string makerId = inven.Maker.ID;
            string measureId = inven.Measure.ID;

            viewmodel.Maker = null;
            viewmodel.Measure = null;
            viewmodel.Client = null;
            viewmodel.Warehouse = null;
            viewmodel.Employee = null;

            string maker = viewmodel.MakerText = "some maker";
            string measure = viewmodel.MeasureText = "some measure";
            string client = viewmodel.ClientText = "some client";
            string ware = viewmodel.WarehouseText = "some warehouse";
            string employee = viewmodel.EmployeeText = "some employee";

            var stock = await viewmodel.RecordAsync();

            switch (stock.StockType)
            {
                case IOStockType.INCOMING:
                    Assert.AreEqual(ware, stock.Warehouse.Name);
                    Assert.AreEqual(fmt.WarehouseID, stock.Warehouse.ID);
                    break;

                case IOStockType.OUTGOING:

                    break;
            }
            Assert.AreEqual(maker, stock.Inventory.Maker.Name);
            Assert.AreEqual(measure, stock.Inventory.Measure.Name);
            Assert.AreEqual(employee, stock.Employee.Name);
            Assert.AreEqual(fmt.EmployeeID, stock.Employee.ID);
            Assert.AreEqual(makerId, stock.Inventory.Maker.ID);
            Assert.AreEqual(measureId, stock.Inventory.Measure.ID);
        }

        /// <summary>
        /// 1. Employee에 null을 할당한다.
        /// 2. 편집기에서 EmployeeText를 할당하고 Employee.Name(with db)에 저장
        /// 3. 변경됨을 확인
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Modify4()
        {
            new Dummy().Create();
            var formats = await DbAdapter.GetInstance().SelectAllAsync<IOStockFormat>();
            var fmt = formats.Random();
            ObservableIOStock ios = new ObservableIOStock(fmt);
            //DataGrid 업데이트
            IOStockStatusViewModel iosViewModel = new IOStockStatusViewModel();
            TreeViewNode node = TreeViewNodeDirector.GetInstance().SearchObservableObjectNode(ios.Inventory.Product.ID);
            if (node != null)
            {
                MultiSelectTreeViewModelView treeView = iosViewModel.TreeViewViewModel;
                treeView.SelectedNodes.Clear();
                treeView.ExecuteNodesSelectedEventCommand(new SelectionChangedCancelEventArgs(new TreeViewNode[] { node }, null));
            }
            Assert.AreNotEqual(0, iosViewModel.DataGridViewModel.Items.Count);

            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(); //추가모드로 열기
            viewmodel.Product = ios.Inventory.Product;
            viewmodel.Inventory = viewmodel.InventoryList.Where(x => x.ID == ios.Inventory.ID).Single();
            viewmodel.Employee = null; //null 할당
            viewmodel.EmployeeText = null;

            var newRecord = await viewmodel.RecordAsync();
            newRecord = iosViewModel.DataGridViewModel.Items.Where(x => x.ID == newRecord.ID).Single();
            Assert.IsNull(newRecord.Employee);

            viewmodel = new IOStockDataAmenderViewModel(newRecord); //편집모드로 열기
            Assert.IsNull(viewmodel.Employee);
            viewmodel.EmployeeText = "Jojo";
            newRecord = await viewmodel.RecordAsync();

            var jojoEmp = newRecord.Employee;
            Assert.IsNotNull(newRecord.Employee);
            Assert.AreEqual(newRecord.Employee.Name, "Jojo");

            Assert.IsNotNull(InventoryDataCommander.GetInstance().SearchObservableField<Employee>(newRecord.Employee.ID));

            newRecord = iosViewModel.DataGridViewModel.Items.Where(x => x.ID == newRecord.ID).Single();
            viewmodel = new IOStockDataAmenderViewModel(newRecord); //편집모드로 열기
            viewmodel.Employee = null;
            viewmodel.EmployeeText = "Jojo2";
            var newnewRecord = await viewmodel.RecordAsync();

            Assert.AreEqual(newnewRecord.Employee.Name, "Jojo2");
            Assert.AreEqual(jojoEmp.ID, newRecord.Employee.ID);
        }

        [TestMethod]
        public async Task Modify5()
        {
            new Dummy().Create();
            var formats = await DbAdapter.GetInstance().SelectAllAsync<IOStockFormat>();
            var fmt = formats.Random();
            ObservableIOStock ios = new ObservableIOStock(fmt);
            //DataGrid 업데이트
            IOStockStatusViewModel iosViewModel = new IOStockStatusViewModel();
            TreeViewNode node = TreeViewNodeDirector.GetInstance().SearchObservableObjectNode(ios.Inventory.Product.ID);
            if (node != null)
            {
                MultiSelectTreeViewModelView treeView = iosViewModel.TreeViewViewModel;
                treeView.SelectedNodes.Clear();
                treeView.ExecuteNodesSelectedEventCommand(new SelectionChangedCancelEventArgs(new TreeViewNode[] { node }, null));
            }
            Assert.AreNotEqual(0, iosViewModel.DataGridViewModel.Items.Count);

            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(); //추가모드로 열기
            viewmodel.Product = ios.Inventory.Product;
            viewmodel.Inventory = viewmodel.InventoryList.Where(x => x.ID == ios.Inventory.ID).Single();
            viewmodel.Maker = null; //null 할당
            viewmodel.Maker = null;

            var newRecord = await viewmodel.RecordAsync();
            newRecord = iosViewModel.DataGridViewModel.Items.Where(x => x.ID == newRecord.ID).Single();
            Assert.IsNull(newRecord.Inventory.Maker);

            viewmodel = new IOStockDataAmenderViewModel(newRecord); //편집모드로 열기
            Assert.IsNull(viewmodel.Maker);
            viewmodel.MakerText = "maKeR";
            newRecord = await viewmodel.RecordAsync();

            var jojoEmp = newRecord.Inventory.Maker;
            Assert.IsNotNull(newRecord.Inventory);
            Assert.AreEqual(newRecord.Inventory.Maker.Name, "maKeR");

            Assert.IsNotNull(InventoryDataCommander.GetInstance().SearchObservableField<Maker>(newRecord.Inventory.Maker.ID));

            newRecord = iosViewModel.DataGridViewModel.Items.Where(x => x.ID == newRecord.ID).Single();
            viewmodel = new IOStockDataAmenderViewModel(newRecord); //편집모드로 열기
            viewmodel.Maker = null;
            viewmodel.MakerText = "maKeR2";
            var newnewRecord = await viewmodel.RecordAsync();

            Assert.AreEqual(newnewRecord.Inventory.Maker.Name, "maKeR2");
            Assert.AreEqual(jojoEmp.ID, newRecord.Inventory.Maker.ID);
        }

        /// <summary>
        /// 출고 시, 출고된 수량만큼 RemainingQuantity, InventoryQuantity 프로퍼티가
        /// 정상적으로 값을 가감하는지 테스트
        /// </summary>
        [TestMethod]
        public void Quantity()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();

            viewmodel.StockType = IOStockType.OUTGOING;
            var product = viewmodel.Product = InventoryDataCommander.GetInstance().CopyObservableFields<Product>().Random();
            var specificationName = viewmodel.Inventory = viewmodel.InventoryList.Random();
            var inventoryQty = viewmodel.Inventory.Quantity;
            if (inventoryQty <= 1)
                return;
            viewmodel.Quantity = inventoryQty - 1; //현재 재고는 1개

            Assert.AreEqual(1, viewmodel.InventoryQuantity); //현재 재고도 1개
        }

        /// <summary>
        /// 기존의 입출고 데이터의 입출고 수량을 수정하고 이에 따른 수량 데이터들의 변화가 올바르게 적용됨을 확인한다.
        /// </summary>
        [TestMethod]
        public void Quantity2()
        {
            new Dummy().Create();
            IOStockFormat fmt = null;
            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadAll<IOStockFormat>().Where(x => x.StockType == IOStockType.INCOMING).OrderBy(x => x.Date).Last();

            ///////////////////////////////////////////입고
            fmt.Quantity = 100; //입고 수량
            fmt.RemainingQuantity = 200; //잔여 재고
            var obIOStock = new ObservableIOStock(fmt);
            obIOStock.Inventory.Quantity = 500; //현재 재고
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(obIOStock);
            viewmodel.StockType = IOStockType.INCOMING;
            viewmodel.Quantity = 99; //입고수량 하나 줄임
            Assert.AreEqual(499, viewmodel.InventoryQuantity); //따라서 현재 재고도 하나 줄어든다.
            viewmodel.Quantity = 101;
            Assert.AreEqual(501, viewmodel.InventoryQuantity);

            ///////////////////////////////////////////출고
            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadAll<IOStockFormat>().Where(x => x.StockType == IOStockType.OUTGOING).OrderBy(x => x.Date).Last();
            fmt.Quantity = 100; //들어간 양
            fmt.RemainingQuantity = 200; //잔여 재고
            obIOStock = new ObservableIOStock(fmt);
            obIOStock.Inventory.Quantity = 500; //현재 재고
            viewmodel = new IOStockDataAmenderViewModel(obIOStock);
            viewmodel.StockType = IOStockType.OUTGOING;
            viewmodel.Quantity = 99; //출고수량 하나 줄임
            Assert.AreEqual(501, viewmodel.InventoryQuantity); //따라서 현재 재고도 하나 늘어난다.
            viewmodel.Quantity = 101;
            Assert.AreEqual(499, viewmodel.InventoryQuantity);
        }

        /// <summary>
        /// 과거의 입출고 데이터 중 입고수량 또는 출고 수량을 수정하였다면
        /// 그에 맞게 과거부터 현재까지의 모든 잔여 수량과 재고수량을 수정한다.
        /// </summary>
        [TestMethod]
        public async Task Quantity3()
        {
            new Dummy().Create();
            IOStockStatusViewModel iostockStatusViewModel = new IOStockStatusViewModel();
            IOStockFormat fmt = null;
            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadAll<IOStockFormat>().Random();
            IObservableIOStockProperties obIOStock = new ObservableIOStock(fmt);
            var inventoryQty = obIOStock.Inventory.Quantity;
            var viewmodel = new IOStockDataAmenderViewModel(iostockStatusViewModel, obIOStock);
            viewmodel.Quantity = 12;
            obIOStock = await viewmodel.RecordAsync();
            await CheckQuantity(obIOStock.Inventory.ID);
        }

        /// <summary>
        /// 현재가 아닌 과거의 Date로 새로운 입출고 데이터를 추가했을 경우 과거를 기준으로 현재까지 관련된 모든
        /// 입출고 데이터의 잔여 수량과 재고수량이 동기화되어야 한다.
        /// </summary>
        [TestMethod]
        public async Task Quantity4()
        {
            new Dummy().Create();
            IOStockStatusViewModel iostockStatusViewModel = new IOStockStatusViewModel();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(iostockStatusViewModel);
            //설정
            viewmodel.StockType = new Random().NextDouble() > 0.5 ? IOStockType.INCOMING : IOStockType.OUTGOING;
            var product = viewmodel.Product = InventoryDataCommander.GetInstance().CopyObservableFields<Product>().Random();
            var inven = viewmodel.Inventory = viewmodel.InventoryList.Random();
            var beforeInvenQty = inven.Quantity;
            var date = viewmodel.Date = DateTime.Now.AddDays(-600.0 * new Random().NextDouble()); //과거로 저장
            int qty = new Random().Next(1, 10);
            viewmodel.Quantity = qty;
            //저장
            IObservableIOStockProperties obIOStock = await viewmodel.RecordAsync();
            await CheckQuantity(obIOStock.Inventory.ID);
        }

        /// <summary>
        /// 입출고 데이터를 수정하는데, 그 날짜가 과거이다.
        /// 이럴 경우 입출고 수량의 변동에 따라 잔여수량과 입고 수량의 변이를 올바르게 업데이트 할 수 있어야 한다.
        /// </summary>
        [TestMethod]
        public void Quantity5()
        {
            new Dummy().Create();
            IOStockFormat fmt = null;
            IEnumerable<IOStockFormat> formats;
            var inventory = InventoryDataCommander.GetInstance().CopyObservableInventories().Random();
            using (var db = LexDb.GetDbInstance())
            {
                formats = db.LoadAll<IOStockFormat>().Where(x => x.InventoryID == inventory.ID).OrderBy(x => x.Date)
                    .ToList();
                fmt = formats.ElementAt(new Random().Next(0, formats.Count() - 2)); //최신 데이터는 넣지 말도록
            }
            var obIOStock = new ObservableIOStock(fmt);
            int inventoryQty = obIOStock.Inventory.Quantity; //오리진 재고 수량
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(obIOStock);

            int ioStockQty = fmt.Quantity; //오리진 입출고 수량

            int changeQty = 1;
            int effectQty = viewmodel.StockType == IOStockType.INCOMING ? changeQty : -changeQty;

            viewmodel.Quantity = ioStockQty + changeQty; //입출고 수량을 하나 증가함
            Assert.AreEqual(viewmodel.InventoryQuantity, inventoryQty + effectQty);
        }

        /// <summary>
        /// 입출고 데이터를 추가하는데 그 날짜가 과거이다.
        /// 이럴 경우 입출고 수량의 변동에 따라 잔여수량과 입고 수량의 변이를 올바르게 업데이트 할 수 있어야 한다.
        /// </summary>
        [TestMethod]
        public void Quantity6()
        {
            new Dummy().Create();
            IOStockFormat fmt = null;
            IEnumerable<IOStockFormat> formats;
            var inventory = InventoryDataCommander.GetInstance().CopyObservableInventories().Random();
            using (var db = LexDb.GetDbInstance())
            {
                formats = db.LoadAll<IOStockFormat>().Where(x => x.InventoryID == inventory.ID).OrderBy(x => x.Date)
                    .ToList();
            }
            fmt = formats.ElementAt(new Random().Next(0, formats.Count() - 2));

            var iostock = new ObservableIOStock(fmt);
            int inventoryQty = iostock.Inventory.Quantity;
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.INCOMING;
            viewmodel.Product = iostock.Inventory.Product;
            viewmodel.Inventory = viewmodel.InventoryList.Where(x => x.ID == iostock.Inventory.ID).Single();

            //범위1 F .. T ... L
            viewmodel.Date = fmt.Date.AddMilliseconds(10);
            var lastst = formats.Where(x => x.Date < viewmodel.Date).OrderBy(x => x.Date).Last();
            int qty = 1;
            viewmodel.Quantity = qty;
            Assert.AreEqual(inventoryQty + qty, viewmodel.InventoryQuantity); //재고수량 증가 확인
            viewmodel.StockType = IOStockType.OUTGOING;
            Assert.AreEqual(inventoryQty - qty, viewmodel.InventoryQuantity); //재고수량 증가 확인

            //범위2 TF .... L
            viewmodel.StockType = IOStockType.INCOMING;
            viewmodel.Date = formats.First().Date.AddMilliseconds(-10000);
            Assert.AreEqual(inventoryQty + qty, viewmodel.InventoryQuantity); //재고수량 증가 확인

            viewmodel.StockType = IOStockType.OUTGOING;
            viewmodel.Date = formats.First().Date.AddMilliseconds(-10000);
            Assert.AreEqual(inventoryQty - qty, viewmodel.InventoryQuantity); //재고수량 증가 확인

            //범위3 F .... LT
            viewmodel.StockType = IOStockType.INCOMING;
            viewmodel.Date = formats.Last().Date.AddMilliseconds(1000);
            lastst = formats.Where(x => x.Date < viewmodel.Date).OrderBy(x => x.Date).Last();
            Assert.AreEqual(inventoryQty + qty, viewmodel.InventoryQuantity); //재고수량 증가 확인

            viewmodel.StockType = IOStockType.OUTGOING;
            Assert.AreEqual(inventoryQty - qty, viewmodel.InventoryQuantity); //재고수량 증가 확인
        }

        /// <summary>
        /// 제일 과거의 출고 데이터를 넣을 경우
        /// 잔여수량이 꼬이는 이슈를 해결
        /// </summary>
        [TestMethod]
        public async Task Quantity7()
        {
            new Dummy().Create();
            IOStockFormat fmt = null;
            IEnumerable<IOStockFormat> formats;
            var inventory = InventoryDataCommander.GetInstance().CopyObservableInventories().Random();
            formats = await DbAdapter.GetInstance().QueryAsync<IOStockFormat>(
                DbCommand.WHERE, "InventoryID", inventory.ID, DbCommand.ASCENDING, "Date");
            fmt = formats.First();
            var iostock = new ObservableIOStock(fmt);
            int inventoryQty = iostock.Inventory.Quantity;
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(new IOStockStatusViewModel());
            viewmodel.StockType = IOStockType.OUTGOING;
            viewmodel.Product = iostock.Inventory.Product;
            viewmodel.Inventory = viewmodel.InventoryList.Where(x => x.ID == iostock.Inventory.ID).Single();
            viewmodel.Date = fmt.Date.AddDays(-10000);
            viewmodel.Quantity = 1;
            await viewmodel.RecordAsync();
            await CheckQuantity(inventory.ID);
        }

        public async Task CheckQuantity(string inventoryID)
        {
            var formats = await DbAdapter.GetInstance().QueryAsync<IOStockFormat>(
                DbCommand.WHERE, "InventoryID", inventoryID, DbCommand.ASCENDING, "Date");
            IOStockFormat near = null;
            foreach (var fmt in formats)
            {
                int remainQty = 0;
                int iosQty = fmt.Quantity;
                if (fmt.StockType == IOStockType.OUTGOING)
                    iosQty = -iosQty;
                if (near != null)
                    remainQty = near.RemainingQuantity;
                int exp = remainQty + iosQty;
                Assert.AreEqual(fmt.RemainingQuantity, exp);
                near = fmt;
            }
            var last = formats.Last();
            var stock = new ObservableIOStock(last);
            Assert.AreEqual(stock.RemainingQuantity, stock.Inventory.Quantity);
        }

        /// <summary>
        /// 수정만 할 뿐 저장을 하지 않을 때 수정된 부분은 원본에 영향을 끼쳐서는 안된다.
        /// </summary>
        [TestMethod]
        public void DontSave()
        {
            new Dummy().Create();
            IOStockFormat fmt = null;
            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadAll<IOStockFormat>().Random();
            var id = fmt.ID;
            var viewmodel = new IOStockDataAmenderViewModel(new ObservableIOStock(fmt));

            var specMemo = viewmodel.SpecificationMemo = "SOME SPECIFICATION MEMO";
            var maker = viewmodel.Maker = viewmodel.MakerList.Where(x => x != viewmodel.Maker).Random();
            var measure = viewmodel.Measure = viewmodel.MeasureList.Where(x => x != viewmodel.Measure).Random();
            var warehouse = viewmodel.Warehouse = viewmodel.WarehouseList.Where(x => x != viewmodel.Warehouse).Random();
            var employee = viewmodel.Employee = viewmodel.EmployeeList.Where(x => x != viewmodel.Employee).Random();
            var memo = viewmodel.Memo = "MEMO";
            var price = viewmodel.UnitPrice = 7777;

            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadByKey<IOStockFormat>(id);

            var obIOStock = new ObservableIOStock(fmt);
            Assert.AreNotEqual(specMemo, obIOStock.Inventory.Memo);
            Assert.AreNotEqual(maker, obIOStock.Inventory.Maker);
            Assert.AreNotEqual(measure, obIOStock.Inventory.Measure);
            Assert.AreNotEqual(warehouse, obIOStock.Warehouse);
            Assert.AreNotEqual(employee, obIOStock.Employee);
            Assert.AreNotEqual(memo, obIOStock.Memo);
        }

        /// <summary>
        /// 라디오버튼에서 입고와 출고를 번읍해 선택할 경우의 이벤트를 체크한다.
        /// </summary>
        [TestMethod]
        public void CheckStackTypePropertyAction()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            //입고시 제품과 규격을 설정한다.
            viewmodel.StockType = IOStockType.INCOMING;
            viewmodel.ProductText = "some product";
            viewmodel.SpecificationText = "some specification name";
            //출고로 바꿀 경우
            viewmodel.StockType = IOStockType.OUTGOING;
            //설정된 프로퍼티를 null로 초기화한다.
            Assert.IsNull(viewmodel.ProductText);
            Assert.IsNull(viewmodel.SpecificationText);
            Assert.IsNull(viewmodel.Product);
            Assert.IsNull(viewmodel.Inventory);
            //출고에서 제품과 규격을 선택한 후
            var product = viewmodel.Product = InventoryDataCommander.GetInstance().CopyObservableFields<Product>().Random();
            var inven = viewmodel.Inventory = viewmodel.InventoryList.Random();
            //입고로 전환해도 제품과 규격 프로퍼티는 그대로 있고
            viewmodel.StockType = IOStockType.INCOMING;
            //다시 출고로 바뀌어도 마찬가지이다.
            viewmodel.StockType = IOStockType.OUTGOING;
            //결과
            Assert.IsNotNull(viewmodel.Product);
            Assert.IsNotNull(viewmodel.ProductText);
            Assert.IsNotNull(viewmodel.Inventory);
        }

        /// <summary>
        /// 제품을 하나 선택하고 규격도 선택 한 뒤에 다시 제품의 이름을 직접 써넣고 규격도 써넣음
        /// 그 상태에서 그대로 저장하고 그 후 제대로 저장되었는지 확인
        /// </summary>
        [TestMethod]
        public async Task LogicPatten00()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();

            var product = viewmodel.Product = InventoryDataCommander.GetInstance().CopyObservableFields<Product>().Random();
            var specificationName = viewmodel.Inventory = viewmodel.InventoryList.Random();
            var productText = viewmodel.ProductText = "new product text";
            var specificationText = viewmodel.SpecificationText = "new specification text";
            Assert.IsNull(viewmodel.InventoryList);
            var stock = await viewmodel.RecordAsync();
            Assert.AreEqual(productText, stock.Inventory.Product.Name);
            Assert.AreEqual(specificationText, stock.Inventory.Specification);
        }

        /// <summary>
        /// 기존의 규격을 선택하였을 떄 규격과 관련된 (제조사, 단위, 규격의 메모 등) 프로퍼티가 셋팅되어야 한다.
        /// </summary>
        [TestMethod]
        public void WhenSelectSpecificationThenInventoryPropertiesInitialize()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();

            var product = viewmodel.Product = InventoryDataCommander.GetInstance().CopyObservableFields<Product>().Random();
            var inventory = viewmodel.Inventory = viewmodel.InventoryList.Random();

            Assert.AreEqual(viewmodel.Inventory.Memo, viewmodel.SpecificationMemo);
            Assert.AreEqual(viewmodel.Inventory.Maker, viewmodel.Maker);
            Assert.AreEqual(viewmodel.Inventory.Measure, viewmodel.Measure);
        }

        /// <summary>
        /// 기존의 규격을 선택하였을 때 규격 관련 프로퍼티가 셋팅된다. 이 후 기존의 제품이 아닌 새로운 제품을 사용하고자 할 때
        /// (유저가 제품 이름을 새로 입력하였을 때)
        /// Inventory와 관련된 모든 프로퍼티를 null로 초기화한다.
        /// 이 테스는 입고일 때만 가능하다. (출고는 기존의 제품과 규격만을 사용해야 하니까)
        /// </summary>
        [TestMethod]
        public void WhenCreateNewProductThenPropertiesOfInventoryBecomeNull()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            //기존의 제품과 규격을 선택
            viewmodel.StockType = IOStockType.INCOMING;
            var product = viewmodel.Product = InventoryDataCommander.GetInstance().CopyObservableFields<Product>().Random();
            var inventory = viewmodel.Inventory = viewmodel.InventoryList.Random();
            inventory.Memo = "some specification memo";
            viewmodel.MeasureText = "some measure";
            viewmodel.MakerText = "some maker";
            //규격의 관련 속성이 로드됨을 확인
            Assert.IsNotNull(viewmodel.ProductText);
            Assert.IsNotNull(viewmodel.SpecificationMemo);
            Assert.IsNotNull(viewmodel.Maker);
            Assert.IsNotNull(viewmodel.Measure);
            //새로운 제품을 쓰고자 하면
            viewmodel.ProductText = "some product text";
            //관련 규격 데이터들을 널로 초기화 되었는지 확인
            Assert.IsNull(viewmodel.Project);
            Assert.IsNull(viewmodel.Inventory);
            Assert.IsNull(viewmodel.SpecificationMemo);
            Assert.IsNull(viewmodel.Maker);
            Assert.IsNull(viewmodel.Measure);
            Assert.IsNull(viewmodel.MakerText);
            Assert.IsNull(viewmodel.MeasureText);
        }

        /// <summary>
        /// 입고와 출고 라디오 버튼의 입력에 따라 일부 프로퍼티의 IsEnabled 속성이 변한다.
        /// </summary>
        [TestMethod]
        public void BoundBooleanPropertiesLockOnOff()
        {
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.StockType = IOStockType.OUTGOING;
            //제품이름 변경 금지
            Assert.IsTrue(viewmodel.IsReadOnlyProductTextBox);
            //보관장소 금지
            Assert.IsFalse(viewmodel.IsEnabledWarehouseComboBox);
            //프로젝트는 열려야함
            Assert.IsTrue(viewmodel.IsEnabledProjectComboBox);
            viewmodel.StockType = IOStockType.INCOMING;
            Assert.IsFalse(viewmodel.IsReadOnlyProductTextBox);
            Assert.IsTrue(viewmodel.IsEnabledWarehouseComboBox);
            Assert.IsFalse(viewmodel.IsEnabledProjectComboBox);
        }

        /// <summary>
        /// InventoryList의 자식으로 Inventory에 할당할 때 복사생성이 발생하는 이슈
        /// </summary>
        [TestMethod]
        public void WhenModifyThenInventoryIsChildOfInventoryList()
        {
            new Dummy().Create();
            IOStockFormat fmt = null;
            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadAll<IOStockFormat>().Random();
            IOStockType type = fmt.StockType;
            var iso = new ObservableIOStock(fmt);
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(new ObservableIOStock(fmt));

            Assert.AreEqual(iso.Inventory.Product, viewmodel.Product);
            Assert.IsNotNull(viewmodel.Inventory);
            Assert.IsNotNull(viewmodel.InventoryList);
            Assert.IsTrue(viewmodel.Inventory.ID == iso.Inventory.ID);
            Assert.IsTrue(viewmodel.InventoryList.Any(x => x.ID == viewmodel.Inventory.ID));
            Assert.IsTrue(viewmodel.InventoryList.Contains(viewmodel.Inventory)); //여기서 문제남
        }

        /// <summary>
        /// 입출고 데이터를 수정할 경우 프로퍼티가 정상적으로 대입되었는지 확인한다.
        /// </summary>
        [TestMethod]
        public void CheckInitializedProperties()
        {
            new Dummy().Create();
            IOStockFormat fmt = null;
            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadAll<IOStockFormat>().Where(x => x.StockType == IOStockType.INCOMING).Random();
            ObservableIOStock obIOStock = new ObservableIOStock(fmt);
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(obIOStock);

            Assert.AreEqual(obIOStock.Date, viewmodel.Date);
            Assert.AreEqual(obIOStock.StockType, viewmodel.StockType);
            Assert.AreEqual(obIOStock.Employee, viewmodel.Employee);
            Assert.AreEqual(obIOStock.Inventory.ID, viewmodel.Inventory.ID);
            Assert.AreEqual(obIOStock.Inventory.Product, viewmodel.Product);
            Assert.AreEqual(obIOStock.Inventory.Measure, viewmodel.Measure);
            Assert.AreEqual(obIOStock.Inventory.Maker, viewmodel.Maker);
            Assert.AreEqual(obIOStock.Warehouse, viewmodel.Warehouse);
            Assert.AreEqual(obIOStock.Supplier, viewmodel.Client);
            Assert.AreEqual(obIOStock.Quantity, viewmodel.Quantity);
            Assert.AreEqual(obIOStock.RemainingQuantity, viewmodel.RemainingQuantity);
            Assert.AreEqual(obIOStock.UnitPrice, viewmodel.UnitPrice);
        }

    
    }
}