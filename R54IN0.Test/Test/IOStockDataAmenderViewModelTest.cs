﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System;
using System.Collections.Generic;
using System.Linq;

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
            new IOStockDataAmenderViewModel();
            new Dummy().Create();
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
        public void Add()
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
            //저장
            var iostock = viewmodel.Record();
            Assert.IsTrue(iostock.StockType == IOStockType.INCOMING);
            Assert.IsTrue(iostock.Supplier != null);
            Assert.IsTrue(iostock.Customer == null);
            //입출고 데이터그리드에 추가되었는지 확인
            Assert.IsTrue(iostockStatusViewModel.DataGridViewModel.Items.Any(x => x.ID == iostock.ID));
            var inoutStock = iostockStatusViewModel.DataGridViewModel.Items.Where(x => x.ID == iostock.ID).Single();
            //재고 데이터그리드에 추가되었는지 확인
            Assert.IsTrue(inventoryStatusViewModel.GetDataGridItems().Any(x => x.ID == iostock.Inventory.ID));
            var inventory = inventoryStatusViewModel.GetDataGridItems().Where(x => x.ID == iostock.Inventory.ID).Single();
            var oid = ObservableInventoryDirector.GetInstance();
            //검사
            Assert.IsNotNull(oid.Search(iostock.Inventory.ID)); //inventory director에 추가되었는지 확인한다.
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
            var ofd = ObservableFieldDirector.GetInstance();
            Assert.IsNotNull(ofd.Search<Maker>(iostock.Inventory.Maker.ID));
            Assert.IsNotNull(ofd.Search<Measure>(iostock.Inventory.Measure.ID));
        }

        /// <summary>
        /// 기존의 제품의 새로운 규격을 사용하여 입고 기록을 만든다.
        /// </summary>
        [TestMethod]
        public void Add2()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            //설정
            viewmodel.StockType = IOStockType.INCOMING;
            var product = viewmodel.Product = ObservableFieldDirector.GetInstance().CreateList<Product>().Random();
            var inven = viewmodel.Inventory = viewmodel.InventoryList.Random();
            var specMemo = viewmodel.SpecificationMemo = "some memo";
            var maker = viewmodel.Maker = viewmodel.MakerList.Random();
            var measure = viewmodel.Measure = viewmodel.MeasureList.Random();
            var client = viewmodel.Client = viewmodel.ClientList.Random();
            var warehouse = viewmodel.Warehouse = viewmodel.WarehouseList.Random();
            var employee = viewmodel.Employee = viewmodel.EmployeeList.Random();
            var memo = viewmodel.Memo = "some memo";
            viewmodel.UnitPrice = 6666;
            var stock = viewmodel.Record();
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
        public void Add3()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            //설정
            var product = viewmodel.Product = ObservableFieldDirector.GetInstance().CreateList<Product>().Random();
            var specificationName = viewmodel.SpecificationText = "some specification";
            var specMemo = viewmodel.SpecificationMemo = "some specification memo";
            var maker = viewmodel.Maker = viewmodel.MakerList.Random();
            var measure = viewmodel.Measure = viewmodel.MeasureList.Random();
            var client = viewmodel.Client = viewmodel.ClientList.Random();
            var warehouse = viewmodel.Warehouse = viewmodel.WarehouseList.Random();
            var employee = viewmodel.Employee = viewmodel.EmployeeList.Random();
            var memo = viewmodel.Memo = "some memo";
            viewmodel.UnitPrice = 6666;
            var obIOStock = viewmodel.Record();
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
        public void Modify()
        {
            new Dummy().Create();
            IOStockFormat fmt = null;
            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadAll<IOStockFormat>().Random();
            IOStockType type = fmt.StockType;
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(new ObservableIOStock(fmt));

            Assert.AreEqual(type, viewmodel.StockType);
            string id = fmt.ID;
            var clientText = viewmodel.ClientText = "SOME CLIENT";
            viewmodel.Client = null;
            var warehouseText = viewmodel.WarehouseText = "SOME WAREHOUSE";
            viewmodel.Warehouse = null;
            var employeeText = viewmodel.EmployeeText = "SOME EMPLOYEE";
            viewmodel.Employee = null;
            var proejctText = viewmodel.ProjectText = "SOME PROJECT";
            viewmodel.Project = null;
            var memo = viewmodel.Memo = "SOME MEMO";
            var specificationMemo = viewmodel.SpecificationMemo = "SOME SPEC_MEMO";
            var makerText = viewmodel.MakerText = "SOME MAKER";
            viewmodel.Maker = null;
            var measureText = viewmodel.MeasureText = "SOME MEASURE";
            viewmodel.Measure = null;
            var qty = viewmodel.Quantity = 1111;
            var price = viewmodel.UnitPrice = 30302;

            viewmodel.Record();

            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadByKey<IOStockFormat>(fmt.ID);
            ObservableIOStock obIOStock = new ObservableIOStock(fmt);

            Assert.AreEqual(clientText, obIOStock.StockType == IOStockType.INCOMING ? obIOStock.Supplier.Name : obIOStock.Customer.Name);
            Assert.AreEqual(warehouseText, obIOStock.Warehouse.Name);
            Assert.AreEqual(employeeText, obIOStock.Employee.Name);
            Assert.AreEqual(proejctText, obIOStock.Project.Name);
            Assert.AreEqual(memo, obIOStock.Memo);
            Assert.AreEqual(makerText, obIOStock.Inventory.Maker.Name);
            Assert.AreEqual(measureText, obIOStock.Inventory.Measure.Name);
            Assert.AreEqual(type, obIOStock.StockType);

            var ofd = ObservableFieldDirector.GetInstance();
            Assert.IsNotNull(ofd.Search<Maker>(obIOStock.Inventory.Maker.ID));
            Assert.IsNotNull(ofd.Search<Measure>(obIOStock.Inventory.Measure.ID));
        }

        [TestMethod]
        public void Modify2()
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
        /// 출고 시, 출고된 수량만큼 RemainingQuantity, InventoryQuantity 프로퍼티가
        /// 정상적으로 값을 가감하는지 테스트
        /// </summary>
        [TestMethod]
        public void Quantity()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();

            viewmodel.StockType = IOStockType.OUTGOING;
            var product = viewmodel.Product = ObservableFieldDirector.GetInstance().CreateList<Product>().Random();
            var specificationName = viewmodel.Inventory = viewmodel.InventoryList.Random();
            var inventoryQty = viewmodel.Inventory.Quantity;
            if (inventoryQty <= 1)
                return;
            viewmodel.Quantity = inventoryQty - 1; //현재 재고는 1개

            Assert.AreEqual(1, viewmodel.RemainingQuantity); //따라서 잔여 재고도 1개이고
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
            Assert.AreEqual(199, viewmodel.RemainingQuantity); //따라서 잔여 재고가 하나 줄어든다.
            Assert.AreEqual(499, viewmodel.InventoryQuantity); //따라서 현재 재고도 하나 줄어든다.
            viewmodel.Quantity = 101;
            Assert.AreEqual(201, viewmodel.RemainingQuantity);
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
            Assert.AreEqual(201, viewmodel.RemainingQuantity); //따라서 잔여 재고가 하나 늘어난다.
            Assert.AreEqual(501, viewmodel.InventoryQuantity); //따라서 현재 재고도 하나 늘어난다.
            viewmodel.Quantity = 101;
            Assert.AreEqual(199, viewmodel.RemainingQuantity);
            Assert.AreEqual(499, viewmodel.InventoryQuantity);
        }

        /// <summary>
        /// 과거의 입출고 데이터 중 입고수량 또는 출고 수량을 수정하였다면
        /// 그에 맞게 과거부터 현재까지의 모든 잔여 수량과 재고수량을 수정한다.
        /// </summary>
        [TestMethod]
        public void Quantity3()
        {
            new Dummy().Create();
            IOStockFormat fmt = null;
            using (var db = LexDb.GetDbInstance())
                fmt = db.LoadAll<IOStockFormat>().Random();

            IEnumerable<IOStockFormat> before = null;
            using (var db = LexDb.GetDbInstance())
                before = db.LoadAll<IOStockFormat>().Where(x => x.InventoryID == fmt.InventoryID).Where(x => x.Date > fmt.Date).OrderBy(x => x.Date);

            IObservableIOStockProperties obIOStock = new ObservableIOStock(fmt);
            int addQty = new Random().Next(-10, 10);

            var inventoryQty = obIOStock.Inventory.Quantity;
            var viewmodel = new IOStockDataAmenderViewModel(obIOStock);
            viewmodel.Quantity = viewmodel.Quantity + addQty; //개수 하나를 늘린다.
            obIOStock = viewmodel.Record();

            addQty = obIOStock.StockType == IOStockType.INCOMING ? addQty : -addQty;

            IEnumerable<IOStockFormat> after = null;
            using (var db = LexDb.GetDbInstance())
                after = db.LoadAll<IOStockFormat>().Where(x => x.InventoryID == obIOStock.Inventory.ID).Where(x => x.Date > obIOStock.Date).OrderBy(x => x.Date);

            Assert.AreEqual(before.Count(), after.Count());
            Assert.AreEqual(obIOStock.Inventory.Quantity, inventoryQty + addQty);
            for (int i = 0; i < after.Count(); i++)
            {
                var a = after.ElementAt(i);
                var b = before.ElementAt(i);
                Assert.AreEqual(a.ID, b.ID);
                Assert.AreEqual(a.RemainingQuantity, b.RemainingQuantity + addQty);
            }
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
            var product = viewmodel.Product = ObservableFieldDirector.GetInstance().CreateList<Product>().Random();
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
        public void LogicPatten00()
        {
            new Dummy().Create();
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();

            var product = viewmodel.Product = ObservableFieldDirector.GetInstance().CreateList<Product>().Random();
            var specificationName = viewmodel.Inventory = viewmodel.InventoryList.Random();
            var productText = viewmodel.ProductText = "new product text";
            var specificationText = viewmodel.SpecificationText = "new specification text";
            Assert.IsNull(viewmodel.InventoryList);
            var stock = viewmodel.Record();
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

            var product = viewmodel.Product = ObservableFieldDirector.GetInstance().CreateList<Product>().Random();
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
            var product = viewmodel.Product = ObservableFieldDirector.GetInstance().CreateList<Product>().Random();
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
            var obIOStock = new ObservableIOStock(fmt);
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(new ObservableIOStock(fmt));

            Assert.AreEqual(obIOStock.Inventory.Product, viewmodel.Product);
            Assert.IsNotNull(viewmodel.Inventory);
            Assert.IsNotNull(viewmodel.InventoryList);
            Assert.IsTrue(viewmodel.Inventory.ID == obIOStock.Inventory.ID);
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