using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System.Linq;
using System.Windows.Controls;

namespace R54IN0.Test
{
    [TestClass]
    public class IOStockManagerViewModelUnitTest
    {
        [TestMethod]
        public void CanCreate0()
        {
            new Dummy().Create();
            var prod = DataDirector.GetInstance().CopyFields<Product>().Random();
            new IOStockManagerViewModel(prod);
        }

        [TestMethod]
        public void CanCreate1()
        {
            new Dummy().Create();
            var inv = DataDirector.GetInstance().CopyInventories().Random();
            new IOStockManagerViewModel(inv);
        }

        [TestMethod]
        public void CanCreate2()
        {
            new Dummy().Create();
            var inv = DataDirector.GetInstance().CopyInventories().Random();
            var qret = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' limit 1;", inv.ID);
            var ios = new ObservableIOStock(qret.Random());
            new IOStockManagerViewModel(ios);
        }

        [TestMethod]
        public void TestInit0()
        {
            new Dummy().Create();
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
            new Dummy().Create();
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
            new Dummy().Create();
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
            new Dummy().Create();
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
            new Dummy().Create();
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
            new Dummy().Create();
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
            new Dummy().Create();
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
            new Dummy().Create();
            var prod = DataDirector.GetInstance().CopyFields<Product>().Random();
            var vm = new IOStockManagerViewModel(prod);

            var inv = vm.SelectedInventory = vm.Inventories.Random();
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
            new Dummy().Create();
            var prod = DataDirector.GetInstance().CopyFields<Product>().Random();
            var vm = new IOStockManagerViewModel(prod);

            vm.StockType = IOStockType.OUTGOING;
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
            new Dummy().Create();
            var prod = DataDirector.GetInstance().CopyFields<Product>().Random();
            var vm = new IOStockManagerViewModel(prod);

            vm.StockType = IOStockType.OUTGOING;
            var inv = vm.SelectedInventory = vm.Inventories.Random();
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
            new Dummy().Create();
            var inv = DataDirector.GetInstance().CopyInventories().Random();
            var qret = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' and StockType = 1 limit 1;", inv.ID);
            var oios = new ObservableIOStock(qret.Random());
            var vm = new IOStockManagerViewModel(oios);

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
            new Dummy().Create();
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
            new Dummy().Create();
            var inv = DataDirector.GetInstance().CopyInventories().Random();
            var qret = DataDirector.GetInstance().DB.Query<IOStockFormat>("select * from IOStockFormat where InventoryID = '{0}' and StockType = 2 limit 1;", inv.ID);
            var oios = new ObservableIOStock(qret.Random());
            var vm = new IOStockManagerViewModel(oios);

            var acc = vm.SelectedAccount = vm.Accounts.Random();
            var emp =  vm.SelectedEmployee = vm.Employees.Random();
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
            new Dummy().Create();
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
    }
}