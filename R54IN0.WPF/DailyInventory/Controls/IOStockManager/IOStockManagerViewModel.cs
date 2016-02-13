using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    public class IOStockManagerViewModel
    {
        private IOStockType _stockType;
        private ObservableCollection<Observable<Supplier>> _suppiers;
        private ObservableCollection<Observable<Warehouse>> _warehouses;
        private ObservableCollection<Observable<Customer>> _customers;
        private ObservableCollection<Observable<Project>> _projects;

        #region viewmodel binding properties 
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
                InventoryDataCommander idc = InventoryDataCommander.GetInstance();
                switch (_stockType)
                {
                    case IOStockType.INCOMING: //suppier, warehouse list set
                        Accounts = _suppiers;
                        SecondComboboxItemsSource = _warehouses;
                        break;
                    case IOStockType.OUTGOING: //customer, project list set
                        Accounts = _customers;
                        SecondComboboxItemsSource = _projects;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// DatePicker control SelectedDate property binding
        /// </summary>
        public DateTime SelectedDate { get; set; }

        /// <summary>
        /// inventory list for combobox
        /// </summary>
        public IEnumerable<ObservableInventory> Inventories { get; private set; }
        /// <summary>
        /// selected inventory 
        /// </summary>
        public ObservableInventory SelectedInventory { get; set; }
        /// <summary>
        /// inventory combobox IsEnabled binding property
        /// </summary>
        public bool IsEnabledInventoryComboBox { get; private set; }

        /// <summary>
        /// quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// product unit price 
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// surppier or customer list for combobox
        /// </summary>
        public IEnumerable<IObservableField> Accounts { get; private set; }
        /// <summary>
        /// selected surppier or customer object in combobox
        /// </summary>
        public IObservableField Account { get; set; }
        /// <summary>
        /// account combobox text
        /// </summary>
        public string AccountText { get; set; }

        /// <summary>
        /// project or warehouse list for combobox
        /// </summary>
        public IEnumerable<IObservableField> SecondComboboxItemsSource { get; private set; }
        /// <summary>
        /// selected project warehouse for combobox
        /// </summary>
        public IObservableField SelectedSecondComboboxItem { get; set; }
        /// <summary>
        /// second combobox text
        /// </summary>
        public string SecondComboboxText { get; set; }

        /// <summary>
        /// employee list
        /// </summary>
        public IEnumerable<Observable<Employee>> Employees { get; private set; }
        /// <summary>
        /// selected employee obj
        /// </summary>
        public Observable<Employee> SelectedEmployee { get; set; }
        /// <summary>
        /// employee combobox text
        /// </summary>
        public string EmployeeText { get; set; }

        /// <summary>
        /// memo for iostock 
        /// </summary>
        public string Memo { get; set; }

        #endregion

        /// <summary>
        /// 새로운 입출고 데이터를 등록합니다. 이 때 규격은 선택되지 않은 상태입니다.
        /// </summary>
        /// <param name="inventory"></param>
        public IOStockManagerViewModel(Observable<Product> product)
        {
            Quantity = 1;
            IsEnabledInventoryComboBox = true;
            InitComboboxItemsSources(product);
            StockType = IOStockType.INCOMING;
            SelectedDate = DateTime.Now;
        }

        /// <summary>
        /// 새로운 입출고 데이터를 등록합니다. 이 때 규격은 선택된 상태입니다.
        /// </summary>
        /// <param name="inventory"></param>
        public IOStockManagerViewModel(ObservableInventory inventory) : this(inventory.Product)
        {
            IsEnabledInventoryComboBox = false;
            SelectedInventory = inventory;
        }

        /// <summary>
        /// 기존의 입출고 데이터를 수정합니다. 이 떄 규격은 변경되지 아니합니다.
        /// </summary>
        /// <param name="iostock"></param>
        public IOStockManagerViewModel(ObservableIOStock iostock) : this(iostock.Inventory as ObservableInventory)
        {
            IsEnabledInventoryComboBox = false;
            Quantity = iostock.Quantity;
            UnitPrice = iostock.UnitPrice;
            SelectedEmployee = iostock.Employee;
            Memo = iostock.Memo;
            SelectedDate = iostock.Date;
            if (StockType != iostock.StockType)
                StockType = iostock.StockType;
            switch (StockType)
            {
                case IOStockType.INCOMING:
                    Account = iostock.Supplier;
                    SelectedSecondComboboxItem = iostock.Warehouse;
                    break;
                case IOStockType.OUTGOING:
                    Account = iostock.Customer;
                    SelectedSecondComboboxItem = iostock.Project;
                    break;
            }
        }

        /// <summary>
        /// initialze combobox items sources
        /// </summary>
        /// <param name="product"></param>
        public void InitComboboxItemsSources(Observable<Product> product)
        {
            InventoryDataCommander idc = InventoryDataCommander.GetInstance();
            IEnumerable<ObservableInventory> inventories = idc.SearchInventoryAsProductID(product.ID);
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
    }
}