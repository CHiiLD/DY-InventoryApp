using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using MySQL.Test;
using R54IN0.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace R54IN0.Test
{
    [TestClass]
    public class IOStockManagerViewModelUnitTest
    {
        private static MySqlConnection _conn;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Console.WriteLine(nameof(ClassInitialize));
            Console.WriteLine(context.TestName);

            _conn = new MySqlConnection(ConnectingString.KEY);
            _conn.Open();

            Dummy dummy = new Dummy(_conn);
            dummy.Create();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Console.WriteLine(nameof(ClassCleanup));
            _conn.Close();
            _conn = null;
        }


        [TestInitialize]
        public void TestInitialize()
        {
            MySqlConnection conn = DataDirector.GetInstance().DB.Connection;
            using (MySqlCommand cmd = new MySqlCommand("begin work;", conn))
                cmd.ExecuteNonQuery();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            MySqlConnection conn = DataDirector.GetInstance().DB.Connection;
            using (MySqlCommand cmd = new MySqlCommand("rollback;", conn))
                cmd.ExecuteNonQuery();

            CollectionViewModelObserverSubject.Destory();
            TreeViewNodeDirector.Destroy(true);
            DataDirector.Destroy();
        }

        [TestMethod]
        public void CanCreate0()
        {
            var prod = DataDirector.GetInstance().CopyFields<Product>().Random();
            new IOStockManagerViewModel(prod);
        }

        [TestMethod]
        public void CanCreate1()
        {
            var inv = DataDirector.GetInstance().CopyInventories().Random();
            new IOStockManagerViewModel(inv);
        }

        [TestMethod]
        public void CanCreate2()
        {
            var inv = DataDirector.GetInstance().CopyInventories().Random();
            var qret = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' limit 1;", inv.ID);
            var ios = new ObservableIOStock(qret.Random());
            new IOStockManagerViewModel(ios);
        }

        [TestMethod]
        public void TestInit0()
        {
            var inv = DataDirector.GetInstance().CopyInventories().Random();
            var qret = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' and StockType = 1 limit 1;", inv.ID);
            var ios = new ObservableIOStock(qret.Random());
            var vm = new IOStockManagerViewModel(ios);

            Assert.AreEqual(vm.SelectedAccount, ios.Supplier);
            Assert.AreEqual(vm.SelectedProject, ios.Warehouse);
            Assert.AreEqual(vm.SelectedEmployee, ios.Employee);
            Assert.AreEqual(vm.Memo, ios.Memo);
            Assert.AreEqual(vm.Quantity, ios.Quantity);
            Assert.AreEqual(vm.SelectedDate, ios.Date);
            Assert.AreEqual(vm.SelectedInventory, ios.Inventory);
            Assert.AreEqual(vm.UnitPrice, ios.UnitPrice);
        }

        [TestMethod]
        public void TestInit1()
        {
            var inv = DataDirector.GetInstance().CopyInventories().Random();
            var qret = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' and StockType = 2 limit 1;", inv.ID);
            var ios = new ObservableIOStock(qret.Random());
            var vm = new IOStockManagerViewModel(ios);

            Assert.AreEqual(vm.SelectedProject, ios.Project);
            Assert.AreEqual(vm.SelectedInventory, ios.Inventory);
            Assert.AreEqual(vm.SelectedAccount, ios.Customer);
            Assert.AreEqual(vm.SelectedProject, ios.Project);
            Assert.AreEqual(vm.SelectedEmployee, ios.Employee);
            Assert.AreEqual(vm.Memo, ios.Memo);
            Assert.AreEqual(vm.Quantity, ios.Quantity);
            Assert.AreEqual(vm.SelectedDate, ios.Date);
            Assert.AreEqual(vm.SelectedInventory, ios.Inventory);
            Assert.AreEqual(vm.UnitPrice, ios.UnitPrice);
        }

        /// <summary>
        /// 입고에서 출고로 변경할 때 프로젝트와 출고처를 선택할 수 있도록 한다.
        /// </summary>
        [TestMethod]
        public void TestIOStockTypeChange0()
        {
            var prod = DataDirector.GetInstance().CopyFields<Product>().Random();
            var vm = new IOStockManagerViewModel(prod);

            vm.StockType = IOStockType.OUTGOING;
            Assert.AreEqual(vm.Accounts.Random().GetType(), typeof(Observable<Customer>));
            Assert.AreEqual(vm.Projects.Random().GetType(), typeof(Observable<Project>));
        }

        /// <summary>
        /// 출고에서 입고로 변경할 때 창고와 입고처를 선택할 수 있게 한다.
        /// </summary>
        [TestMethod]
        public void TestIOStockTypeChange1()
        {
            var prod = DataDirector.GetInstance().CopyFields<Product>().Random();
            var vm = new IOStockManagerViewModel(prod);

            vm.StockType = IOStockType.OUTGOING;
            vm.StockType = IOStockType.INCOMING;
            Assert.AreEqual(vm.Accounts.Random().GetType(), typeof(Observable<Supplier>));
            Assert.AreEqual(vm.Projects.Random().GetType(), typeof(Observable<Warehouse>));
        }

        /// <summary>
        /// 출고로 변경할 때 선택된 창고와 입고처를 null로 대입
        /// </summary>
        [TestMethod]
        public void TestIOStockTypeChange2()
        {
            var inv = DataDirector.GetInstance().CopyInventories().Random();
            var qret = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' and StockType = 1 limit 1;", inv.ID);
            var iios = new ObservableIOStock(qret.Random());
            var vm = new IOStockManagerViewModel(iios);

            vm.StockType = IOStockType.OUTGOING;

            Assert.AreEqual(null, vm.SelectedAccount);
            Assert.AreEqual(null, vm.SelectedProject);
        }

        /// <summary>
        /// 입고로 변경할 때 선택된 프로젝트와 출고처를 null로 대입
        /// </summary>
        [TestMethod]
        public void TestIOStockTypeChange3()
        {
            var inv = DataDirector.GetInstance().CopyInventories().Random();
            var qret = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' and StockType = 2 limit 1;", inv.ID);
            var oios = new ObservableIOStock(qret.Random());
            var vm = new IOStockManagerViewModel(oios);

            vm.StockType = IOStockType.INCOMING;

            Assert.AreEqual(null, vm.SelectedAccount);
            Assert.AreEqual(null, vm.SelectedProject);
        }

        /// <summary>
        /// IOStockFormat 생성
        /// </summary>
        [TestMethod]
        public void CreateNewIOStock0()
        {
            var prod = DataDirector.GetInstance().CopyFields<Product>().Random();
            var vm = new IOStockManagerViewModel(prod);

            var inv = vm.SelectedInventory = vm.Inventories.Random();
            var acc = vm.SelectedAccount = vm.Accounts.Random();
            var eep = vm.SelectedEmployee = vm.Employees.Random();
            var prj = vm.SelectedProject = vm.Projects.Random();
            var mem = vm.Memo = "some memo";

            ObservableIOStock oio = vm.Insert();

            Assert.AreEqual(inv, oio.Inventory);
            Assert.AreEqual(acc, oio.Supplier);
            Assert.AreEqual(eep, oio.Employee);
            Assert.AreEqual(prj, oio.Warehouse);
            Assert.AreEqual(mem, oio.Memo);
        }

        /// <summary>
        /// IOStockFormat 생성
        /// </summary>
        [TestMethod]
        public void CreateNewIOStock1()
        {
            var prod = DataDirector.GetInstance().CopyFields<Product>().Random();
            var vm = new IOStockManagerViewModel(prod);

            var inv = vm.SelectedInventory = vm.Inventories.Random();
            vm.SelectedAccount = null;
            var acc = vm.AccountText = "new";
            var eep = vm.EmployeeText = "new";
            var prj = vm.ProjectText = "new";

            ObservableIOStock oio = vm.Insert();

            Assert.AreEqual(inv, oio.Inventory);
            Assert.AreEqual(acc, oio.Supplier.Name);
            Assert.AreEqual(eep, oio.Employee.Name);
            Assert.AreEqual(prj, oio.Warehouse.Name);
        }

        /// <summary>
        /// IOStockFormat 생성
        /// </summary>
        [TestMethod]
        public void CreateNewIOStock2()
        {
            var prod = DataDirector.GetInstance().CopyFields<Product>().Random();
            var vm = new IOStockManagerViewModel(prod);

            vm.StockType = IOStockType.OUTGOING;
            vm.SelectedAccount = null;
            var inv = vm.SelectedInventory = vm.Inventories.Random();
            var acc = vm.SelectedAccount = vm.Accounts.Random();
            var eep = vm.SelectedEmployee = vm.Employees.Random();
            var prj = vm.SelectedProject = vm.Projects.Random();
            var mem = vm.Memo = "some memo";

            ObservableIOStock oio = vm.Insert();

            Assert.AreEqual(inv, oio.Inventory);
            Assert.AreEqual(acc, oio.Customer);
            Assert.AreEqual(eep, oio.Employee);
            Assert.AreEqual(prj, oio.Project);
            Assert.AreEqual(mem, oio.Memo);
        }

        /// <summary>
        /// IOStockFormat 생성
        /// </summary>
        [TestMethod]
        public void CreateNewIOStock3()
        {
            var prod = DataDirector.GetInstance().CopyFields<Product>().Random();
            var vm = new IOStockManagerViewModel(prod);

            vm.StockType = IOStockType.OUTGOING;
            var inv = vm.SelectedInventory = vm.Inventories.Random();
            vm.SelectedAccount = null;
            var acc = vm.AccountText = "new";
            var eep = vm.EmployeeText = "new";
            var prj = vm.ProjectText = "new";

            ObservableIOStock oio = vm.Insert();

            Assert.AreEqual(inv, oio.Inventory);
            Assert.AreEqual(acc, oio.Customer.Name);
            Assert.AreEqual(eep, oio.Employee.Name);
            Assert.AreEqual(prj, oio.Project.Name);
        }

        /// <summary>
        /// 금액과 수량 변경 확인
        /// </summary>
        [TestMethod]
        public void ModifyIOStockFormat0()
        {
            ObservableInventory inv = DataDirector.GetInstance().CopyInventories().Random();
            List<IOStockFormat> qret = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' and StockType = 1 limit 1;", inv.ID);
            ObservableIOStock oios = new ObservableIOStock(qret.Random());
            IOStockManagerViewModel vm = new IOStockManagerViewModel(oios);

            var qty = vm.Quantity = 10;
            var pri = vm.UnitPrice = 1000;

            ObservableIOStock oio = vm.Update();

            Assert.AreEqual(qty, oio.Quantity);
            Assert.AreEqual(pri, oio.UnitPrice);
        }

        /// <summary>
        /// 구입처, 담당자, 창고 변경하기
        /// </summary>
        [TestMethod]
        public void ModifyIOStockFormat1()
        {
            var inv = DataDirector.GetInstance().CopyInventories().Random();
            var qret = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' and StockType = 1 limit 1;", inv.ID);
            var oios = new ObservableIOStock(qret.Random());
            var vm = new IOStockManagerViewModel(oios);

            vm.SelectedAccount = vm.Accounts.Random();
            vm.SelectedEmployee = vm.Employees.Random();
            vm.SelectedProject = vm.Projects.Random();

            ObservableIOStock oio = vm.Update();

            Assert.AreEqual(vm.SelectedAccount, oio.Supplier);
            Assert.AreEqual(vm.SelectedEmployee, oio.Employee);
            Assert.AreEqual(vm.SelectedProject, oio.Warehouse);
        }

        /// <summary>
        /// 출고처, 담당자, 프로젝트 변경하기
        /// </summary>
        [TestMethod]
        public void ModifyIOStockFormat2()
        {
            var inv = DataDirector.GetInstance().CopyInventories().Random();
            var qret = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' and StockType = 2 limit 1;", inv.ID);
            var oios = new ObservableIOStock(qret.Random());
            var vm = new IOStockManagerViewModel(oios);

            var acc = vm.SelectedAccount = vm.Accounts.Random();
            var emp = vm.SelectedEmployee = vm.Employees.Random();
            var prj = vm.SelectedProject = vm.Projects.Random();

            ObservableIOStock oio = vm.Update();

            Assert.AreEqual(acc, oio.Customer);
            Assert.AreEqual(emp, oio.Employee);
            Assert.AreEqual(prj, oio.Project);
        }

        /// <summary>
        /// 이름 수정하기
        /// </summary>
        [TestMethod]
        public void ModifyIOStockFormat3()
        {
            var inv = DataDirector.GetInstance().CopyInventories().Random();
            var qret = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' and StockType = 2 limit 1;", inv.ID);
            var oios = new ObservableIOStock(qret.Random());
            var vm = new IOStockManagerViewModel(oios);

            string name = "new";
            vm.SelectedAccount = null;
            vm.SelectedProject = null;
            vm.SelectedEmployee = null;
            vm.AccountText = name;
            vm.ProjectText = name;
            vm.EmployeeText = name;

            ObservableIOStock oio = vm.Update();

            Assert.AreEqual(name, oio.Customer.Name);
            Assert.AreEqual(name, oio.Employee.Name);
            Assert.AreEqual(name, oio.Project.Name);
        }

        [TestMethod]
        public void WhenModifyQtyThenSyncDataGridViewItems()
        {
            ObservableInventory inv = DataDirector.GetInstance().CopyInventories().Random();
            IOStockFormat qret = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' and StockType = 2 limit 1;", inv.ID).Single();

            IOStockStatusViewModel svm = new IOStockStatusViewModel();

            svm.SelectedDataGridGroupOption = IOStockStatusViewModel.DATAGRID_OPTION_PRODUCT;
            List<TreeViewNode> nodes = svm.TreeViewViewModel.SearchNodesInRoot(NodeType.INVENTORY);
            TreeViewNode node = nodes.Where(x => x.ObservableObjectID == qret.InventoryID).Single();
            svm.TreeViewViewModel.AddSelectedNodes(node);

            IOStockDataGridItem stock = DataDirector.GetInstance().StockCollection.Where(x => x.ID == qret.ID).Single();

            IOStockManagerViewModel vm = new IOStockManagerViewModel(stock);
            int qty = vm.Quantity = 10;
            vm.Update();

            ObservableCollection<IOStockDataGridItem> items = svm.DataGridViewModel.Items;
            IOStockDataGridItem result = items.Where(x => x.ID == stock.ID).Single();

            Assert.AreEqual(result, stock);
            Assert.AreEqual(qty, result.Quantity);
        }

        /// <summary>
        /// bugfix sql quantity null query 문제
        /// </summary>
        [TestMethod]
        public void CreateInventoryThenCreateStock()
        {
            var prod = DataDirector.GetInstance().CopyFields<Product>().Random();
            InventoryFormat inv = new InventoryFormat();
            inv.ProductID = prod.ID;
            inv.ID = Guid.NewGuid().ToString();

            DataDirector.GetInstance().AddInventory(inv);

            var vm = new IOStockManagerViewModel(prod);
            vm.SelectedInventory = vm.Inventories.Random();
            vm.Quantity = 10;
            vm.UnitPrice = 1000;

            vm.Insert();
        }

        /// <summary>
        /// 입출고 데이터가 하나도 없는 상태에서 UnitPrice를 쿼리 시 적절한 대처를 검사
        /// </summary>
        [TestMethod]
        public void UnitPriceAndAccountQueryTest0()
        {
            DataDirector ddr = DataDirector.GetInstance();
            Observable<Product> prod = ddr.CopyFields<Product>().Random();
            InventoryFormat invf = new InventoryFormat() { ProductID = prod.ID, Specification = "some"};
            
            ddr.AddInventory(invf);

            ObservableInventory inv = DataDirector.GetInstance().SearchInventory(invf.ID);

            IOStockManagerViewModel vm = new IOStockManagerViewModel(inv);
            Assert.AreEqual(0, vm.UnitPrice); //입고 출고에 아무런 데이터가 없어서 자동적으로 0으로 초기화

            decimal price = vm.UnitPrice = 1500;
            vm.Insert(); //입고 넣기

            vm = new IOStockManagerViewModel(inv);
            Assert.AreEqual(price, vm.UnitPrice); //입고에 있는 값 불러옴

            vm.StockType = IOStockType.OUTGOING;
            Assert.AreEqual(price, vm.UnitPrice); //출고지만 자료가 없어 입고 값으로 덮씌움
            price = vm.UnitPrice = 1700;

            vm.Insert(); //출고 넣기

            vm = new IOStockManagerViewModel(inv);
            vm.StockType = IOStockType.OUTGOING;
            Assert.AreEqual(price, vm.UnitPrice); //출고의 UnitPrice 가져옴
            Assert.IsNull(vm.SelectedAccount);
        }

        [TestMethod]
        public void UnitPriceAndAccountQueryTest1()
        {
            DataDirector ddr = DataDirector.GetInstance();
            Observable<Product> prod = ddr.CopyFields<Product>().Random();
            IOStockManagerViewModel vm = new IOStockManagerViewModel(prod);

            Assert.AreEqual(0, vm.UnitPrice);

            vm.SelectedInventory = vm.Inventories.Random();

            Assert.AreNotEqual(0, vm.UnitPrice);
        }

        [TestMethod]
        public void UnitPriceAndAccountQueryTest2()
        {
            ObservableInventory inv = DataDirector.GetInstance().CopyInventories().Random();
            IOStockManagerViewModel vm = new IOStockManagerViewModel(inv);
            decimal price = vm.UnitPrice;
            var supplier = vm.SelectedAccount = vm.Accounts.Random();
            price = vm.UnitPrice = price + 1000;
            vm.Insert();

            vm = new IOStockManagerViewModel(inv);
            Assert.AreEqual(price, vm.UnitPrice);
            Assert.AreEqual(supplier, vm.SelectedAccount);
        }

        [TestMethod]
        public void UnitPriceAndAccountQueryTest3()
        {
            ObservableInventory inv = DataDirector.GetInstance().CopyInventories().Random();
            IOStockManagerViewModel vm = new IOStockManagerViewModel(inv);
            decimal price = vm.UnitPrice;
            vm.StockType = IOStockType.OUTGOING;
            price = vm.UnitPrice = price + 1000;
            var supplier = vm.SelectedAccount = vm.Accounts.Random();
            vm.Insert();

            vm = new IOStockManagerViewModel(inv);
            vm.StockType = IOStockType.OUTGOING;
            Assert.AreEqual(price, vm.UnitPrice);
            Assert.AreEqual(supplier, vm.SelectedAccount);
        }
    }
}