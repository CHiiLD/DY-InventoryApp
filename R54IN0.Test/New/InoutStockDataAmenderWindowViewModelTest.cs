using System;
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

            Assert.IsFalse(viewmodel.IsReadOnlyProductName); //제품 이름 변경 가능
            Assert.IsTrue(viewmodel.ProductSearchCommand.CanExecute(null)); //제품 선택 가능

            //새로운 제품의 새로운 규격 .. 모든 것을 새롭게!
            viewmodel.IsCheckedInComing = true;
            viewmodel.ProductText = "제품이름 아무거나";
            Assert.IsTrue(viewmodel.Product == null);

            var spec = viewmodel.SpecificationText = "규격1";
            var specMemo = viewmodel.SpecificationMemo = "specification meno";
            var maker = viewmodel.MakerText = "maker text";
            var measure = viewmodel.MeasureText = "measure text";
            var client =  viewmodel.ClientText = "거래처 텍스트";
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
            Assert.AreEqual(inven, stock.Inventory);
            Assert.AreEqual(specMemo, stock.Inventory.Memo);
            Assert.AreEqual(maker, stock.Inventory.Maker);
            Assert.AreEqual(measure, stock.Inventory.Measure);
            Assert.AreEqual(warehouse, stock.Warehouse);
            Assert.AreEqual(employee, stock.Employee);
            Assert.AreEqual(memo, stock.Memo);
        }

        /// <summary>
        /// 기존의 제품의 규격을 사용하여 입고 기록을 만든다.
        /// TODO :여기 테스트를 해야함
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
            Assert.IsTrue(viewmodel.IsReadOnlyProductName);
            //보관장소 금지 
            Assert.IsFalse(viewmodel.IsEnabledWarehouseComboBox);
            //프로젝트는 열려야함 
            Assert.IsTrue(viewmodel.IsEnabledProjectComboBox);

            viewmodel.IsCheckedOutGoing = false;
            //입고일 때
            viewmodel.IsCheckedInComing = true;
            Assert.IsFalse(viewmodel.IsReadOnlyProductName);
            Assert.IsTrue(viewmodel.IsEnabledWarehouseComboBox);
            Assert.IsFalse(viewmodel.IsEnabledProjectComboBox);
        }
    }
}