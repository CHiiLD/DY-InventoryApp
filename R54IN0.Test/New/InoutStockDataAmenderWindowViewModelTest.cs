using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System.Linq;

namespace R54IN0.Test.New
{
    [TestClass]
    public class InoutStockDataAmenderWindowViewModelTest
    {
        [TestMethod]
        public void CanCreate()
        {
            new InoutStockDataAmenderWindowViewModel();
        }

        /// <summary>
        /// 새로운 제품과 규격을 설정하여 추가한다.
        /// </summary>
        [TestMethod]
        public void Add()
        {
            new Dummy2().Create();
            var viewmodel = new InoutStockDataAmenderWindowViewModel();
            var inventoryViewModel = new InventoryStatusViewModel();
            var inoutViewModel = new InoutStockStatusViewModel();
            //날짜를 선택
            inoutViewModel.SelectedGroupItem = InoutStockStatusViewModel.GROUPITEM_DATE;
            inoutViewModel.DatePickerViewModel.TodayCommand.Execute(null); //올해 버튼을 클릭

            Assert.IsFalse(viewmodel.IsReadOnlyProductText); //제품 이름 변경 가능
            Assert.IsTrue(viewmodel.ProductSearchCommand.CanExecute(null)); //제품 선택 가능

            //새로운 제품의 새로운 규격 .. 모든 것을 새롭게!
            viewmodel.IsCheckedInComing = true;
            viewmodel.ProductText = "제품이름 아무거나";
            Assert.IsTrue(viewmodel.Product == null);

            var spec = viewmodel.SpecificationText = "규격1";
            var specMemo = viewmodel.SpecificationMemo = "specification meno";
            var maker = viewmodel.MakerText = "maker text";
            var measure = viewmodel.MeasureText = "measure text";
            var client = viewmodel.ClientText = "거래처 텍스트";
            var warehouse = viewmodel.WarehouseText = "연규실";
            var project = viewmodel.ProjectText = "dy 123123123";
            var employee = viewmodel.EmployeeText = "홍길동";
            var memo = viewmodel.Memo = "??";
            var qty = viewmodel.Quantity = 123;
            var price = viewmodel.UnitPrice = 1000;
            //저장
            var iostock = viewmodel.Record();
            Assert.IsTrue(iostock.StockType == StockType.INCOMING);
            Assert.IsTrue(iostock.Supplier != null);
            Assert.IsTrue(iostock.Customer == null);
            //입출고 데이터그리드에 추가되었는지 확인
            Assert.IsTrue(inoutViewModel.DataGridViewModel.Items.Any(x => x.ID == iostock.ID));
            var inoutStock = inoutViewModel.DataGridViewModel.Items.Where(x => x.ID == iostock.ID).Single();
            //재고 데이터그리드에 추가되었는지 확인
            Assert.IsTrue(inventoryViewModel.GetDataGridItems().Any(x => x.ID == iostock.Inventory.ID));
            var inventory = inventoryViewModel.GetDataGridItems().Where(x => x.ID == iostock.Inventory.ID).Single();
            var oid = ObservableInvenDirector.GetInstance();
            Assert.IsNotNull(oid.Search(iostock.Inventory.ID));

            Assert.AreEqual(spec, iostock.Inventory.Specification);
            Assert.AreEqual(specMemo, iostock.Inventory.Memo);
            Assert.AreEqual(maker, iostock.Inventory.Maker.Name);
            Assert.AreEqual(measure, iostock.Inventory.Measure.Name);
            Assert.AreEqual(client, iostock.Supplier.Name);
            Assert.AreEqual(warehouse, iostock.Warehouse.Name);
            Assert.AreEqual(project, iostock.Project.Name);
            Assert.AreEqual(employee, iostock.Employee.Name);
            Assert.AreEqual(qty, iostock.Quantity);
            Assert.AreEqual(memo, iostock.Memo);
            Assert.AreEqual(price, iostock.UnitPrice);

            Assert.AreEqual(qty, iostock.RemainingQuantity);
            Assert.AreEqual(qty, iostock.Inventory.Quantity);
        }

