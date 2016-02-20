using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace R54IN0.WPF
{
    public class IOStockManagerViewModel : INotifyPropertyChanged
    {
        #region private var

        private IOStockType _stockType;
        private ObservableCollection<Observable<Supplier>> _suppiers;
        private ObservableCollection<Observable<Warehouse>> _warehouses;
        private ObservableCollection<Observable<Customer>> _customers;
        private ObservableCollection<Observable<Project>> _projects;
        private IEnumerable<IObservableField> _accounts;
        private IEnumerable<IObservableField> _proejects;
        private ObservableIOStock _target;
        private DateTime _selectedDate;
        private ObservableInventory _selectedInventory;
        private int _quantity;
        private decimal _unitPrice;
        private string _accountText;
        private IObservableField _selectedAccount;
        private IObservableField _selectedProject;
        private string _projectText;
        private Observable<Employee> _selectedEmployee;
        private string _employeeText;
        private string _memo;
        private IOStockStatusViewModel _iOStockStatusViewModel;
        private ObservableIOStock iostock;

        private event PropertyChangedEventHandler _propertyChanged;

        #endregion private var

        /// <summary>
        /// 기존의 입출고 데이터를 수정합니다. 이 떄 규격은 변경되지 아니합니다.
        /// </summary>
        /// <param name="stock"></param>
        public IOStockManagerViewModel(ObservableIOStock stock) : this(stock.Inventory.Product.ID)
        {
            _target = stock;
            Title = string.Format("입출고 데이터 편집하기", stock.ID);
            IsEnabledRadioButton = true;
            IsEnabledInventoryComboBox = false;

            Quantity = stock.Quantity;
            UnitPrice = stock.UnitPrice;
            SelectedEmployee = stock.Employee;
            Memo = stock.Memo;
            SelectedDate = stock.Date;
            SelectedInventory = stock.Inventory as ObservableInventory;
            StockType = stock.StockType;

            switch (StockType)
            {
                case IOStockType.INCOMING:
                    SelectedAccount = stock.Supplier;
                    SelectedProject = stock.Warehouse;
                    break;
                case IOStockType.OUTGOING:
                    SelectedAccount = stock.Customer;
                    SelectedProject = stock.Project;
                    break;
            }
        }

        /// <summary>
        /// 새로운 입출고 데이터를 등록합니다. 이 때 규격은 선택된 상태입니다.
        /// </summary>
        /// <param name="inventory"></param>
        public IOStockManagerViewModel(ObservableInventory inventory) : this(inventory.Product)
        {
            Title = string.Format("새로운 입출고 데이터 등록하기");
            IsEnabledRadioButton = true;
            IsEnabledInventoryComboBox = false;
            SelectedInventory = inventory;
        }

        /// <summary>
        /// 새로운 입출고 데이터를 등록합니다. 이 때 규격은 선택되지 않은 상태입니다.
        /// </summary>
        /// <param name="inventory"></param>
        public IOStockManagerViewModel(Observable<Product> product) : this(product.ID)
        {
            Title = string.Format("새로운 입출고 데이터 등록하기");
            UnitPrice = 0;
            Quantity = 1;
            IsEnabledRadioButton = false;
            IsEnabledInventoryComboBox = true;
            StockType = IOStockType.INCOMING;
            SelectedDate = DateTime.Now;
        }

        private IOStockManagerViewModel(string productID)
        {
            InitComboboxItemsSources(productID);
            RecordCommand = new RelayCommand(ExecuteRecordCommand, CanRecord);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        public IOStockManagerViewModel(IOStockStatusViewModel iOStockStatusViewModel, ObservableIOStock stock)
            : this(stock)
        {
            _iOStockStatusViewModel = iOStockStatusViewModel;
        }

        #region viewmodel binding properties

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged -= value;
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        public string Title
        {
            get;
            private set;
        }

        public bool IsEnabledRadioButton
        {
            get; private set;
        }

        /// <summary>
        /// radiobutton binding property
        /// </summary>
        public IOStockType StockType
        {
            get
            {
                return _stockType;
            }
            set
            {
                _stockType = value;
                DataDirector idc = DataDirector.GetInstance();
                switch (_stockType)
                {
                    case IOStockType.INCOMING: //suppier, warehouse list set
                        Accounts = _suppiers;
                        Projects = _warehouses;
                        break;

                    case IOStockType.OUTGOING: //customer, project list set
                        Accounts = _customers;
                        Projects = _projects;
                        break;

                    default:
                        throw new NotSupportedException();
                }
                SelectedAccount = null;
                SelectedProject = null;
                NotifyPropertyChanged(nameof(StockType));
                SetUnitPriceAndAccount();
            }
        }

        /// <summary>
        /// DatePicker control SelectedDate property binding
        /// </summary>
        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                NotifyPropertyChanged(nameof(SelectedDate));
            }
        }

        /// <summary>
        /// inventory list for combobox
        /// </summary>
        public IEnumerable<ObservableInventory> Inventories { get; private set; }

        /// <summary>
        /// selected inventory
        /// </summary>
        public ObservableInventory SelectedInventory
        {
            get
            {
                return _selectedInventory;
            }
            set
            {
                _selectedInventory = value;
                NotifyPropertyChanged(nameof(SelectedInventory));
                RecordCommand.RaiseCanExecuteChanged();
                SetUnitPriceAndAccount();
            }
        }

        /// <summary>
        /// inventory combobox IsEnabled binding property
        /// </summary>
        public bool IsEnabledInventoryComboBox { get; private set; }

        /// <summary>
        /// quantity
        /// </summary>
        public int Quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                _quantity = value;
                NotifyPropertyChanged(nameof(Quantity));
            }
        }

        /// <summary>
        /// product unit price
        /// </summary>
        public decimal UnitPrice
        {
            get
            {
                return _unitPrice;
            }
            set
            {
                _unitPrice = value;
                NotifyPropertyChanged(nameof(UnitPrice));
            }
        }

        /// <summary>
        /// surppier or customer list for combobox
        /// </summary>
        public IEnumerable<IObservableField> Accounts
        {
            get
            {
                return _accounts;
            }
            private set
            {
                _accounts = value;
                NotifyPropertyChanged(nameof(Accounts));
            }
        }

        /// <summary>
        /// selected surppier or customer object in combobox
        /// </summary>
        public IObservableField SelectedAccount
        {
            get
            {
                return _selectedAccount;
            }
            set
            {
                _selectedAccount = value;
                NotifyPropertyChanged(nameof(SelectedAccount));
            }
        }

        /// <summary>
        /// account combobox text
        /// </summary>
        public string AccountText
        {
            get
            {
                return _accountText;
            }
            set
            {
                _accountText = value;
                NotifyPropertyChanged(nameof(AccountText));
            }
        }

        /// <summary>
        /// project or warehouse list for combobox
        /// </summary>
        public IEnumerable<IObservableField> Projects
        {
            get
            {
                return _proejects;
            }
            private set
            {
                _proejects = value;
                NotifyPropertyChanged(nameof(Projects));
            }
        }

        /// <summary>
        /// selected project warehouse for combobox
        /// </summary>
        public IObservableField SelectedProject
        {
            get
            {
                return _selectedProject;
            }
            set
            {
                _selectedProject = value;
                NotifyPropertyChanged(nameof(SelectedProject));
            }
        }

        /// <summary>
        /// second combobox text
        /// </summary>
        public string ProjectText
        {
            get
            {
                return _projectText;
            }
            set
            {
                _projectText = value;
                NotifyPropertyChanged(nameof(ProjectText));
            }
        }

        /// <summary>
        /// employee list
        /// </summary>
        public IEnumerable<Observable<Employee>> Employees { get; private set; }

        /// <summary>
        /// selected employee obj
        /// </summary>
        public Observable<Employee> SelectedEmployee
        {
            get
            {
                return _selectedEmployee;
            }
            set
            {
                _selectedEmployee = value;
                NotifyPropertyChanged(nameof(SelectedEmployee));
            }
        }

        /// <summary>
        /// employee combobox text
        /// </summary>
        public string EmployeeText
        {
            get
            {
                return _employeeText;
            }
            set
            {
                _employeeText = value;
                NotifyPropertyChanged(nameof(EmployeeText));
            }
        }

        /// <summary>
        /// memo for iostock
        /// </summary>
        public string Memo
        {
            get
            {
                return _memo;
            }
            set
            {
                _memo = value;
                NotifyPropertyChanged(nameof(Memo));
            }
        }

        public RelayCommand RecordCommand
        {
            get;
            private set;
        }

        public RelayCommand CancelCommand
        {
            get;
            private set;
        }

        #endregion viewmodel binding properties

        /// <summary>
        /// initialze combobox items sources
        /// </summary>
        /// <param name="productID"></param>
        public void InitComboboxItemsSources(string productID)
        {
            DataDirector idc = DataDirector.GetInstance();
            IEnumerable<ObservableInventory> inventories = idc.SearchInventories(productID);
            Inventories = new ObservableCollection<ObservableInventory>(inventories);

            IEnumerable<Observable<Employee>> employees = idc.CopyFields<Employee>();
            Employees = new ObservableCollection<Observable<Employee>>(employees);

            IEnumerable<Observable<Supplier>> suppiers = idc.CopyFields<Supplier>();
            _suppiers = new ObservableCollection<Observable<Supplier>>(suppiers);

            IEnumerable<Observable<Warehouse>> warehouses = idc.CopyFields<Warehouse>();
            _warehouses = new ObservableCollection<Observable<Warehouse>>(warehouses);

            IEnumerable<Observable<Customer>> customers = idc.CopyFields<Customer>();
            _customers = new ObservableCollection<Observable<Customer>>(customers);

            IEnumerable<Observable<Project>> projects = idc.CopyFields<Project>();
            _projects = new ObservableCollection<Observable<Project>>(projects);
        }

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public ObservableIOStock Insert()
        {
            if (SelectedInventory == null)
                throw new Exception();

            CreateBindingProperties();

            var fmt = CreateIOStockFormat();
            DataDirector.GetInstance().DB.Insert<IOStockFormat>(fmt);

            return new ObservableIOStock(fmt);
        }

        public ObservableIOStock Update()
        {
            if (SelectedInventory == null)
                throw new Exception();
            ObservableIOStock origin = _target;
            IOStockType bType = origin.StockType;
            int bQty = origin.Quantity;
            DateTime bDate = origin.Date;

            ModifyBindingProperties();
            CreateBindingProperties();
            IOStockFormat modify = CreateIOStockFormat();
            modify.ID = origin.ID;

            PropertyInfo[] properties = modify.GetType().GetProperties();
            foreach (PropertyInfo modifyProperty in properties)
            {
                if (modifyProperty.PropertyType.IsNotPublic)
                    continue;
                string pname = modifyProperty.Name;
                PropertyInfo originProperty = origin.GetType().GetProperty(pname);
                object v1 = originProperty.GetValue(origin);
                object v2 = modifyProperty.GetValue(modify);
                if (v1 != v2)
                    originProperty.SetValue(origin, v2);
            }
            //for datagrid update
            if (_iOStockStatusViewModel != null)
            {
                if (bType != origin.StockType || bDate != origin.Date)
                {
                    _iOStockStatusViewModel.SetDataGridItems();
                    _iOStockStatusViewModel.CalcRemainQuantity();
                }
                else if (bQty != origin.Quantity)
                    _iOStockStatusViewModel.CalcRemainQuantity();
            }
            return origin;
        }

        private void CreateBindingProperties()
        {
            var account = SelectedAccount;
            var project = SelectedProject;
            var employee = SelectedEmployee;

            switch (StockType)
            {
                case IOStockType.INCOMING:
                    if (account == null && AccountText != null)
                    {
                        account = new Observable<Supplier>(AccountText);
                        DataDirector.GetInstance().AddField(account);
                        SelectedAccount = account;
                    }
                    if (project == null && ProjectText != null)
                    {
                        project = new Observable<Warehouse>(ProjectText);
                        DataDirector.GetInstance().AddField(project);
                        SelectedProject = project;
                    }
                    break;

                case IOStockType.OUTGOING:
                    if (account == null && AccountText != null)
                    {
                        account = new Observable<Customer>(AccountText);
                        DataDirector.GetInstance().AddField(account);
                        SelectedAccount = account;
                    }
                    if (project == null && ProjectText != null)
                    {
                        project = new Observable<Project>(ProjectText);
                        DataDirector.GetInstance().AddField(project);
                        SelectedProject = project;
                    }
                    break;
            }
            if (employee == null && EmployeeText != null)
            {
                employee = new Observable<Employee>(EmployeeText);
                DataDirector.GetInstance().AddField(employee);
                SelectedEmployee = employee;
            }
        }

        private IOStockFormat CreateIOStockFormat()
        {
            var account = SelectedAccount;
            var project = SelectedProject;
            var employee = SelectedEmployee;

            IOStockFormat fmt = new IOStockFormat();
            fmt.StockType = StockType;
            fmt.Date = SelectedDate;
            fmt.Memo = Memo;
            fmt.Quantity = Quantity;
            fmt.UnitPrice = UnitPrice;

            switch (StockType)
            {
                case IOStockType.INCOMING:
                    if (account != null)
                        fmt.SupplierID = account.ID;
                    if (project != null)
                        fmt.WarehouseID = project.ID;
                    break;

                case IOStockType.OUTGOING:
                    if (account != null)
                        fmt.CustomerID = account.ID;
                    if (project != null)
                        fmt.ProjectID = project.ID;
                    break;
            }
            if (employee != null)
                fmt.EmployeeID = employee.ID;

            fmt.InventoryID = SelectedInventory.ID;
            fmt.ID = Guid.NewGuid().ToString();
            return fmt;
        }

        private void ModifyBindingProperties()
        {
            var account = SelectedAccount;
            var project = SelectedProject;
            var employee = SelectedEmployee;
            ObservableIOStock origin = _target;

            switch (StockType)
            {
                case IOStockType.INCOMING: //UPDATE 실행
                    if (origin.Supplier != null && account == null)
                    {
                        origin.Supplier.Name = AccountText;
                        SelectedAccount = origin.Supplier;
                    }
                    if (origin.Warehouse != null && project == null)
                    {
                        origin.Warehouse.Name = ProjectText;
                        SelectedProject = origin.Warehouse;
                    }
                    break;
                case IOStockType.OUTGOING:
                    if (origin.Customer != null && account == null)
                    {
                        origin.Customer.Name = AccountText;
                        SelectedAccount = origin.Customer;
                    }
                    if (origin.Project != null && project == null)
                    {
                        origin.Project.Name = ProjectText;
                        SelectedProject = origin.Project;
                    }
                    break;
            }
            if (origin.Employee != null && employee == null)
            {
                origin.Employee.Name = EmployeeText;
                SelectedEmployee = origin.Employee;
            }
        }

        private void ExecuteCancelCommand()
        {
            var window = Application.Current.Windows.OfType<Window>().Where(x => x.IsActive).FirstOrDefault();
            if (window != null)
                window.Close();
        }

        private bool CanRecord()
        {
            return SelectedInventory != null;
        }

        private void ExecuteRecordCommand()
        {
            if (_target == null)
                Insert();
            else
                Update();

            ExecuteCancelCommand();
        }

        private void SetUnitPriceAndAccount()
        {
            if (_target != null || SelectedInventory == null)
                return;
            string sql = string.Format("select UnitPrice from {0} where InventoryID = '{1}' and StockType = '{2}' order by Date desc limit 1",
                            nameof(IOStockFormat), SelectedInventory.ID, (int)IOStockType.INCOMING);
            List<Tuple<decimal>> pTuples = null;
            List<Tuple<string>> cTuples = null;
            if (StockType == IOStockType.OUTGOING)
            {
                sql = string.Format(@"select ifnull((select UnitPrice from {0} where InventoryID = '{1}' and StockType = '{2}' order by Date desc limit 1), ({3}))",
                        nameof(IOStockFormat), SelectedInventory.ID, (int)IOStockType.OUTGOING, sql);
            }
            pTuples = DataDirector.GetInstance().DB.QueryReturnTuple<decimal>(sql);
            Tuple<decimal> pTuple = pTuples.SingleOrDefault();
            if (pTuple != null)
                UnitPrice = pTuple.Item1;

            string idName = StockType == IOStockType.INCOMING ? "SupplierID" : "CustomerID";
            sql = string.Format("select {0} from {1} where InventoryID = '{2}' and StockType = '{3}' order by Date desc limit 1",
                idName, nameof(IOStockFormat), SelectedInventory.ID, (int)StockType);
            cTuples = DataDirector.GetInstance().DB.QueryReturnTuple<string>(sql);
            Tuple<string> cTuple = cTuples.SingleOrDefault();
            if (cTuple != null)
                SelectedAccount = Accounts.Where(x => x.ID == cTuple.Item1).SingleOrDefault();
        }
    }
}