        /// <summary>
        /// 기존의 제품의 규격을 사용하여 입고 기록을 만든다.
        /// </summary>
        [TestMethod]
        public void Add2()
        {
            new Dummy2().Create();
            var viewmodel = new InoutStockDataAmenderWindowViewModel();

            var product = viewmodel.Product = ObservableFieldDirector.GetInstance().CreateList<Product>().Random();
            var inven = viewmodel.Inventory = viewmodel.InventoryList.Random();
            var specMemo = viewmodel.SpecificationMemo = "123";
            var maker = viewmodel.Maker = viewmodel.MakerList.Random();
            var measure = viewmodel.Measure = viewmodel.MeasureList.Random();
            var client = viewmodel.Client = viewmodel.ClientList.Random();
            var warehouse = viewmodel.Warehouse = viewmodel.WarehouseList.Random();
            var employee = viewmodel.Employee = viewmodel.EmployeeList.Random();
            var memo = viewmodel.Memo = "ㅎ로로롤루루루루루루루";
            viewmodel.UnitPrice = 101010100;
            var stock = viewmodel.Record();

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
        public void Add3()
        {
            new Dummy2().Create();
            var viewmodel = new InoutStockDataAmenderWindowViewModel();

            var product = viewmodel.Product = ObservableFieldDirector.GetInstance().CreateList<Product>().Random();
            var specificationName = viewmodel.SpecificationText = "새로운 규격찡";
            var specMemo = viewmodel.SpecificationMemo = "123";
            var maker = viewmodel.Maker = viewmodel.MakerList.Random();
            var measure = viewmodel.Measure = viewmodel.MeasureList.Random();
            var client = viewmodel.Client = viewmodel.ClientList.Random();
            var warehouse = viewmodel.Warehouse = viewmodel.WarehouseList.Random();
            var employee = viewmodel.Employee = viewmodel.EmployeeList.Random();
            var memo = viewmodel.Memo = "ㅎ로로롤루루루루루루루";
            viewmodel.UnitPrice = 101010100;
            var stock = viewmodel.Record();

            Assert.AreEqual(product, stock.Inventory.Product);
            Assert.AreEqual(specificationName, stock.Inventory.Specification);
            Assert.AreEqual(specMemo, stock.Inventory.Memo);
            Assert.AreEqual(maker, stock.Inventory.Maker);
            Assert.AreEqual(measure, stock.Inventory.Measure);
            Assert.AreEqual(warehouse, stock.Warehouse);
            Assert.AreEqual(employee, stock.Employee);
            Assert.AreEqual(memo, stock.Memo);
        }
        /// <summary>
        /// 입고에서 출고로 바꿀 경우 ..
        /// 기존에 있던 데이터들을 경우에 따라 유지하기도 하고 없애기도 한다.
        /// </summary>
        [TestMethod]
        public void ChangeStockType()
        {
            new Dummy2().Create();
            var viewmodel = new InoutStockDataAmenderWindowViewModel();
            //입고
            viewmodel.IsCheckedInComing = true;
            viewmodel.IsCheckedOutGoing = false;
            viewmodel.ProductText = "asdf";
            viewmodel.SpecificationText = "asdfasdf";
            //출고
            viewmodel.IsCheckedInComing = false;
            viewmodel.IsCheckedOutGoing = true;

            Assert.IsNull(viewmodel.ProductText);
            Assert.IsNull(viewmodel.SpecificationText);
            Assert.IsNull(viewmodel.Product);
            Assert.IsNull(viewmodel.Inventory);

            var product = viewmodel.Product = ObservableFieldDirector.GetInstance().CreateList<Product>().Random();
            var inven = viewmodel.Inventory = viewmodel.InventoryList.Random();

            //입고
            viewmodel.IsCheckedInComing = true;
            viewmodel.IsCheckedOutGoing = false;
            //출고
            viewmodel.IsCheckedInComing = false;
            viewmodel.IsCheckedOutGoing = true;

            Assert.IsNotNull(viewmodel.Product);
            Assert.IsNotNull(viewmodel.ProductText);
            Assert.IsNotNull(viewmodel.Inventory);
        }

        /// <summary>
        /// 제품을 하나 선택하고 규격도 선택 한 뒤에 다시 제품의 이름을 직접 써넣고 규격도 써넣음
        /// 그 상태에서 그대로 저장하고 그 후 제대로 저장되었는지 확인
        /// </summary>
        [TestMethod]
        public void Patten1()
        {
            new Dummy2().Create();
            var viewmodel = new InoutStockDataAmenderWindowViewModel();

            var product = viewmodel.Product = ObservableFieldDirector.GetInstance().CreateList<Product>().Random();
            var specificationName = viewmodel.Inventory = viewmodel.InventoryList.Random();

            var productText = viewmodel.ProductText = "새로운 제품";
            var specificationText = viewmodel.SpecificationText = "새로운 규격";

            Assert.IsNull(viewmodel.InventoryList);

            var stock = viewmodel.Record();

            Assert.AreEqual(productText, stock.Inventory.Product.Name);
            Assert.AreEqual(specificationText, stock.Inventory.Specification);
        }

        [TestMethod]
        public void WhenSelectSpecificationThenInventoryPropertiesInitialize()
        {
            new Dummy2().Create();
            var viewmodel = new InoutStockDataAmenderWindowViewModel();

            var product = viewmodel.Product = ObservableFieldDirector.GetInstance().CreateList<Product>().Random();
            var inventory = viewmodel.Inventory = viewmodel.InventoryList.Random();

            Assert.AreEqual(viewmodel.Inventory.Memo, viewmodel.SpecificationMemo);
            Assert.AreEqual(viewmodel.Inventory.Maker, viewmodel.Maker);
            Assert.AreEqual(viewmodel.Inventory.Measure, viewmodel.Measure);
        }

        [TestMethod]
        public void WhenIncomingAndUseNewProductThenSettedProeprtiesBecomeNull()
        {
            new Dummy2().Create();
            var viewmodel = new InoutStockDataAmenderWindowViewModel();
            //기존의 제품과 규격을 선택
            viewmodel.IsCheckedInComing = true;
            var product = viewmodel.Product = ObservableFieldDirector.GetInstance().CreateList<Product>().Random();
            var inventory = viewmodel.Inventory = viewmodel.InventoryList.Random();
            inventory.Memo = "123";
            viewmodel.MeasureText = "111";
            viewmodel.MakerText = "11111";
            //관련 속성이 로드됨을 확인
            Assert.IsNotNull(viewmodel.ProductText);
            Assert.IsNotNull(viewmodel.SpecificationMemo);
            Assert.IsNotNull(viewmodel.Maker);
            Assert.IsNotNull(viewmodel.Measure);
            //제품을 수정
            viewmodel.ProductText = "아무거나";
            //관련 규격 데이터들을 Null로
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
        public void LockAnUnLock()
        {
            var viewmodel = new InoutStockDataAmenderWindowViewModel();
            viewmodel.IsCheckedInComing = false;
            //출고일 때
            viewmodel.IsCheckedOutGoing = true;
            //제품이름 변경 금지
            Assert.IsTrue(viewmodel.IsReadOnlyProductText);
            //보관장소 금지
            Assert.IsFalse(viewmodel.IsEnabledWarehouseComboBox);
            //프로젝트는 열려야함
            Assert.IsTrue(viewmodel.IsEnabledProjectComboBox);

            viewmodel.IsCheckedOutGoing = false;
            //입고일 때
            viewmodel.IsCheckedInComing = true;
            Assert.IsFalse(viewmodel.IsReadOnlyProductText);
            Assert.IsTrue(viewmodel.IsEnabledWarehouseComboBox);
            Assert.IsFalse(viewmodel.IsEnabledProjectComboBox);
        }

        [TestMethod]
        public void CanModify()
        {
            new Dummy2().Create();
            InoutStockFormat fmt = null;
            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadAll<InoutStockFormat>().Random();
            var viewmodel = new InoutStockDataAmenderWindowViewModel(new ObservableInoutStock(fmt));
            Assert.IsFalse(viewmodel.IsEnabledProductText);
            Assert.IsFalse(viewmodel.IsEnabledSpecification);
            viewmodel = new InoutStockDataAmenderWindowViewModel();
            Assert.IsTrue(viewmodel.IsEnabledProductText);
            Assert.IsTrue(viewmodel.IsEnabledSpecification);
        }

        [TestMethod]
        public void Modify()
        {
            new Dummy2().Create();
            InoutStockFormat fmt = null;
            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadAll<InoutStockFormat>().Random();
            var type = fmt.StockType;
            var viewmodel = new InoutStockDataAmenderWindowViewModel(new ObservableInoutStock(fmt));

            Assert.AreEqual(type, viewmodel.StockType);

            string id = fmt.ID;

            var clientText = viewmodel.ClientText = "ctd_d";
            viewmodel.Client = null;

            var warehouseText = viewmodel.WarehouseText = "wd_dh";
            viewmodel.Warehouse = null;

            var employeeText = viewmodel.EmployeeText = "d_demp";
            viewmodel.Employee = null;

            var proejctText = viewmodel.ProjectText = "neprod_d";
            viewmodel.Project = null;

            var memo = viewmodel.Memo = "ddd';";

            var specificationMemo = viewmodel.SpecificationMemo = "332_aa";
            var makerText = viewmodel.MakerText = "dd_d12";
            viewmodel.Maker = null;

            var measureText = viewmodel.MeasureText = "12;4MM";
            viewmodel.Measure = null;

            var qty = viewmodel.Quantity = 1111;
            var price = viewmodel.UnitPrice = 30302;

            viewmodel.Record();

            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadByKey<InoutStockFormat>(fmt.ID);
            var stock = new ObservableInoutStock(fmt);

            Assert.AreEqual(id, stock.ID);
            Assert.AreEqual(clientText, stock.StockType == StockType.INCOMING ? stock.Supplier.Name : stock.Customer.Name);
            Assert.AreEqual(warehouseText, stock.Warehouse.Name);
            Assert.AreEqual(employeeText, stock.Employee.Name);
            Assert.AreEqual(proejctText, stock.Project.Name);
            Assert.AreEqual(memo, stock.Memo);
            Assert.AreEqual(makerText, stock.Inventory.Maker.Name);
            Assert.AreEqual(measureText, stock.Inventory.Measure.Name);
            Assert.AreEqual(type, stock.StockType);
        }

        [TestMethod]
        public void Modify2()
        {
            new Dummy2().Create();
            InoutStockFormat fmt = null;
            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadAll<InoutStockFormat>().Random();
            var viewmodel = new InoutStockDataAmenderWindowViewModel(new ObservableInoutStock(fmt));

            var specMemo = viewmodel.SpecificationMemo = "-_---1-_---_";
            var maker = viewmodel.Maker = viewmodel.MakerList.Random();
            var measure = viewmodel.Measure = viewmodel.MeasureList.Random();
            var client = viewmodel.Client = viewmodel.ClientList.Random();
            var warehouse = viewmodel.Warehouse = viewmodel.WarehouseList.Random();
            var employee = viewmodel.Employee = viewmodel.EmployeeList.Random();
            var memo = viewmodel.Memo = "ㅎ로로롤루루루루루루루";
            var price = viewmodel.UnitPrice = 101010100;
            var stock = viewmodel.Record();

            Assert.AreEqual(specMemo, stock.Inventory.Memo);
            Assert.AreEqual(maker, stock.Inventory.Maker);
            Assert.AreEqual(measure, stock.Inventory.Measure);
            Assert.AreEqual(warehouse, stock.Warehouse);
            Assert.AreEqual(employee, stock.Employee);
            Assert.AreEqual(memo, stock.Memo);
            Assert.AreEqual(price, stock.UnitPrice);
        }

        /// <summary>
        /// 출고 시, 남은 잔여 수량과 재고 수량이 제대로 적용됨을 확인한다.
        /// </summary>
        [TestMethod]
        public void Quantity()
        {
            new Dummy2().Create();
            var viewmodel = new InoutStockDataAmenderWindowViewModel();
            //출고
            viewmodel.IsCheckedInComing = false;
            viewmodel.IsCheckedOutGoing = true;

            var product = viewmodel.Product = ObservableFieldDirector.GetInstance().CreateList<Product>().Random();
            var specificationName = viewmodel.Inventory = viewmodel.InventoryList.Random();

            var inventoryQty = viewmodel.Inventory.Quantity;
            if (inventoryQty <= 1)
                return;

            viewmodel.Quantity = inventoryQty - 1;
            viewmodel.Memo = "잔여수량 테스트하기";

            Assert.AreEqual(1, viewmodel.RemainingQuantity);
            Assert.AreEqual(1, viewmodel.InventoryQuantity);
        }

        /// <summary>
        /// 기존의 입출고 데이터의 입출고 수량을 수정하고 이에 따른 수량 데이터들의 변화가 올바르게 적용됨을 확인한다.
        /// </summary>
        [TestMethod]
        public void Quantity2()
        {
            new Dummy2().Create();
            InoutStockFormat fmt = null;
            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadAll<InoutStockFormat>().Where(x=> x.StockType == StockType.INCOMING).OrderBy(x => x.Date).Last();

            //입고
            fmt.Quantity = 100; //들어간 양
            fmt.RemainingQuantity = 200; //잔여 재고
            var obIOStock = new ObservableInoutStock(fmt);
            obIOStock.Inventory.Quantity = 500; //현재 재고 

            var viewmodel = new InoutStockDataAmenderWindowViewModel(obIOStock);

            viewmodel.IsCheckedInComing = true;
            viewmodel.IsCheckedOutGoing = false;

            viewmodel.Quantity = 99; //입고수량 하나 줄임
            Assert.AreEqual(199, viewmodel.RemainingQuantity); //따라서 잔여 재고가 하나 줄어든다.
            Assert.AreEqual(499, viewmodel.InventoryQuantity); //따라서 현재 재고도 하나 줄어든다.

            viewmodel.Quantity = 101; 
            Assert.AreEqual(201, viewmodel.RemainingQuantity); 
            Assert.AreEqual(501, viewmodel.InventoryQuantity);
            
            //출고
            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadAll<InoutStockFormat>().Where(x => x.StockType == StockType.OUTGOING).OrderBy(x => x.Date).Last();
            fmt.Quantity = 100; //들어간 양
            fmt.RemainingQuantity = 200; //잔여 재고
            obIOStock = new ObservableInoutStock(fmt);
            obIOStock.Inventory.Quantity = 500; //현재 재고 

            viewmodel = new InoutStockDataAmenderWindowViewModel(obIOStock);

            viewmodel.IsCheckedInComing = false;
            viewmodel.IsCheckedOutGoing = true;

            viewmodel.Quantity = 99; //출고수량 하나 줄임
            Assert.AreEqual(201, viewmodel.RemainingQuantity); //따라서 잔여 재고가 하나 늘어난다.
            Assert.AreEqual(501, viewmodel.InventoryQuantity); //따라서 현재 재고도 하나 늘어난다.

            viewmodel.Quantity = 101;
            Assert.AreEqual(199, viewmodel.RemainingQuantity);
            Assert.AreEqual(499, viewmodel.InventoryQuantity);
        }
    }
}